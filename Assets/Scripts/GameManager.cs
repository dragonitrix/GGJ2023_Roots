using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Prefabs")]
    public GameObject branch_knob_prefab;
    public GameObject sub_branch_prefab;

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

        var controlPoints = GameObject.FindGameObjectsWithTag("ControlPoints");

        foreach (var item in controlPoints)
        {
            var sr = item.GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
