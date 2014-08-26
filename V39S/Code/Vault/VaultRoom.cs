using UnityEngine;
using System.Collections;

public class VaultRoom : MonoBehaviour
{
    [System.Serializable]
    public class VaultRoomProp
    {
        public float myHealth;
        public float myRadLevel;
        public float myHealthTimer, myRadTimer;
        public bool createRad = true;

        public VaultRoomProp()
        {

        }
    }

    public GameObject myRadZone;

    public VaultRoomProp Rp = new VaultRoomProp();

    public Material newMat;

    public float lerpSpeed;

    void Awake()
    {
        transform.parent = GameObject.FindGameObjectWithTag("Vault").transform;
    }

    void Start()
    {
        Rp.myHealthTimer = Time.timeSinceLevelLoad + 360;
        UpdateVaultRoomMat();
    }

    void Update()
    {
        if (Rp.myHealthTimer <= Time.timeSinceLevelLoad)
        {
            GiveDamage(1);
            Rp.myHealthTimer = Time.timeSinceLevelLoad + 360;
        }

        if (Rp.myHealth > 75)
        {
            if (myRadZone)
            {
                //GameObject.Find("Vault").GetComponent<VaultInfo>().RadMap.Remove(myRadZone.GetComponent<RadZone>());
                Destroy(myRadZone.gameObject);
                myRadZone = null;
            }

            if (GameObject.Find("RadZone" + name))
            {
                Destroy(GameObject.Find("RadZone" + name).gameObject);
            }
        }

        //renderer.material.Lerp(renderer.material, newMat, lerpSpeed);
        //renderer.material = newMat;

    }

    public void GiveDamage(float damage = 0)
    {
        Rp.myHealth -= damage;
        if (Rp.myHealth < 1)
            Rp.myHealth = 1;

        if (Rp.myHealth <= 75)
            if (Rp.createRad)
                CreateRadZone();

        float a = Mathf.Cos(3.14f * Rp.myHealth * 0.01f);
        float b = Mathf.Cos(3.14f * Rp.myHealth * 0.01f + 4.71f);
        float c = Mathf.Cos(3.14f * Rp.myHealth * 0.01f + 3.14f);

        if (Rp.myHealth * 0.01f > 0.5f)
            a = 0.0f;
        if (Rp.myHealth * 0.01f < 0.5f)
            c = 0.0f;

        renderer.material.SetFloat("_FactorA", a);
        renderer.material.SetFloat("_FactorB", b);
        renderer.material.SetFloat("_FactorC", c);
    }

    public void CreateRadZone()
    {
        if (!myRadZone)
        {
            myRadZone = Instantiate(transform.root.GetComponent<VaultInfo>().RadZonePrefab.gameObject, transform.position, Quaternion.identity) as GameObject;
            myRadZone.name = "RadZone" + name;
            myRadZone.gameObject.tag = "Untagged";
            myRadZone.GetComponent<RadZone>().stable = true;
            //transform.root.GetComponent<VaultInfo>().RadMap.Add(myRadZone.GetComponent<RadZone>());
        }
        myRadZone.GetComponent<RadZone>().myMaxRad = -0.05f * Rp.myHealth + 5;
    }

    public void UpdateVaultRoomMat()
    {
        GiveDamage();
        Invoke("UpdateVaultRoomMat", 1);
    }
}
