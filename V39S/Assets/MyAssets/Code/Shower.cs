using UnityEngine;
using System.Collections;

public class Shower : MonoBehaviour
{
    public bool canFind;
    void Start()
    {
        FindPlayer();
    }

    void Update()
    {

    }

    public void Wash()
    {
        transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = true;
    }

    public void EndWash()
    {
        transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = false;
    }

    public void FindPlayer()
    {
        foreach (Collider coll in Physics.OverlapSphere(transform.position, 4))
        {
            RaycastHit hit, hit1;
            if (Physics.Raycast(transform.position, coll.transform.position - transform.position, out hit, 3))
            {
                if (hit.transform.GetComponent<PlayerInfo>())
                {
                    Wash();
                    hit.transform.GetComponent<PlayerInfo>().myRad.GetComponent<RadZone>().myMaxRad = 0;
                    hit.transform.GetComponent<PlayerInfo>().myRad.GetComponent<RadZone>().myRad = 0;

                    foreach (Collider obj in Physics.OverlapSphere(transform.position, 4))
                    {
                        if (Physics.Raycast(transform.position, obj.transform.position - transform.position, out hit1, 3))
                        {
                            if (hit1.transform.GetComponent<RadZone>())
                            {
                                hit1.transform.GetComponent<RadZone>().myMaxRad = 0;
                                hit1.transform.GetComponent<RadZone>().myRad = 0;
                            }
                        }
                    }

                    break;
                }
            }
            else
                EndWash();
        }

        if (canFind)
            Invoke("FindPlayer", 1);
    }
}
