using UnityEngine;
using System.Collections.Generic;

public class VaultInfo : MonoBehaviour
{
    public GameObject RadZonePrefab;
    public float RadMapNodePerMeter;

    public int myId;

    float AstarTimer = 0;

    void Start()
    {
        myId = GetInstanceID();
        CreateRadMap(RadMapNodePerMeter, new Vector3(0, 0, 10), new Vector3(-40, 0, -10));
    }

    public void CreateRadMap(float perMeter, Vector3 bottomLeft, Vector3 topRight)
    {
        if (!GameObject.FindGameObjectWithTag("RadZone"))
        {
            float width = topRight.x - bottomLeft.x;
            float height = topRight.z - bottomLeft.z;
            float step = 1 / perMeter;

            for (float w = 0; w > width; w -= step)
            {
                for (float h = 0; h > height; h -= step)
                {
                    GameObject g = (GameObject)GameObject.Instantiate(RadZonePrefab.gameObject, new Vector3(bottomLeft.x + w, 1f, bottomLeft.z + h), Quaternion.identity);
                }
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject g = (GameObject)GameObject.Instantiate(RadZonePrefab.gameObject, transform.GetChild(i).position, Quaternion.identity);
            }
        }
    }

    public float GetAverageHealth()
    {
        int iter = 1; float res = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<VaultRoom>())
                res += transform.GetChild(i).GetComponent<VaultRoom>().Rp.myHealth;
            iter++;
        }
        return res / iter;
    }

    public void GiveDamage(float damage)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<VaultRoom>())
                transform.GetChild(i).GetComponent<VaultRoom>().GiveDamage(damage);
        }
    }

    public void GiveDamage(float damage, string name)
    {
        if (transform.FindChild(name).GetComponent<VaultRoom>())
            transform.FindChild(name).GetComponent<VaultRoom>().GiveDamage(damage);
    }
}
