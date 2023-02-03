using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BranchController;

public class Branch : MonoBehaviour
{
    [HideInInspector]
    public BranchController parent;

    [Header("Obj Ref")]
    public LineRenderer lineRenderer;

    [Header("Properties")]
    public Vector2 origin;
    public List<int> branchVerticesIndex = new List<int>();
    public List<BranchVertex> branchVertices = new List<BranchVertex>();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
    }

    public void getBranchVertices()
    {
        branchVertices.Clear();
        for (int i = 0; i < branchVerticesIndex.Count; i++)
        {
            branchVertices.Add(parent.depthVertices[i][branchVerticesIndex[i]]);
        }
    }

    public void updateLineRenderer()
    {
        lineRenderer.positionCount = branchVertices.Count +1 ;

        lineRenderer.SetPosition(0, origin);

        for (int i = 0; i < branchVertices.Count; i++)
        {
            lineRenderer.SetPosition(i+1, branchVertices[i].pos);
        }
    }

}
