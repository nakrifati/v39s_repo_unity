using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
    public bool Opened = false;
    public float offset;
    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        //CloseTimer();
    }

    void Update()
    {
        if (Opened)
            Open();
        else
            Close();
    }

    public void Open()
    {
        transform.position = Vector3.Lerp(transform.position, startPos + transform.right * offset, 2 * Time.deltaTime);
        Opened = true;
    }

    public void Close()
    {
        transform.position = Vector3.Lerp(transform.position, startPos, 2 * Time.deltaTime);
        Opened = false;
    }

    //void CloseTimer()
    //{
    //    Close();
    //    Invoke("CloseTimer", 3);
    //}

    void OnMouseDown()
    {
        Opened = !Opened;
    }

    //void OnCollisionStay(Collision coll)
    //{
    //    if (Opened && coll.gameObject.layer != 13)
    //    {
    //        if (coll.gameObject.GetComponent<Item>() && coll.gameObject.GetComponent<Item>().Ip.eat)
    //            Destroy(coll.gameObject);
    //        else
    //            Opened = true;
    //    }
    //}
}
