using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Roots : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject sub_branch_prefab;

    [Header("Obj Ref")]
    public Transform start_point;
    public Transform end_point;
    public Transform min_angle_point;
    public Transform max_angle_point;
    public BranchLineController line;
    public Transform knobsGroup;


    [Header("Settings")]
    public int index = 0;
    public int depth = 0;
    public int start_depth = 3;
    public int min_depth = 1;
    public int max_depth = 5;
    public float depth_treshold = 50f;
    public float depth_length = 1f;
    public float sub_depth_length = 1f;

    public Transform subBranchGroup;
    public List<SubBranchLineController> subBranchLines = new List<SubBranchLineController>();

    [HideInInspector]
    public List<BranchKnob> knobs = new List<BranchKnob>();
    Vector2 click_origin;
    Vector2 drag_origin;
    bool start_drag = false;
    float mainAngle;

    // Start is called before the first frame update
    void Start()
    {
        mainAngle = getAngle(start_point.position, end_point.position);

        //fetch knobs;
        var knobs_arr = knobsGroup.GetComponentsInChildren<BranchKnob>();
        knobs.Clear();
        knobs = knobs_arr.ToList<BranchKnob>();
        max_depth = knobs.Count - 1;

        for (int i = min_depth + 1; i < knobs.Count; i++)
        {
            randomKnobPos(i);
        }

        //SetDepth(start_depth);
        //SetDepth(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (start_drag)
        {
            if (Input.GetMouseButton(0))
            {
                doDrag();
            }
            if (Input.GetMouseButtonUp(0))
            {
                endDrag();
            }
        }
    }
    public void OnKnobClicked(BranchKnob branchKnob)
    {
        var cam = Camera.main;
        //var cam = GameManager.instance.root_cam;
        var knobPos = branchKnob.transform.position;
        click_origin = cam.WorldToScreenPoint(knobPos);
        drag_origin = cam.WorldToScreenPoint(knobPos);
        startDrag();
    }
    void startDrag()
    {
        start_drag = true;
    }
    void doDrag()
    {
        var mousePos = Input.mousePosition;
        var deltaVector = (Vector2)mousePos - drag_origin;
        var length = deltaVector.magnitude;

        if (length >= depth_treshold)
        {
            //Debug.Log("tresholdReached");
            if (deltaVector.y > 0) // up
            {
                PrevDepth();
            }
            else // down
            {
                NextDepth();
            }
            //drag_origin = Camera.main.WorldToScreenPoint(knobs[depth].transform.position);
            drag_origin = mousePos;
        }
    }
    void endDrag()
    {
        start_drag = false;
    }

    [ContextMenu("SetDepth")]
    public void SetDepth()
    {
        SetDepth(depth);
    }
    public virtual void SetDepth(int depth)
    {
        this.depth = depth;
        line.UpdatePosList();
        line.UpdateLineRenderer();
        generateSubBranch();
    }
    public void PrevDepth()
    {
        if (depth <= min_depth)
        {
            return;
        }
        SetDepth(depth - 1);
        GameManager.instance.OnRootDepthChanged(this);
    }

    public void NextDepth()
    {
        if (depth >= max_depth)
        {
            return;
        }
        // alter next depth to create more varient

        //var currentKnob = knobs[depth];
        //var nextKnob = knobs[depth+1];
        randomKnobPos(depth + 1);
        SetDepth(depth + 1);
        GameManager.instance.OnRootDepthChanged(this);
    }

    public void randomKnobPos(int index)
    {
        if (index == 0)
        {
            return;
        }
        var prev_knob = knobs[index - 1];
        var knob = knobs[index];
        //var length = ((Vector2)prev_knob.transform.position - (Vector2)knob.transform.position).magnitude;

        //var length = Random.Range(depth_length * 1f, depth_length * 1.5f);
        var depth01 = (float)index / (float)max_depth;
        var length = depth01.Remap(0, 1, depth_length * 1.5f, depth_length * 1f);

        var min_angle = getAngle(start_point.position, min_angle_point.position);
        var max_angle = getAngle(start_point.position, max_angle_point.position);

        var prev_angle = getAngle(start_point.position, prev_knob.transform.position);

        float randomAngle = 0f;
        var gap = 10f * Mathf.Deg2Rad;
        if (prev_angle - mainAngle > 0)
        {
            randomAngle = Random.Range(min_angle, mainAngle - gap);
        }
        else
        {
            randomAngle = Random.Range(mainAngle + gap, max_angle);
        }
        //var randomAngle = Random.Range(min_angle, max_angle);

        var newVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
        knob.transform.position = (Vector2)prev_knob.transform.position + (newVector.normalized * length);

        // Debug.Log("length: " + length);
        // Debug.Log("min_angle: " + min_angle);
        // Debug.Log("max_angle: " + max_angle);
        // Debug.Log("randomAngle: " + randomAngle);
        // Debug.Log("newVector: " + newVector);


    }

    public float getAngle(Vector2 v1, Vector2 v2)
    {
        var v = v2 - v1;
        var a = Mathf.Atan2(v.y, v.x);
        if (a < 0) a += Mathf.PI * 2;
        return a;
    }

    public virtual void generateSubBranch()
    {
        var minDepth = 1;
        if (depth < minDepth)
        {
            return;
        }

        foreach (Transform t in subBranchGroup)
        {
            Destroy(t.gameObject);
        }
        subBranchLines.Clear();

        for (int i = minDepth; i < depth; i++)
        {
            createSubBranchOnKnob(i);
        }
    }

    public void createSubBranchOnKnob(int index)
    {
        if (index == 0)
        {
            return;
        }
        var knob = knobs[index];
        var prev_knob = knobs[index - 1];
        //var subBranch_obj = Instantiate(GameManager.instance.sub_branch_prefab, subBranchGroup);
        var subBranch_obj = Instantiate(sub_branch_prefab, subBranchGroup);
        var subBranch = subBranch_obj.GetComponent<SubBranchLineController>();
        subBranch.branch_parent = this;
        subBranchLines.Add(subBranch);

        //var length = Random.Range(depth_length * 0.2f, depth_length * 0.5f);
        var length = depth_length * 1f;
        //var min_angle = -30f * Mathf.Deg2Rad;
        //var max_angle = 30f * Mathf.Deg2Rad;
        //var knob_angle = getAngle(start_point.position, knob.transform.position);
        //float randomAngle = 0f;
        //var gap = 30f * Mathf.Deg2Rad;
        //if (knob_angle - mainAngle > 0)
        //{
        //    randomAngle = Random.Range(mainAngle + gap, mainAngle + gap + max_angle);
        //}
        //else
        //{
        //    randomAngle = Random.Range(mainAngle - gap - min_angle, mainAngle - gap);
        //}
        //var newVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        var lineWidth = line.lineRenderer.widthCurve.Evaluate((float)index / (float)knobs.Count) / 2f;
        var knob_angle = getAngle(prev_knob.transform.position, knob.transform.position);
        var newVector = new Vector2(Mathf.Cos(knob_angle), Mathf.Sin(knob_angle));
        var newPos1 = (Vector2)knob.transform.position + (newVector.normalized * length * 0.7f);

        var randomAngle2 = knob_angle + (Random.Range(-40f, 40f) * Mathf.Deg2Rad);
        var newVector2 = new Vector2(Mathf.Cos(randomAngle2), Mathf.Sin(randomAngle2));
        var newPos2 = newPos1 + (newVector2.normalized * length * 0.3f);

        //var posList = new List<Vector2>();
        //posList.Add(knob.transform.position);
        //posList.Add(newPos);

        subBranch.posList.Clear();
        subBranch.posList.Add(knob.transform.position + (Vector3)(newVector.normalized * lineWidth * 0.5f));
        subBranch.posList.Add(newPos1);
        subBranch.posList.Add(newPos2);
        subBranch.UpdateLineRenderer();
    }
}
