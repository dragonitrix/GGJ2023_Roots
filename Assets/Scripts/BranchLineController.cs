using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchLineController : MonoBehaviour
{
    public Branch branch_parent;
    public LineRenderer lineRenderer;

    public List<Vector3> posList = new List<Vector3>();

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

    [ContextMenu("UpdateLine")]
    public void UpdateLine()
    {
        UpdatePosList();
        UpdateLineRenderer();
    }

    public virtual void UpdatePosList()
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

    public virtual void UpdateLineRenderer()
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
