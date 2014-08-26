using UnityEngine;
using System.Collections.Generic;

public class PlayerInfo : MonoBehaviour
{
    public string ActiveWeapon, Name, Gender, modelName;
    public float Hunger, newHunger, Health, newHealth, Happy, Speed, safeRadLevel, myRadLevel;
    public int Age;
    public float HungerTimer, HealthTimer, HappyTimer, AgeTimer, secTimer;
    public bool showSuit, suitEffect, regenHealth, regenHunger;
    public GameObject suitPrefab, GlaskG7, myRad;
    public Item.ItemProp activeSuit, activeWeapon;
    public Vector3 weapOffset;

    public PlayerGUI pg;

    public List<MyStack> Stacks = new List<MyStack>(); // o_O

    [System.Serializable]
    public class MyStack
    {
        public string name;
        public List<Item.ItemProp> MyItems = new List<Item.ItemProp>();

        public string GetName()
        {
            if (MyItems.Count > 0)
                name = MyItems[0].name;
            else
                name = "";
            return name;
        }
    }

    void Start()
    {
        AstarScan();
        pg = GetComponent<PlayerGUI>();
        HungerTimer = Time.timeSinceLevelLoad;

        newHealth = Health; newHunger = Hunger;

        // Создание ячеек инвентаря
        if (Stacks.Count == 0)
            for (int i = 0; i < 10; i++)
            {
                MyStack S = new MyStack();
                Stacks.Add(S);
            }

        myRad = (GameObject)Instantiate(Resources.Load("Objects/PlayerRadZone"), transform.position + Vector3.up, Quaternion.identity);
        myRad.GetComponent<PlayerRadZone>().myPlayer = transform;
        myRad.GetComponent<RadZone>().myMaxRad = myRadLevel;
        myRad.name = name + " Rad";

        AstarScan();
        Invoke("AstarScanNow", 1);

        foreach (MyStack obj in Stacks)
        {
            if (obj.GetName() != "")
                if (obj.MyItems[0].weapon && obj.MyItems[0].active)
                {
                    SetActiveWeapon(obj.MyItems[0]);
                    break;
                }
        }

        if (activeSuit.name != "")
        {
            DressSuit(activeSuit);
        }
    }

    void Update()
    {
        if (secTimer <= Time.timeSinceLevelLoad)
        {
            //if (regenHealth)
            Health = Mathf.MoveTowards(Health, newHealth, 50 * Time.deltaTime);
            // if (regenHunger)
            Hunger = Mathf.MoveTowards(Hunger, newHunger, 50 * Time.deltaTime);

            //if (Health >= newHealth)
            //    regenHealth = false;
            //if (Hunger >= newHunger)
            //    regenHunger = false;

            foreach (Item.ItemProp obj in FindItems("Suit", Stacks))
            {
                if (!obj.active && obj.health < 600)
                    obj.health++;
            }

            if (activeSuit != null && activeSuit.active)
            {
                activeSuit.health--;
                if (activeSuit.health <= 0)
                    activeSuit.active = false;
                safeRadLevel = 0.05f;
            }
            else safeRadLevel = 1;

            if (HealthTimer <= Time.timeSinceLevelLoad)
            {
                Health -= GetAverageRad() * safeRadLevel;
                if (Health <= 0)
                {
                    Application.LoadLevel("MainMenu");
                }
                HealthTimer = Time.timeSinceLevelLoad + 1;
            }

            if (Health <= 0)
            {
                Application.LoadLevel("MainMenu");
            }

            if (HungerTimer <= Time.timeSinceLevelLoad)
            {
                Hunger -= 0.3f;
                if (Hunger <= 0)
                {
                    Hunger = 0;
                    Health -= 5;
                }
                HungerTimer = Time.timeSinceLevelLoad + 1;
            }

            if (HappyTimer <= Time.timeSinceLevelLoad)
            {
                Happy -= 0.01f;
                if (Happy <= 0)
                {
                    Happy = 0;
                }
                HappyTimer = Time.timeSinceLevelLoad + 1;
            }

            if (AgeTimer <= Time.timeSinceLevelLoad)
            {
                Age++;
                AgeTimer = Time.timeSinceLevelLoad + 2160;
            }

            myRad.GetComponent<RadZone>().FindAndSet();
            myRadLevel = myRad.GetComponent<RadZone>().myMaxRad;



            secTimer = Time.timeSinceLevelLoad + 1;
        }
    }

    public void AstarScan()
    {
        GameObject.Find("A*").GetComponent<AstarPath>().Scan();
        Invoke("AstarScan", 5);
    }

    public void AstarScanNow()
    {
        GameObject.Find("A*").GetComponent<AstarPath>().Scan();
    }

    // Добавить в инвентарь
    public void AddItem(Item It, int stkNum, List<MyStack> targList)
    {
        targList[stkNum].MyItems.Add(It.Ip);
    }

    // Удалить из инвентаря
    public void DeleteItem(int stkNum, Item.ItemProp it, List<MyStack> targList)
    {
        targList[stkNum].MyItems.Remove(it);
    }

    // Удалить из инвентаря
    public void DeleteItem(Item.ItemProp it, List<MyStack> targList)
    {
        for (int i = 0; i < targList.Count; i++)
        {
            if (targList[i].MyItems.Count > 0)
            {
                if (targList[i].MyItems[0] == it)
                {
                    targList[i].MyItems.Remove(it);
                    return;
                }
            }
        }
    }

    public List<Item.ItemProp> FindItems(string name, List<MyStack> targList)
    {
        List<Item.ItemProp> itpr = new List<Item.ItemProp>();

        foreach (MyStack obj in targList)
        {
            foreach (Item.ItemProp it in obj.MyItems)
            {
                if (it.name == name)
                {
                    itpr.Add(it);
                }
            }
        }
        return itpr;
    }


    public Item.ItemProp FindItem(string name, List<MyStack> targList)
    {
        foreach (MyStack obj in targList)
        {
            foreach (Item.ItemProp it in obj.MyItems)
            {
                if (it.name == name)
                {
                    return it;
                }
            }
        }
        return new Item.ItemProp("", 0, false);
    }

    public Item.ItemProp FindItem(int stkNum, string name, List<MyStack> targList)
    {
        foreach (Item.ItemProp it in targList[stkNum].MyItems)
        {
            if (it.name == name)
            {
                return it;
            }
        }
        return null;
    }

    public Item.ItemProp FindItem(int stkNum, uint id, List<MyStack> targList)
    {
        foreach (Item.ItemProp it in targList[stkNum].MyItems)
        {
            if (it.id == id)
            {
                return it;
            }
        }
        return null;
    }

    public Item.ItemProp FindLastItem(int stkNum, string name, List<MyStack> targList) // Dangerous o_O
    {
        if (targList[stkNum].MyItems.Count > 0)
            return targList[stkNum].MyItems[targList[stkNum].MyItems.Count - 1];
        return new Item.ItemProp("", 0, false);
    }

    public float GetAverageRad()
    {
        int iter = 1; float res = 0;

        foreach (Collider obj in Physics.OverlapSphere(transform.position, 2.5f))
            if (collider != obj.collider && obj.GetComponent<RadZone>())
            {
                if (Physics.Raycast(transform.position, obj.transform.position - transform.position, 4))
                {
                    res += obj.GetComponent<RadZone>().myRad;
                    iter++;
                }
            }

        return res / iter;
    }

    public string GetMyRoom()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), -transform.up, out hit, 5))
            if (hit.transform.GetComponent<VaultRoom>())
                return hit.transform.name;

        return null;
    }

    public void DressSuit(Item.ItemProp it)
    {
        if (!showSuit)
        {
            Transform w = GameObject.Find("WeapObj").transform.FindChild(activeWeapon.name);
            if (w)
                w.parent = transform;

            Destroy(transform.FindChild("Hostage").gameObject);

            GameObject clone = (GameObject)Instantiate(suitPrefab.gameObject, transform.position, transform.rotation);
            clone.name = "Suit";
            clone.transform.parent = transform;

            activeSuit = it;
            it.active = true;
            showSuit = true;

            if (transform.FindChild(activeWeapon.name))
                Invoke("WeapParent", 0.05f);
        }
    }

    public void WeapParent()
    {
        transform.FindChild(activeWeapon.name).position = GameObject.Find("WeapObj").transform.position;
        transform.FindChild(activeWeapon.name).rotation = GameObject.Find("WeapObj").transform.rotation;
        transform.FindChild(activeWeapon.name).parent = GameObject.Find("WeapObj").transform;
    }

    public void RemoveSuit()
    {
        if (showSuit)
        {
            Transform w = GameObject.Find("WeapObj").transform.FindChild(activeWeapon.name);
            if (w)
                w.parent = transform;

            Transform g = transform.FindChild("Suit");
            if (g == null)
                return;

            Destroy(g.gameObject);
            showSuit = false;
            activeSuit.active = false;
            activeSuit = null;

            GameObject clone = (GameObject)Instantiate(Resources.Load("Objects/Hostage"), transform.position, transform.rotation);
            clone.name = "Hostage";
            clone.transform.Rotate(Vector3.right, -90, Space.Self);
            clone.transform.parent = transform;

            if (transform.FindChild(activeWeapon.name))
                Invoke("WeapParent", 0.05f);
        }
    }

    public void Shoot(Vector3 target)
    {
        GameObject.Find("WeapObj").transform.FindChild(activeWeapon.name).GetComponent<Weapon>().Shoot(target);
    }

    public void SetActiveWeapon(Item.ItemProp it)
    {
        GameObject clone = (GameObject)Instantiate(Resources.Load("Objects/" + it.name + "_model"), GameObject.Find("WeapObj").transform.position, GameObject.Find("WeapObj").transform.rotation);
        it.active = true;
        clone.name = it.name;
        clone.transform.parent = GameObject.Find("WeapObj").transform;
        //clone.transform.localPosition = weapOffset;
        activeWeapon = new Item.ItemProp(it);//it;
        clone.GetComponent<Item>().Ip = it;
    }

    public void DisableAllWeapons()
    {
        foreach (MyStack obj in Stacks)
        {
            if (obj.MyItems.Count > 0)
                if (obj.MyItems[0].weapon)
                    obj.MyItems[0].active = false;
        }
        if (activeWeapon.name != "")
        {
            if (GameObject.Find("WeapObj").transform.FindChild(activeWeapon.name))
            {
                Destroy(GameObject.Find("WeapObj").transform.FindChild(activeWeapon.name).gameObject);
            }
        }
        activeWeapon = new Item.ItemProp("", 0, false);
    }

    public void Recovery(Item.ItemProp it)
    {
        if (FindItems("Cement", Stacks).Count == 0 || GetMyRoom() == null)
            return;

        GameObject vi = GameObject.Find("Vault");

        VaultRoom.VaultRoomProp vr = vi.transform.FindChild(GetMyRoom()).GetComponent<VaultRoom>().Rp;

        if (vr.myHealth == 100)
            return;

        if (it.name == "RecoverySet1")
        {
            vr.myHealth += 30;
            if (vr.myHealth > 100)
                vr.myHealth = 100;
        }
        else if (it.name == "RecoverySet2")
        {
            vr.myHealth += 70;
            if (vr.myHealth > 100)
                vr.myHealth = 100;
        }

        it.health -= 5;
        if (it.health <= 0)
            DeleteItem(it, Stacks);
        DeleteItem(FindItems("Cement", Stacks)[0], Stacks);
        pg.UpdateStacksIcon();
    }
}