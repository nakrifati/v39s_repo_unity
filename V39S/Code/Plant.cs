using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour
{
    public Item.ItemProp Pr;
    public Vector3 startScale;
    RaycastHit hit;

    void Awake()
    {
        Pr = GetComponent<Item>().Ip;
        startScale = transform.localScale;
        collider.enabled = false;
    }

    void Start()
    {
        Grow();
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f))
        {
            Greenhouse gr = hit.transform.GetComponent<Greenhouse>();
            if (gr)
            {
                gr.myData.timer = Time.timeSinceLevelLoad + 2;
            }
            else
                RemoveMe();
        }
        else
            RemoveMe();
    }

    void Grow()
    {
        if (Time.timeScale > 0)
        {
            if (Pr.health < 100)
            {
                Pr.health += 0.1f;
                transform.localScale = startScale * Pr.health * 0.01f;
                collider.enabled = false;
                rigidbody.drag = 100;
                rigidbody.angularDrag = 100f;
            }
            else
                collider.enabled = true;

            Pr.rDrag = rigidbody.drag;
            Pr.rAngularDrag = rigidbody.angularDrag;
        }
        Invoke("Grow", 0.1f);
    }

    public void RemoveMe()
    {
        collider.enabled = true;
        rigidbody.drag = 0;
        rigidbody.angularDrag = 0.05f;
        if (transform.FindChild("Leaves"))
            Destroy(transform.FindChild("Leaves").gameObject);
        Destroy(this);
    }
}
