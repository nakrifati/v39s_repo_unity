using UnityEngine;
using System.Collections;

public class Kosyak : MonoBehaviour
{
    public Door door;

    void Start()
    {

    }


    void OnMouseDown()
    {
        door.OnMouseDown();
    }
}
