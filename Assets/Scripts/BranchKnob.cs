using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchKnob : MonoBehaviour
{
    public Roots branch_parent;

    private void OnMouseDown()
    {
        branch_parent.OnKnobClicked(this);
    }

}
