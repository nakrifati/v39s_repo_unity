using UnityEngine;
using System.Collections;

public class PlayerRadZone : MonoBehaviour
{
    public Transform myPlayer;
    public float offset;

    void Start()
    {

    }

    void Update()
    {
        transform.position = myPlayer.position + Vector3.up * offset;
    }
}
