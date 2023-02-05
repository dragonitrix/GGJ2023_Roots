using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchKnob : MonoBehaviour
{
    public int index = 0;
    public Roots branch_parent;
    public bool isBranch = false;

    private void OnMouseDown()
    {
        branch_parent.OnKnobClicked(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBranch && collision.tag == "Hitbox")
        {
            Debug.Log("branch_parent.depth: " + branch_parent.depth);
            Debug.Log("index: " + index);

            if (branch_parent.depth >= index)
            {
                branch_parent.GetComponent<Branchs>().Hitted();
            }
            else
            {
                Debug.Log("avoided");
            }

        }
    }


}
