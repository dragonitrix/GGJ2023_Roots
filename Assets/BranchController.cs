using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BranchController : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject branch_prefab;

    //[Header("Object ref")]
    //public List<Transform> branch_transform;

    public Vector2 origin;
    public float depthWidth = 2f;
    //public int depthCount = 5;
    //public int startBranchCount = 3;
    //public float multiply = 2;


    // Start is called before the first frame update
    void Start()
    {
        generateDepthVertices();
        setVerticesPos();
        generateBranch();
    }
    public List<int> depthVerticesCount = new List<int>();
    public List<List<BranchVertex>> depthVertices = new List<List<BranchVertex>>();

    public List<Branch> branches = new List<Branch>();

    //public void generateDepthVerticesCount()
    //{
    //    depthVerticesCount.Clear();
    //    var count = startBranchCount;
    //
    //    for (int i = 0; i < depthCount; i++)
    //    {
    //        depthVerticesCount.Add(count);
    //        count = Mathf.RoundToInt(count * multiply);
    //    }
    //}

    public void generateDepthVertices()
    {
        depthVertices.Clear();
        for (int i = 0; i < depthVerticesCount.Count; i++)
        {
            var depth = i;
            var vertices = new List<BranchVertex>();

            for (int j = 0; j < depthVerticesCount[depth]; j++)
            {
                vertices.Add(new BranchVertex(j, depth));
            }
            depthVertices.Add(vertices);
        }
    }


    public void generateBranch()
    {
        var branchDeltas = new List<int>();

        for (int i = 0; i < depthVerticesCount.Count; i++)
        {
            if (i == 0)
            {
                branchDeltas.Add(0);
                continue;
            }
            else
            {
                var currentDepth = depthVerticesCount[i];
                var prevDepth = depthVerticesCount[i - 1];
                var ratio = Mathf.RoundToInt(currentDepth / prevDepth);
                branchDeltas.Add(ratio);
            }
        }

        // for (int i = 0; i < branchDeltas.Count; i++)
        // {
        //     Debug.Log("branchDeltas depth: " + i + " , count: " + branchDeltas[i]);
        // }


        var depth = depthVertices.Count;
        var lastDepthVertices = depthVertices[depth - 1];

        for (int i = 0; i < lastDepthVertices.Count; i++)
        {
            var index = i;

            var branch_clone = Instantiate(branch_prefab);
            var branch = branch_clone.GetComponent<Branch>();
            branch.parent = this;
            branch.origin = this.origin;
            branch.branchVerticesIndex.Add(index);

            //var indexstring = "" + (index + 1);
            for (int j = depth - 1; j > 0; j--)
            {
                index = Mathf.FloorToInt((float)index / (float)branchDeltas[j]);
                //indexstring = (index + 1) + "-" + indexstring;
                branch.branchVerticesIndex.Insert(0, index);
            }
            //Debug.Log(indexstring);

            branch.getBranchVertices();
            branch.updateLineRenderer();

        }

    }

    public void setVerticesPos()
    {
        var totalAngle = (float)Math.PI * 2f;
        var startAngle = 0f;

        //Debug.Log("totalAngle: " + totalAngle);

        //var depthWidth = 5f;

        for (int i = 0; i < depthVertices.Count; i++)
        {
            var angleDelta = totalAngle / (float)depthVertices[i].Count;
            var angle = startAngle + (angleDelta / 2f);

            // Debug.Log("angleDelta: " + angleDelta);
            // Debug.Log("angle: " + angle);

            for (int j = 0; j < depthVertices[i].Count; j++)
            {
                depthVertices[i][j].pos = origin + RadianToVector2(angle).normalized * (depthWidth * (i + 1));
                //angle += angleDelta / 2 + Random.Range(0, angleDelta / 2);
                angle += angleDelta;
            }
        }

    }
    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }


    public class BranchVertex
    {
        public int index;
        public int depth;
        public int previousDepthBranchIndex = 0;

        public Vector2 pos;

        public BranchVertex()
        {
            this.index = 0;
            this.depth = 0;
        }
        public BranchVertex(int index, int depth)
        {
            this.index = index;
            this.depth = depth;
        }
    }
}
