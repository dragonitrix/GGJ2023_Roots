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
        if (isDamaged)
        {
            damaged_elapsed += Time.deltaTime;
            if (damaged_elapsed >= damaged_cooldown)
            {
                Recover();
                damaged_elapsed = 0f;

            }
        }
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
            leaf_obj.transform.localScale = Vector3.one * ((float)i / (float)(max_depth - 1)).Remap(0, 1, min_leafSize, max_leafSize);
        }
    }

    public bool isDamaged = false;
    public float damaged_elapsed = 0f;
    public float damaged_cooldown = 5f;

    public void Hitted()
    {
        if (isDamaged)
        {
            return;
        }

        Debug.Log("hitted");

        isDamaged = true;
        damaged_elapsed = 0f;
        if (depth > min_depth)
        {
            SetDepth(depth - 1);
        }
        //do something
        GameManager.instance.OnBranchHitted(this);
    }

    public void Recover()
    {
        if (!isDamaged)
        {
            return;
        }
        isDamaged = false;
        SetDepth(depth + 1);

        GameManager.instance.OnBranchRecover(this);
    }

}
