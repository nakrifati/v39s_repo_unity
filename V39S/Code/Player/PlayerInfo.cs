using UnityEngine;
using System.Collections.Generic;

public class PlayerInfo : MonoBehaviour
{
    public string ActiveWeapon, Name, Gender, modelName;
    public float Hunger, Health, Happy, Speed, safeRadLevel, myRadLevel;
    public int Age;
    public float HungerTimer, HealthTimer, HappyTimer, AgeTimer, secTimer;
    public bool showSuit, suitEffect;
    public GameObject suitPrefab, myRad;
    public Item.ItemProp activeSuit;

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
    }

    void FixedUpdate()
    {
        if (secTimer <= Time.timeSinceLevelLoad)
        {
            foreach (Item.ItemProp obj in FindItems("Suit"))
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
    public void AddItem(Item It, int stkNum, MyStack obj)
    {
        Stacks[stkNum].MyItems.Add(It.Ip);
    }

    // Удалить из инвентаря
    public void DeleteItem(int stkNum, Item.ItemProp it)
    {
        Stacks[stkNum].MyItems.Remove(it);
    }

    public void DeleteItem(Item.ItemProp it)
    {
        foreach (MyStack obj in Stacks)
        {
            foreach (Item.ItemProp ip in obj.MyItems)
            {
                if (ip == it)
                {
                    obj.MyItems.Remove(ip);
                    return;
                }
            }
        }
    }

    public List<Item.ItemProp> FindItems(string name)
    {
        List<Item.ItemProp> itpr = new List<Item.ItemProp>();

        foreach (MyStack obj in Stacks)
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


    public Item.ItemProp FindItem(string name)
    {
        foreach (MyStack obj in Stacks)
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

    public Item.ItemProp FindItem(int stkNum, string name)
    {
        bool flag = true;
        foreach (MyStack obj in Stacks)
        {
            if (Stacks.IndexOf(obj) == stkNum)
            {
                foreach (Item.ItemProp it in obj.MyItems)
                {
                    if (it.name == name)
                    {
                        return it;
                    }
                }
            }
            if (!flag)
                break;
        }
        return null;
    }

    public Item.ItemProp FindItem(int stkNum, uint id)
    {
        bool flag = true;
        foreach (MyStack obj in Stacks)
        {
            if (Stacks.IndexOf(obj) == stkNum)
            {
                foreach (Item.ItemProp it in obj.MyItems)
                {
                    if (it.id == id)
                    {
                        return it;
                    }
                }
            }
            if (!flag)
                break;
        }
        return null;
    }

    public Item.ItemProp FindLastItem(int stkNum, string name) // Dangerous o_O
    {
        if (Stacks[stkNum].MyItems.Count > 0)
            return Stacks[stkNum].MyItems[Stacks[stkNum].MyItems.Count - 1];
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
            GameObject clone = (GameObject)Instantiate(suitPrefab.gameObject, transform.position, transform.rotation);
            clone.name = "Suit";
            clone.transform.parent = transform;

            activeSuit = it;
            it.active = true;
            showSuit = true;

            transform.FindChild("Hostage").gameObject.SetActive(false);
            transform.FindChild("Suit").gameObject.SetActive(true);
        }
    }

    public void RemoveSuit()
    {
        if (showSuit)
        {
            Transform g = transform.FindChild("Suit");
            if (g == null)
                return;

            Destroy(g.gameObject);
            showSuit = false;
            activeSuit.active = false;
            activeSuit = null;

            transform.FindChild("Suit").gameObject.SetActive(false);
            transform.FindChild("Hostage").gameObject.SetActive(true);
        }
    }

    public void Recovery(Item.ItemProp it)
    {
        if (FindItems("Cement").Count == 0 || GetMyRoom() == null)
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
            DeleteItem(it);
        DeleteItem(FindItems("Cement")[0]);
        pg.UpdateStacksIcon();
    }
}