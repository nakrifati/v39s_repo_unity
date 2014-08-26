using UnityEngine;
using System.Collections.Generic;

public class RadZone : MonoBehaviour
{
    public float myMaxRad, myRad;
    public List<Collider> MyConnectors = new List<Collider>();
    public bool stable = false;
    public float SetRadTimer;

    void Start()
    {
        //SetRadTimer = Random.Range(Time.timeSinceLevelLoad, Time.timeSinceLevelLoad + 5);
        if (!InVault() && !stable)
        {
            Destroy(gameObject);
        }
        Invoke("FindConnectors", 2);
    }

    void Update()
    {
        if (SetRadTimer < Time.timeSinceLevelLoad)
        {
            SetRad();
            SetRadTimer = Time.timeSinceLevelLoad + 3;
        }

        if (!stable)
        {
            if (myRad != myMaxRad)
            {
                myRad = Mathf.MoveTowards(myRad, myMaxRad, 10f * Time.deltaTime);
                SetColor();
            }
            myMaxRad = Mathf.MoveTowards(myMaxRad, 0, 0.1f * Time.deltaTime);
        }
    }

    public bool InVault()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10.0f))
            return true;
        //if (hit.transform.GetComponent<VaultRoom>())
        //    return true;


        return false;
    }

    public void SetRad()
    {
        RaycastHit hit;

        foreach (Collider obj in MyConnectors)
            if (Physics.Raycast(transform.position, (obj.transform.position - transform.position), out hit, 5.0f))
            {
                RadZone rz = hit.transform.GetComponent<RadZone>();
                if (rz && rz.myMaxRad < myMaxRad)
                    rz.myMaxRad = myMaxRad - 0.06f * (hit.transform.position - transform.position).magnitude;
            }
    }

    public void SetColor()
    {
        renderer.material.color = new Color(myRad, 0, 0);
    }

    public void FindConnectors()
    {
        Vector3 trPos = transform.position;
        MyConnectors.Clear();
        foreach (Collider o in Physics.OverlapSphere(trPos, 1.5f * (1 / GameObject.FindGameObjectWithTag("Vault").GetComponent<VaultInfo>().RadMapNodePerMeter)))
        {
            if (o.GetComponent<RadZone>() && o.collider != collider)
            {
                MyConnectors.Add(o);
            }
        }
    }

    public void ReCheckInArea()
    {
        Vector3 trPos = transform.position;
        foreach (Collider o in Physics.OverlapSphere(trPos, 4 * (1 / GameObject.FindGameObjectWithTag("Vault").GetComponent<VaultInfo>().RadMapNodePerMeter)))
        {
            if (o.GetComponent<RadZone>() && o.collider != collider)
            {
                o.GetComponent<RadZone>().MyConnectors.Add(collider);
                o.GetComponent<RadZone>().SetRad();
            }
        }
    }

    public void RemoveMeInArea()
    {
        Vector3 trPos = transform.position;
        foreach (Collider o in Physics.OverlapSphere(trPos, 5 * (1 / GameObject.FindGameObjectWithTag("Vault").GetComponent<VaultInfo>().RadMapNodePerMeter)))
        {
            if (o.GetComponent<RadZone>() && o.collider != collider)
            {
                if (o.GetComponent<RadZone>().MyConnectors.Contains(collider))
                    o.GetComponent<RadZone>().MyConnectors.Remove(collider);
            }
        }
    }

    public void FindAndSet()
    {
        ReCheckInArea(); // Добавить меня к ним
        FindConnectors(); // Добавить их ко мне
        SetRad();
        RemoveMeInArea(); // Удалить меня из них
        MyConnectors.Clear();
    }
}
