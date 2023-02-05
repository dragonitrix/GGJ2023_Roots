using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Thunders : Roots
{

    public Transform start_point_A;
    public Transform start_point_B;

    protected override void Start()
    {
        base.Start();

        //var endPointDistance = (end_point.transform.position - start_point.transform.position).magnitude;
        //depth_length = (endPointDistance*1.3f) / (max_depth-1);
        //for (int i = min_depth + 1; i < knobs.Count; i++)
        //{
        //    randomKnobPos(i);
        //}
        //
        //SetDepth(max_depth);

    }

    public void ThunderCalled(Vector3 position)
    {
        // random start point;


        var newStartPos = new Vector3(
            Random.Range(start_point_A.position.x, start_point_B.position.x),
            start_point.position.y, 
            start_point.position.z
            );

        newStartPos = start_point.position; // debug

        start_point.position = newStartPos;
        knobs[0].transform.position = newStartPos;

        end_point.position = position;

        var endPointDistance = (end_point.position - start_point.position).magnitude;
        depth_length = (endPointDistance * 1.05f) / (max_depth - 1);

        mainAngle = getAngle(start_point.position, end_point.position);

        for (int i = 0; i < knobs.Count; i++)
        {
            randomKnobPos(i);
        }

        SetDepth(max_depth);
    }

    public void ThunderDispelled()
    {
        SetDepth(0);
    }

    public override void SetDepth(int depth)
    {
        this.depth = depth;
        line.UpdatePosList();
        line.UpdateLineRenderer();
    }

    public override void randomKnobPos(int index)
    {
        if (index == 0)
        {
            return;
        }
        var prev_knob = knobs[index - 1];
        var knob = knobs[index];

        var length = depth_length;

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

}
