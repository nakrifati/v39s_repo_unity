using UnityEngine;
using System.Collections;

public class Suit : MonoBehaviour
{
    public float regenTimer;
    public Item it;

    void Awake()
    {
        it = GetComponent<Item>();
    }
    void Start()
    {
        regenTimer = Time.timeSinceLevelLoad + 1;
    }

    void Update()
    {
        if (it.Ip.health < 600 && regenTimer <= Time.timeSinceLevelLoad)
        {
            it.Ip.health++;
            regenTimer = Time.timeSinceLevelLoad + 1;
        }
    }
}
