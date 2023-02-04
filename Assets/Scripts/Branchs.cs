using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branchs : Roots
{

    public GameObject leaf_prefab;
    public Transform leavesGroup;
    public float min_leafSize = 3f;
    public float max_leafSize = 5f;

    void Update()
    {
    }

    public override void generateSubBranch()
    {
        base.generateSubBranch();
    }

    public override void SetDepth(int depth)
    {
        base.SetDepth(depth);
        generateLaeves();
    }

    public void generateLaeves()
    {
        foreach (Transform l in leavesGroup)
        {
            Destroy(l.gameObject);
        }
        subBranchLines.Clear();

        for (int i = 0; i < depth; i++)
        {
            var leaf_obj = Instantiate(leaf_prefab, leavesGroup);
            leaf_obj.transform.position = knobs[i].transform.position;
            leaf_obj.transform.localScale = Vector3.one * ((float)i/(float)(max_depth-1)).Remap(0,1,min_leafSize,max_leafSize);
        }
    }

}
