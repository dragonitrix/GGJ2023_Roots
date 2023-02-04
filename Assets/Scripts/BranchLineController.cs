using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class BranchLineController : MonoBehaviour
{
    public Branch branch_parent;
    public LineRenderer lineRenderer;

    //public List<Transform> knobs = new List<Transform>();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpdateLineRenderer()
    {
        if (lineRenderer.positionCount != branch_parent.depth + 1)
        {
            lineRenderer.positionCount = branch_parent.depth + 1;
        }

        var knobs = branch_parent.knobs;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            var pos = knobs[i].transform.position;
            pos.z = 1f;
            lineRenderer.SetPosition(i, pos);
        }
    }

}
