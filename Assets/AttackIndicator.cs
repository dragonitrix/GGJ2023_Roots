using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicator : MonoBehaviour
{
    public float duration;
    public float elasped = 0f;
    public float radius;
    public bool start = false;
    public Transform circle;
    public Transform circleOutline;

    public void SetupIndicator(Vector2 position,float duration,float radius)
    {
        this.transform.position = position;
        this.duration = duration;
        this.radius = radius;
        elasped = 0;
        circle.transform.localScale = Vector3.zero;
        circleOutline.transform.localScale = Vector3.one * radius;
    }

    public void StartIndicator()
    {
        this.start = true;
        Destroy(this.gameObject, duration);
    }

    private void Update()
    {
        elasped += Time.deltaTime;
        circle.transform.localScale = Vector3.one * elasped.Remap(0, duration, 0, radius);
    }

}
