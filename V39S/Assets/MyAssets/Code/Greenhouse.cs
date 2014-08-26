using UnityEngine;
using System.Collections.Generic;

public class Greenhouse : MonoBehaviour
{
    public int foodPerPush;

    [System.Serializable]
    public class Data
    {
        public Item.ItemProp food;
        public float timer;//+3600;
        //public int fPP;

        public Data(string name)
        {
            food = new Item.ItemProp(name, 0, true);
        }
    }

    public Data myData;
    public List<Data> foods = new List<Data>();
    public string playerName, foodName;
    public int playerCurrStack;
    public bool canSearch, canGrow;
    float searchTimer, updateTimer;


    void Start()
    {
        //foodPerPush = myData.fPP;
        searchTimer = Time.time + 2;
        updateTimer = Time.time + 2;
    }

    void Update()
    {
        //foodPerPush = myData.fPP;
        if (canSearch && searchTimer <= Time.time)
        {
            SearchPlayer();
            searchTimer = Time.time + 0.5f;
        }
        if (updateTimer <= Time.time)
        {
            UpdateData();
            updateTimer = Time.time + 0.5f;
        }
        if (myData.timer <= Time.timeSinceLevelLoad)
        {
            canGrow = true;
        }
    }

    public void AddFood(string name)
    {
        Data f = new Data(name);
        foods.Add(f);
    }

    public void DropFood(Data d)
    {
        GameObject g = null;
        for (int i = 0; i < foodPerPush; i++)
        {
            g = (GameObject)Instantiate(Resources.Load("Objects/" + d.food.name), transform.position + transform.right * Random.Range(-0.2f, 0.2f) + transform.forward * Random.Range(-0.7f, 0.7f), Quaternion.identity);
            g.AddComponent<Plant>();
            g.GetComponent<Item>().Ip.listable = true;
            g.GetComponent<Item>().Ip.health = d.food.health;
            g.rigidbody.drag = 100;  // O_o
            g.rigidbody.angularDrag = 100;
        }
    }

    public void UpdateData()
    {
        float currTime = Time.timeSinceLevelLoad;
        List<Data> trash = new List<Data>();
        foreach (Data obj in foods)
        {
            if (obj.timer <= currTime)
            {
                DropFood(obj);
                trash.Add(obj);
            }
        }
        foreach (Data obj in trash)
        {
            foods.Remove(obj);
        }
    }

    public void SearchPlayer()
    {
        if (canSearch && canGrow)
        {
            foreach (Collider coll in Physics.OverlapSphere(transform.position, 1))
            {
                if (coll.transform.root.GetComponent<PlayerInfo>() && coll.transform.root.GetComponent<PlayerInfo>().Name == playerName)
                {
                    AddFood(foodName);
                    PlayerInfo pi = coll.GetComponent<PlayerInfo>();
                    pi.DeleteItem(playerCurrStack, pi.FindLastItem(playerCurrStack, foodName, pi.Stacks), pi.Stacks);
                    pi.GetComponent<PlayerGUI>().UpdateStacksIcon();
                    pi.GetComponent<PlayerController>().hp = pi.transform.position;
                    UpdateData();
                    myData.timer = Time.timeSinceLevelLoad + 5; // O_o
                    playerName = null;
                    canSearch = false;
                    canGrow = false;
                    break;
                }
            }
        }
    }
}
