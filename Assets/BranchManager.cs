using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BranchManager;

public class BranchManager : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject branch_prefab;

    //[Header("Object ref")]
    //public List<Transform> branch_transform;

    public Vector2 origin;
    //public int depthCount = 5;
    //public int startBranchCount = 3;
    //public float multiply = 2;


    // Start is called before the first frame update
    void Start()
    {
    }
    public List<int> depthVerticesCount = new List<int>();
    public List<List<BranchVertex>> depthVertices = new List<List<BranchVertex>>();

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
        


    }

    public class BranchVertex
    {
        public int index;
        public int depth;
        public int previousDepthBranchIndex = 0;

        public BranchVertex()
        {
            this.index = 0;
            this.depth = 0;
        }
        public BranchVertex(int index,int depth)
        {
            this.index = index;
            this.depth = depth;
        }
    }
    public class Branch
    {
        public List<BranchVertex> branchVertices = new List<BranchVertex>();
    }

}
