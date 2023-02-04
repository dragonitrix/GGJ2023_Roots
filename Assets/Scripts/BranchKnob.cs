using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchKnob : MonoBehaviour
{
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
            branch_parent.GetComponent<Branchs>().Hitted();
        }
    }


}
