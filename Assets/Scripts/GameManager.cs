using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public CanvasGroup thunder_canvas;

    [Header("Properties")]
    public int depthPoints = 0;
    public int depthPoints_max = 0;
    public RectTransform depthPoints_bar;
    public RectTransform depthPoints_deplete_bar;

    [Header("Storm Approaching")]
    public float storm_elapsed = 0f;
    public float storm_duration_idle = 3f;
    public float storm_duration_charge = 2f;
    public float storm_duration_attacking = 1f;
    public float storm_duration_weak = 2f;

    public int storm_attack_count = 0;
    public int storm_attack_count_max = 0;

    public enum GameState
    {
        _INTRO,
        _STORM
    }
    public GameState state;
    public enum StormState
    {
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
            case GameState._INTRO:
                break;
            case GameState._STORM:
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

        depthPoints = depthPoints_max;


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

        Debug.Log("depthPoints: " + depthPoints);
        Debug.Log("depthPoints_max: " + depthPoints_max);
        UpdateDepthPointBar();
        //SetState(GameState._STORM);

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
        var depthPoints_ratio = 1-((float)depthPoints / 10);
        var depthPoints_max_ratio =1-( (float)depthPoints_max / 10);

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

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState._INTRO:
                break;
            case GameState._STORM:

                UpdateStorm();


                break;
        }
    }

    public void UpdateStorm()
    {
        storm_elapsed += Time.deltaTime;

        switch (stormState)
        {
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
        Debug.Log("WindupAttack");
        // pick target
        attackRadius = 2f;

        var targetBranch = branchList[0];

        for (int i = 0; i < branchList.Count; i++)
        {
            if (branchList[i].depth > targetBranch.depth)
            {
                targetBranch = branchList[i];
            }
        }
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
        thunder.ThunderDispelled();

        Debug.Log("isHitted: " + isHitted);
        if (isHitted)
        {
            var particle = Instantiate(smoke_particle_prefab, targetPosition, Quaternion.identity, transform);
            particle.transform.position = new Vector3(particle.transform.position.x, particle.transform.position.y, particle.transform.position.z - 1);
            Destroy(particle, 5f);
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

}
