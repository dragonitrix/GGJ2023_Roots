using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Prefabs")]
    public GameObject branch_knob_prefab;
    public GameObject sub_branch_prefab;

    [Header("Cam")]
    public Camera main_cam;
    public Camera root_cam;

    [Header("Obj Ref")]
    public List<Branch> rootList = new List<Branch>();
    public List<Branch> branchList = new List<Branch>();

    [Header("Properties")]
    public int depthPoints = 0;
    public int depthPoints_max = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //var controlPoints = GameObject.FindGameObjectsWithTag("ControlPoints");
        //
        //foreach (var item in controlPoints)
        //{
        //    var sr = item.GetComponent<SpriteRenderer>();
        //    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        //}

        for (int i = 0; i < rootList.Count; i++)
        {
            Branch root = rootList[i];
            root.index = i;
            depthPoints_max += root.max_depth;
            root.SetDepth(3);

        }

        for (int i = 0; i < branchList.Count; i++)
        {
            Branch branch = branchList[i];
            branch.index = i;
            branch.SetDepth(2);
        }

    }

    public void OnRootDepthChanged(Branch root)
    {
        var rootIndex = root.index;
        var rootDepth = root.depth;

        //set coresponding branch depth;

        var branch = branchList[rootIndex];

        branch.SetDepth(5 - rootDepth + 1);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
