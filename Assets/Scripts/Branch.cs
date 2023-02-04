using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Branch : MonoBehaviour
{
    [Header("Obj Ref")]
    public Transform start_point;
    public Transform end_point;
    public Transform min_angle_point;
    public Transform max_angle_point;
    public float mainAngle;
    public BranchLineController line;
    public Transform knobsGroup;


    [Header("Settings")]
    public int depth = 0;
    public int start_depth = 3;
    public int min_depth = 1;
    public int max_depth = 5;
    public float depth_treshold = 50f;

    public float depth_length = 1f;

    //public float max_randomAngle = 20f; // degree
    //public List<Transform> knobs_transforms = new List<Transform>();
    public List<BranchKnob> knobs = new List<BranchKnob>();

    Vector2 click_origin;
    Vector2 drag_origin;
    bool start_drag = false;

    // Start is called before the first frame update
    void Start()
    {
        mainAngle = getAngle(start_point.position, end_point.position);

        //fetch knobs;
        var knobs_arr = knobsGroup.GetComponentsInChildren<BranchKnob>();
        knobs.Clear();
        knobs = knobs_arr.ToList<BranchKnob>();
        max_depth = knobs.Count - 1;
        SetDepth(start_depth);
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

    public void SetDepth(int depth)
    {
        this.depth = depth;
        line.UpdateLineRenderer();
    }
    public void PrevDepth()
    {
        if (depth <= min_depth)
        {
            return;
        }
        SetDepth(depth - 1);
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

        var length = Random.Range(depth_length * 0.5f, depth_length * 1.5f);

        var min_angle = getAngle(start_point.position, min_angle_point.position);
        var max_angle = getAngle(start_point.position, max_angle_point.position);

        var prev_angle = getAngle(prev_knob.transform.position, end_point.position);

        float randomAngle = 0f;
        var gap = 10f * Mathf.Deg2Rad; 
        if (prev_angle-mainAngle > 0)
        {
            randomAngle = Random.Range(mainAngle + gap, max_angle);
        }
        else
        {
            randomAngle = Random.Range(min_angle, mainAngle - gap);
        }

        //var randomAngle = Random.Range(min_angle, max_angle);

        //randomAngle = mainAngle + randomAngle;
        var newVector = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        // Debug.Log("length: " + length);
        // Debug.Log("min_angle: " + min_angle);
        // Debug.Log("max_angle: " + max_angle);
        // Debug.Log("randomAngle: " + randomAngle);
        // Debug.Log("newVector: " + newVector);

        knob.transform.position = (Vector2)prev_knob.transform.position + (newVector.normalized * length);

        var new_length = ((Vector2)prev_knob.transform.position - (Vector2)knob.transform.position).magnitude;
        //Debug.Log("length: " + length);
        //Debug.Log("new_length: " + new_length);
    }

    public float getAngle(Vector2 v1, Vector2 v2)
    {
        var v = v2 - v1;
        var a = Mathf.Atan2(v.y, v.x);
        if (a < 0) a += Mathf.PI*2;
        return a;
    }

}
