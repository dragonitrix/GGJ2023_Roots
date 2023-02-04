using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubBranchLineController : BranchLineController
{
    public override void UpdatePosList()
    {
        var count = branch_parent.depth + 1;
        var knobs = branch_parent.knobs;

        posList.Clear();
        for (int i = 0; i < count; i++)
        {
            var pos = knobs[i].transform.position;
            pos.z = 1f;
            posList.Add(pos);
        }
    }

    public override void UpdateLineRenderer()
    {
        //updateline
        lineRenderer.positionCount = posList.Count;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            var pos = posList[i];
            lineRenderer.SetPosition(i, pos);
        }

    }

}
