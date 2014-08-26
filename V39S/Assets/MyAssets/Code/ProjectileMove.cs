using UnityEngine;
using System.Collections;

public class ProjectileMove : MonoBehaviour
{
    public Vector3 to;
    public float speed;

    Transform tr;

    void Start()
    {
        tr = transform;
        Destroy(gameObject, 10);
    }

    void Update()
    {
        tr.position = Vector3.Lerp(tr.position, to, speed * Time.deltaTime);
    }
}