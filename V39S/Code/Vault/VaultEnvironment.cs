using UnityEngine;
using System.Collections.Generic;

public class VaultEnvironment : MonoBehaviour
{
    public GameObject Player;
    public float EarthqTimer, Damage;
    protected VaultInfo Vi;
    protected PlayerController Pc;
    bool RandDamage = true;
    float saveTimer;

    void Start()
    {
        saveTimer = Time.time + 120;
        EarthqTimer = Time.time + Random.Range(0, 900);
        //EarthqTimer = Time.time + Random.Range(1, 2);
        Vi = GameObject.Find("Vault").GetComponent<VaultInfo>();
        Pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (GameObject.Find("ToLoad"))
        {
            GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().Load("Save.save");
            Destroy(GameObject.Find("ToLoad"));
        }
        //GameObject.Find("A*").GetComponent<AstarPath>().astarData.LoadFromCache();
    }

    void FixedUpdate()
    {
        if (saveTimer < Time.timeSinceLevelLoad)
        {
            GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().Save("Save.save");
            saveTimer = Time.timeSinceLevelLoad + 900;
        }

        if (EarthqTimer >= Time.timeSinceLevelLoad - 10 && EarthqTimer <= Time.timeSinceLevelLoad)
        {
            if (RandDamage)
            {
                Damage = Random.Range(0, 0.025f);
                RandDamage = false;
            }

            DoEarthquake();
        }
        else if (EarthqTimer < Time.timeSinceLevelLoad - 10)
        {
            //EarthqTimer = Time.time + Random.Range(1, 2);
            EarthqTimer = Time.timeSinceLevelLoad + Random.Range(1900, 2200);
            RandDamage = true;
        }

        if (Input.GetKey(KeyCode.E))
            DoEarthquake();
    }

    public void DoEarthquake()
    {
        Pc.MyCamera.transform.position += Random.insideUnitSphere * 0.1f;
        Vi.GiveDamage(Damage);
    }

    public void DeleteTrash()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Trash"))
        {
            Destroy(obj);
        }
    }
}
