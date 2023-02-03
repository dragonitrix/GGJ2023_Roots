using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static ExtensionMethods;

public class BranchController : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject branch_prefab;

    //[Header("Object ref")]
    //public List<Transform> branch_transform;
    [Header("Settings")]
    public bool randomAngle;
    public float depthWidth_start = 5f;
    public float depthWidth_end = 2f;
    public float totalAngle = 180f;
    public float startAngle = 180f;

    [Header("properties")]
    public Vector2 origin;
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

    [ContextMenu("updateVerticesPos")]
    public void updateVerticesPos()
    {
        generateDepthVertices();
        setVerticesPos();
        generateBranch();
    }


    public void setVerticesPos()
    {
        //var totalAngle = (float)Math.PI * 2f;
        //var totalAngle = 180;
        //var startAngle = 0f;

        //Debug.Log("totalAngle: " + totalAngle);


        var depthWidth = 1f;

        for (int i = 0; i < depthVertices.Count; i++)
        {
            var totalAngle = this.totalAngle;

            var angleDelta = totalAngle / (float)(depthVertices[i].Count - 1);

            if (randomAngle)
            {
                totalAngle -= angleDelta * 0.5f;
                angleDelta = totalAngle / (float)(depthVertices[i].Count - 1);
            }


            //var angle = startAngle + (angleDelta / 2f);
            //var angle = startAngle;
            var angle = startAngle - (totalAngle / 2);

            var min_random_angle = (totalAngle - (angleDelta * 1.5f)) / (depthVertices[i].Count - 2);
            var max_random_angle = (totalAngle - (angleDelta * 0.5f)) / (depthVertices[i].Count - 2);

            // Debug.Log("-----");
            // Debug.Log("depth: " + i);
            // Debug.Log("totalAngle: " + totalAngle);
            // Debug.Log("angleDelta: " + angleDelta);
            // Debug.Log("min_random_angle: " + min_random_angle);
            // Debug.Log("max_random_angle: " + max_random_angle);

            // Debug.Log("angleDelta: " + angleDelta);
            // Debug.Log("angle: " + angle);

            for (int j = 0; j < depthVertices[i].Count; j++)
            {
                //depthVertices[i][j].pos = origin + RadianToVector2(angle).normalized * (depthWidth * (i + 1));

                if (j >= depthVertices[i].Count - 1)
                {
                    angle = startAngle - (totalAngle / 2) + totalAngle;
                }

                depthVertices[i][j].pos = origin + DegreeToVector2(angle).normalized * (depthWidth * (i + 1));
                if (randomAngle)
                {
                    angle += Random.Range(min_random_angle, max_random_angle);
                }
                else
                {
                    angle += angleDelta;
                }

            }
        }

    }

    public void generateBranch()
    {
        foreach (var item in branches)
        {
            Destroy(item.gameObject);
        }
        branches.Clear();

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

            var branch_clone = Instantiate(branch_prefab, this.transform);
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

            branches.Add(branch);

        }

    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }
    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
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
