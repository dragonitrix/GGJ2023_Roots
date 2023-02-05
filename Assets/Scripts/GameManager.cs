using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DigitalRuby.Tween;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Prefabs")]
    public GameObject attack_indicator_prefab;
    public GameObject smoke_particle_prefab;
    public GameObject attack_zone_prefab;
    public GameObject sub_branch_prefab;

    [Header("Cam")]
    public Camera main_cam;
    public Camera root_cam;

    [Header("Obj Ref")]
    public Thunders thunder;
    public List<Roots> rootList = new List<Roots>();
    public List<Branchs> branchList = new List<Branchs>();

    [Header("UI")]
    public CanvasGroup thunder_canvas;
    public RectTransform depthPoints_bar;
    public RectTransform depthPoints_deplete_bar;
    public RectTransform weather_bar;
    public ParticleSystem rain_particle;
    public Image wide_bg;
    public RectTransform progress_bar;
    public RectTransform transition_bar;
    public RectTransform transition_text;




    [Header("Properties")]
    public int depthPoints = 0;
    public int depthPoints_max = 0;
    public int depthPoints_deplete_per_hit = 2;
    int _depthPoints_max = 0;
    public int intro_move_count = 0;
    public int intro_move_count_max = 20;

    public float transition_elasped = 0f;
    public float transition_duration = 3f;


    public float prograss_bar_elapsed = 0f;
    public float prograss_bar_total = 30f;
    public float prograss_bar_rate_min = 0.5f;
    public float prograss_bar_rate_max = 1.5f;

    public int score_survived_days = 0;
    public int score_dodge_lighting = 0;

    [Header("Storm Approaching")]
    public float storm_elapsed = 0f;
    public float storm_duration_idle = 3f;
    public float storm_duration_charge = 2f;
    public float storm_duration_attacking = 1f;
    public float storm_duration_weak = 2f;

    public int storm_attack_count = 0;
    public int storm_attack_count_max = 0;

    [Header("Title screen")]
    public CanvasGroup title_title;
    public CanvasGroup title_clickArea;

    [Header("game screen")]
    public RectTransform root_panel;

    [Header("Result screen")]
    public CanvasGroup result_main_panel;
    public CanvasGroup result_covered_panel;
    public CanvasGroup result_playagain_button;
    public TextMeshProUGUI result_survive_text;
    public TextMeshProUGUI result_dodge_text;


    public enum GameState
    {
        _TITLE,
        _INTRO,
        _STORM,
        _TRANSITION,
        _GAMEOVER,
    }
    public GameState state;
    public enum StormState
    {
        _NONE,
        _IDLE,
        _CHARGE,
        _ATTACKING,
        _WEAK
    }
    public StormState stormState;
    public void SetState(GameState state)
    {
        this.state = state;
        switch (this.state)
        {
            case GameState._TITLE:
                break;
            case GameState._INTRO:
                break;
            case GameState._STORM:
                rain_particle.Play();
                SetStormState(StormState._IDLE);
                break;
            case GameState._TRANSITION:
                transition_elasped = 0f;
                break;
            case GameState._GAMEOVER:
                break;
            default:
                break;
        }
    }
    public void SetStormState(StormState state)
    {
        this.stormState = state;
        switch (this.stormState)
        {
            case StormState._IDLE:
                break;
            case StormState._CHARGE:
                break;
            case StormState._ATTACKING:
                break;
            case StormState._WEAK:
                break;
            default:
                break;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //var controlPoints = GameObject.FindGameObjectsWithTag("ControlPoints");
        //
        //foreach (var item in controlPoints)
        //{
        //    var sr = item.GetComponent<SpriteRenderer>();
        //    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        //}

        _depthPoints_max = depthPoints_max;

        //ResetRound();

        //SetState(GameState._STORM);
        SetState(GameState._TITLE);
        TitleTween();
    }

    public void ResetRound()
    {
        depthPoints_max = _depthPoints_max;
        depthPoints = _depthPoints_max;

        for (int i = 0; i < rootList.Count; i++)
        {
            Roots root = rootList[i];
            root.index = i;
            //depthPoints_max += root.max_depth;
            root.SetDepth(3);

        }

        for (int i = 0; i < branchList.Count; i++)
        {
            Roots branch = branchList[i];
            branch.index = i;
            branch.SetDepth(2);
            depthPoints -= 2;
        }

        //Debug.Log("depthPoints: " + depthPoints);
        //Debug.Log("depthPoints_max: " + depthPoints_max);
        UpdateDepthPointBar();

        prograss_bar_elapsed = 0;

    }

    public bool CheckSpendPoint()
    {
        if (depthPoints > 0)
        {
            depthPoints--;
            UpdateDepthPointBar();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReturnPoint()
    {
        depthPoints++;
        if (depthPoints >= depthPoints_max)
        {
            Debug.Log("depthPoints>=depthPoints_max");
            depthPoints = depthPoints_max;
        }
        UpdateDepthPointBar();
    }

    public void UpdateDepthPointBar()
    {
        var depthPoints_ratio = 1 - ((float)depthPoints / _depthPoints_max);
        var depthPoints_max_ratio = 1 - ((float)depthPoints_max / _depthPoints_max);

        depthPoints_bar.localScale = new Vector3((float)depthPoints_ratio, 1, 1);
        depthPoints_deplete_bar.localScale = new Vector3((float)depthPoints_max_ratio, 1, 1);

    }

    public void OnRootDepthChanged(Roots root)
    {
        var rootIndex = root.index;
        var rootDepth = root.depth;

        //set coresponding branch depth;

        var branch = branchList[rootIndex];

        branch.SetDepth(5 - rootDepth + 1);

        if (state == GameState._INTRO)
        {
            intro_move_count++;
            if (intro_move_count >= intro_move_count_max)
            {
                intro_move_count = 0;
                TweenInWeatherBar();
                SetState(GameState._STORM);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState._TITLE:
                break;
            case GameState._INTRO:
                break;
            case GameState._STORM:
                updateDayProgress();
                UpdateStorm();
                break;
            case GameState._TRANSITION:
                rain_particle.Stop();
                transition_elasped += Time.deltaTime;
                if (transition_elasped >= transition_duration)
                {
                    transition_elasped = 0;
                    ResetRound();
                    SetState(GameState._STORM);
                }
                break;
            case GameState._GAMEOVER:
                break;
        }
    }

    public void updateDayProgress()
    {
        var ratio = (float)depthPoints / (float)depthPoints_max;
        var rate = ratio.Remap(1, 0, prograss_bar_rate_min, prograss_bar_rate_max);
        prograss_bar_elapsed += Time.deltaTime * Mathf.Clamp(rate, prograss_bar_rate_min, prograss_bar_rate_max);

        if (prograss_bar_elapsed >= prograss_bar_total)
        {
            prograss_bar_elapsed = prograss_bar_total;
            Debug.Log("Day Complete");
            SetStormState(StormState._NONE);
            SetState(GameState._TRANSITION);
            score_survived_days++;
            TweenInDayTransition();
            DifficultyIncrease();
        }

        var progress_bar_length = progress_bar.parent.GetComponent<RectTransform>().sizeDelta.x;

        progress_bar.anchoredPosition = new Vector2(
            prograss_bar_elapsed.Remap(0, prograss_bar_total, 0, progress_bar_length),
            progress_bar.anchoredPosition.y
            );

    }

    public void UpdateStorm()
    {
        storm_elapsed += Time.deltaTime;

        switch (stormState)
        {
            case StormState._NONE:
                //none
                break;
            case StormState._IDLE:
                if (storm_elapsed >= storm_duration_idle)
                {
                    storm_elapsed = 0;
                    SetStormState(StormState._CHARGE);
                    WindupAttack();
                }
                break;
            case StormState._CHARGE:
                if (storm_elapsed >= storm_duration_charge)
                {
                    storm_elapsed = 0;
                    SetStormState(StormState._ATTACKING);
                    storm_attack_count++;
                    Attack();
                }
                break;
            case StormState._ATTACKING:
                if (storm_elapsed >= storm_duration_attacking)
                {
                    storm_elapsed = 0;

                    if (storm_attack_count >= storm_attack_count_max)
                    {
                        storm_attack_count = 0;
                        SetStormState(StormState._WEAK);
                    }
                    else
                    {
                        SetStormState(StormState._IDLE);
                    }
                }
                break;
            case StormState._WEAK:
                if (storm_elapsed >= storm_duration_weak)
                {
                    storm_elapsed = 0;
                    SetStormState(StormState._IDLE);
                }
                break;
        }
    }

    public Vector2 targetPosition;
    public float attackRadius;

    public bool isHitted = false;

    public void WindupAttack()
    {
        //Debug.Log("WindupAttack");
        // pick target
        attackRadius = 2f;

        var targetBranch = branchList[0];

        //if (Random.Range(0f, 1f) >= 0.5f)
        //{
        //    //pick branch random
        //    targetBranch = branchList[Random.Range(0, branchList.Count)];
        //}
        //else
        //{
        //pick branch with highest
        for (int i = 0; i < branchList.Count; i++)
        {
            if (branchList[i].depth >= targetBranch.depth)
            {
                targetBranch = branchList[i];
            }
        }
        //}

        targetPosition = targetBranch.knobs[targetBranch.depth].transform.position;

        var indicator_obj = Instantiate(attack_indicator_prefab, targetPosition, Quaternion.identity, transform);
        var indicator = indicator_obj.GetComponent<AttackIndicator>();
        indicator.SetupIndicator(targetPosition, storm_duration_charge, attackRadius);
        indicator.StartIndicator();
    }
    public void Attack()
    {
        Debug.Log("ATTACK");

        isHitted = false;

        var zone = Instantiate(attack_zone_prefab, targetPosition, Quaternion.identity, transform);
        zone.transform.localScale = Vector3.one * attackRadius;
        Destroy(zone, 0.5f);

        //thunder.ThunderCalled(targetPosition);

        StartCoroutine(thunderCall());

    }

    IEnumerator thunderCall()
    {
        thunder.ThunderCalled(targetPosition);
        yield return new WaitForSeconds(0.1f);
        thunder_canvas.alpha = 1;
        yield return new WaitForSeconds(0.05f);
        thunder_canvas.alpha = 0;

        //Debug.Log("isHitted: " + isHitted);
        if (isHitted)
        {
            var particle = Instantiate(smoke_particle_prefab, targetPosition, Quaternion.identity, transform);
            particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y, particle.transform.position.z - 1);


            depthPoints_max -= depthPoints_deplete_per_hit;
            UpdateDepthPointBar();

            if (depthPoints_max <= 0)
            {
                //Debug.Log("GAMEOVER");
                SetState(GameState._GAMEOVER);
                SetStormState(StormState._NONE);
                rain_particle.Pause();
                particle.GetComponent<ParticleSystem>().Pause();
                OnGameOver();
            }
            else
            {
                Destroy(particle, 5f);
                thunder.ThunderDispelled();
            }
        }
        else
        {
            score_dodge_lighting++;
            thunder.ThunderDispelled();
        }

        isHitted = false;

    }

    public void OnBranchHitted(Branchs branchs)
    {
        isHitted = true;
        rootList[branchs.index].start_drag = false;
        rootList[branchs.index].isEnabled = false;
    }
    public void OnBranchRecover(Branchs branchs)
    {
        rootList[branchs.index].isEnabled = true;
    }

    public void TweenInWeatherBar()
    {
        System.Action<ITween<Vector2>> updateWeatherBarPos = (t) =>
        {
            weather_bar.anchoredPosition = t.CurrentValue;
        };

        System.Action<ITween<Vector2>> weatherBarMoveCompleted = (t) =>
        {
        };

        Vector3 currentPos = new Vector2(10, 60);
        Vector3 startPos = new Vector2(10, -10);

        // completion defaults to null if not passed in
        weather_bar.gameObject.Tween("TweenInWeatherBar", currentPos, startPos, 0.5f, TweenScaleFunctions.CubicEaseIn, updateWeatherBarPos, weatherBarMoveCompleted);
    }
    public void TweenOutWeatherBar()
    {
        System.Action<ITween<Vector2>> updateWeatherBarPos = (t) =>
        {
            weather_bar.anchoredPosition = t.CurrentValue;
        };

        System.Action<ITween<Vector2>> weatherBarMoveCompleted = (t) =>
        {
        };

        Vector3 currentPos = new Vector2(10, -10);
        Vector3 startPos = new Vector2(10, 60);

        // completion defaults to null if not passed in
        weather_bar.gameObject.Tween("TweenOutWeatherBar", currentPos, startPos, 0.5f, TweenScaleFunctions.CubicEaseIn, updateWeatherBarPos, weatherBarMoveCompleted);
    }

    public void TweenInDayTransition()
    {
        System.Action<ITween<Vector2>> updateDayTransitionBar = (t) =>
        {
            transition_bar.sizeDelta = t.CurrentValue;
        };

        System.Action<ITween<Vector2>> DayTransitionMoveCompleted = (t) =>
        {

        };

        Vector3 current = new Vector2(720, 0);
        Vector3 target1 = new Vector2(720, 200);

        // completion defaults to null if not passed in
        transition_bar.gameObject.Tween("TweenInDayTransition", current, target1, 0.5f, TweenScaleFunctions.CubicEaseIn, updateDayTransitionBar)
                .ContinueWith(new Vector2Tween().Setup(target1, target1, 2f, TweenScaleFunctions.Linear, updateDayTransitionBar))
                .ContinueWith(new Vector2Tween().Setup(target1, current, 0.5f, TweenScaleFunctions.CubicEaseOut, updateDayTransitionBar, DayTransitionMoveCompleted));

        transition_text.GetComponent<TextMeshProUGUI>().text = "DAY " + (score_survived_days);

        System.Action<ITween<Vector2>> updateDayTransitionTextBar = (t) =>
        {
            transition_text.anchoredPosition = t.CurrentValue;
        };

        System.Action<ITween<Vector2>> DayTransitionTextCompleted = (t) =>
        {

        };

        Vector3 current2 = new Vector2(600, 21);
        Vector3 target2 = new Vector2(0, 21);
        Vector3 target22 = new Vector2(-600, 21);

        transition_text.anchoredPosition = current2;

        // completion defaults to null if not passed in
        transition_text.gameObject.Tween("TweenInWeatherBarText", current2, target2, 0.5f, TweenScaleFunctions.CubicEaseIn, updateDayTransitionTextBar)
                .ContinueWith(new Vector2Tween().Setup(target2, target2, 2f, TweenScaleFunctions.Linear, updateDayTransitionTextBar))
                .ContinueWith(new Vector2Tween().Setup(target2, target22, 0.5f, TweenScaleFunctions.CubicEaseOut, updateDayTransitionTextBar, DayTransitionTextCompleted));

    }


    public void OnGameOver()
    {
        result_main_panel.alpha = 1f;
        result_covered_panel.alpha = 1f;

        result_survive_text.text = score_survived_days.ToString();
        result_dodge_text.text = score_dodge_lighting.ToString();

        System.Action<ITween<float>> coveredAlphaTween = (t) =>
        {
            result_covered_panel.alpha = t.CurrentValue;
        };
        result_covered_panel.gameObject.Tween("coveredAlphaTween", 1f, 0f, 2f, TweenScaleFunctions.CubicEaseIn, coveredAlphaTween);

        System.Action<ITween<float>> playagainAlphaTween = (t) =>
        {
            result_playagain_button.alpha = t.CurrentValue;
        };
        System.Action<ITween<float>> playagainTweenComplete = (t) =>
        {
            result_playagain_button.interactable = true;
            result_playagain_button.blocksRaycasts = true;

            result_main_panel.interactable = true;
            result_main_panel.blocksRaycasts = true;
        };
        result_playagain_button.gameObject.Tween("playagainAlphaTween", 0f, 0f, 5f, TweenScaleFunctions.CubicEaseIn, playagainAlphaTween)
            .ContinueWith(new FloatTween().Setup(0f, 1f, 0.5f, TweenScaleFunctions.CubicEaseIn, playagainAlphaTween, playagainTweenComplete));

    }

    public void ReloadScene()
    {
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TitleTween()
    {
        branchList[0].SetDepth(4);
        branchList[1].SetDepth(4);
        branchList[2].SetDepth(4);
        branchList[3].SetDepth(3);

        System.Action<ITween<float>> titleAlphaTween = (t) =>
        {
            title_title.alpha = t.CurrentValue;
            title_clickArea.alpha = t.CurrentValue;
        };
        System.Action<ITween<float>> titleTweenComplete = (t) =>
        {
            title_clickArea.interactable = true;
            title_clickArea.blocksRaycasts = true;
        };
        title_title.gameObject.Tween("titleAlphaTween", 0f, 0f, 2f, TweenScaleFunctions.CubicEaseIn, titleAlphaTween)
            .ContinueWith(new FloatTween().Setup(0f, 1f, 1f, TweenScaleFunctions.CubicEaseIn, titleAlphaTween, titleTweenComplete));

    }

    public void OnTitleClickArea()
    {
        title_clickArea.interactable = false;
        title_clickArea.blocksRaycasts = false;

        SetState(GameState._INTRO);
        IntroTween();
    }

    public void IntroTween()
    {

        System.Action<ITween<float>> titleAlphaTween = (t) =>
        {
            title_title.alpha = t.CurrentValue;
            title_clickArea.alpha = t.CurrentValue;
        };
        System.Action<ITween<float>> titleTweenComplete = (t) =>
        {
            title_title.interactable = false;
            title_title.blocksRaycasts = false;
            title_clickArea.interactable = false;
            title_clickArea.blocksRaycasts = false;
        };
        title_title.gameObject.Tween("titleAlphaTween", 1f, 0f, 0.5f, TweenScaleFunctions.CubicEaseIn, titleAlphaTween, titleTweenComplete);

        System.Action<ITween<Vector2>> rootPanelTween = (t) =>
        {
            root_panel.anchoredPosition = t.CurrentValue;
        };
        System.Action<ITween<Vector2>> Vector2Complete = (t) =>
        {
            //on complete
            ResetRound();
        };
        var start = new Vector2(0, -700);
        var end = new Vector2(0, 0);
        root_panel.gameObject.Tween("rootPanelTween", start, end, 0.5f, TweenScaleFunctions.CubicEaseIn, rootPanelTween, Vector2Complete);

    }

    public void DifficultyIncrease()
    {
        storm_duration_idle -= 0.2f;
        storm_duration_charge -= 0.2f;
        storm_duration_weak -= 0.2f;

        if (storm_duration_idle <= 1f) storm_duration_idle = 1f;
        if (storm_duration_charge <= 1f) storm_duration_charge = 1f;
        if (storm_duration_weak <= 1f) storm_duration_weak = 1f;

    }

}
