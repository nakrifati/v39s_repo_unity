using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SaveAndLoad : MonoBehaviour
{
    GameObject g;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        objects.Clear();
    }


    public List<GameObject> objects;


    public void Save(string fileName)
    {
        objects.Clear(); UnityEngine.Object[] SaveList = FindObjectsOfType(typeof(Transform));
        foreach (UnityEngine.Object o in SaveList)
        {
            Transform t = (Transform)o; if (t.GetComponent<TypeHolder>())
            {
                t.GetComponent<TypeHolder>().AddMeToSave();
            }
        }
        Hashtable toSave = new Hashtable();
        int count = objects.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject obj = objects[i];
            TypeHolder th = obj.GetComponent(typeof(TypeHolder)) as TypeHolder;
            if (toSave.ContainsKey(obj.name)) obj.name += obj.GetInstanceID();
            toSave.Add(obj.name, new ObjectSaver(obj, th.type));
        }
        //objects.Clear();
        BinarySaver.Save(toSave, fileName);
    }

    public void Load(string fileName)
    {
        GameObject.FindGameObjectWithTag("VaultEnvironment").GetComponent<VaultEnvironment>().DeleteTrash();
        if (System.IO.File.Exists(fileName))
        {
            UnityEngine.Object[] SaveList = FindObjectsOfType(typeof(Transform));
            foreach (UnityEngine.Object o in SaveList)
            {
                Transform t = (Transform)o;
                if (t.GetComponent<TypeHolder>())
                {
                    t.GetComponent<TypeHolder>().AddMeToSave();
                }
            }

            int count = objects.Count;
            for (int i = 0; i < count; i++)
            {
                objects[i].SetActive(false);
                Destroy(objects[i]);
            }
            objects.Clear();

            Hashtable toLoad = BinarySaver.Load(fileName) as Hashtable;
            if (toLoad == null)
            {
                Debug.Log("No File Found");
                return;
            }
            ICollection coll = toLoad.Values;

            foreach (ObjectSaver obj in coll)
            {
                g = Instantiate(Resources.Load("Objects/" + obj.GetObjectType()), obj.GetPosition(), obj.GetRotation()) as GameObject;
                TypeHolder th = g.AddComponent<TypeHolder>() as TypeHolder;
                th.type = obj.GetObjectType();
                g.name = obj.objectName;

                PlayerInfo pi = g.GetComponent<PlayerInfo>();

                if (g.GetComponent<RadZone>())
                    obj.GetRadZone(g.gameObject);
                else if (pi) // Если игрок или бот
                {
                    pi.ActiveWeapon = obj.GetString("ActiveWeapon"); pi.Name = obj.GetString("Name"); pi.Gender = obj.GetString("Gender");
                    pi.Hunger = obj.GetFloat("Hunger"); pi.Health = obj.GetFloat("Health"); pi.Happy = obj.GetFloat("Happy"); pi.Speed = obj.GetFloat("Speed"); pi.safeRadLevel = obj.GetFloat("safeRadLevel");
                    pi.Age = (int)obj.GetFloat("Age");
                    pi.HungerTimer = obj.GetFloat("HungerTimer"); pi.HealthTimer = obj.GetFloat("HealthTimer"); pi.HappyTimer = obj.GetFloat("HappyTimer");
                    pi.AgeTimer = obj.GetFloat("AgeTimer");
                    g.rigidbody.mass = obj.GetFloat("Mass");
                    pi.myRadLevel = obj.GetFloat("myRadLevel");
                    g.GetComponent<PlayerController>().CameraOffset.y = obj.GetFloat("camY");
                    obj.GetInventory(g);
                }
                else if (g.GetComponent<Item>()) // Если объект инвентаря                
                    obj.GetItem(g);
                else if (g.GetComponent<VaultRoom>())
                    obj.GetVaultRoom(g.gameObject);
                else if (g.rigidbody) // Если просто объект с ригидбоди                
                    g.rigidbody.mass = obj.GetMass();
                else if (g.GetComponent<Greenhouse>())
                    obj.GetGreenhouse(g);

                Destroy(g.GetComponent<TypeHolder>());
            }
        }
        //GameObject.Find("A*").GetComponent<AstarPath>().astarData.LoadFromCache();
        // Debug
        print("Load");
        // Debug
    }


}

[System.Serializable]
public class ObjectSaver
{
    [System.Serializable]
    public class VectorSL
    {
        float X, Y, Z;

        public VectorSL(Vector3 pos)
        {
            X = pos.x;
            Y = pos.y;
            Z = pos.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(X, Y, Z);
        }
    }

    [System.Serializable]
    public class RotationSL
    {
        float Xr, Yr, Zr, Wr;
        public RotationSL(Quaternion rot)
        {
            Xr = rot.x;
            Yr = rot.y;
            Zr = rot.z;
            Wr = rot.w;
        }

        public Quaternion GetRotation()
        {
            return new Quaternion(Xr, Yr, Zr, Wr);
        }
    }

    [System.Serializable]
    public class GreenhouseSL
    {
        List<Greenhouse.Data> gda;
        Greenhouse.Data gMyData;
        public GreenhouseSL(GameObject prop)
        {
            gda = prop.GetComponent<Greenhouse>().foods;
            gMyData = prop.GetComponent<Greenhouse>().myData;
            foreach (Greenhouse.Data obj in gda)
            {
                obj.timer -= Time.timeSinceLevelLoad;
            }
            gMyData.timer -= Time.timeSinceLevelLoad;
        }

        public void GetGreenhouse(GameObject prop)
        {
            prop.GetComponent<Greenhouse>().foods = gda;
            prop.GetComponent<Greenhouse>().myData = gMyData;
        }
    }

    [System.Serializable]
    public class ObjRigSL
    {
        public float mass;
        public ObjRigSL(float m)
        {
            mass = m;
        }

        public float GetMass()
        {
            return mass;
        }
    }

    [System.Serializable]
    public class RadZoneSL
    {
        public float myMaxRad, myRad;
        public bool stable = false;
        public float SetRadTimer;

        public RadZoneSL(GameObject Prop)
        {
            RadZone rz = Prop.GetComponent<RadZone>();
            myMaxRad = rz.myMaxRad;
            myRad = rz.myRad;
            stable = rz.stable;
            SetRadTimer = rz.SetRadTimer;
        }

        public void GetRadZone(GameObject Prop)
        {
            RadZone rz = Prop.GetComponent<RadZone>();
            rz.myMaxRad = myMaxRad;
            rz.myRad = myRad;
            rz.stable = stable;
            rz.SetRadTimer = SetRadTimer; ;
        }
    }

    [System.Serializable]
    public class VaultRoomSL
    {
        VaultRoom.VaultRoomProp Rp = new VaultRoom.VaultRoomProp();

        public VaultRoomSL(GameObject Prop)
        {
            Rp = Prop.GetComponent<VaultRoom>().Rp;
        }

        public void GetVaultRoom(GameObject Prop)
        {
            Prop.GetComponent<VaultRoom>().Rp = Rp;
        }
    }

    [System.Serializable]
    public class ItemSL
    {
        Item.ItemProp ip;
        public ItemSL(GameObject Prop)
        {
            ip = Prop.GetComponent<Item>().Ip;
        }

        public void GetItem(GameObject Prop)
        {
            Prop.GetComponent<Item>().Ip = ip;
        }
    }

    [System.Serializable]
    public class PlayerInfoSL
    {
        public List<PlayerInfo.MyStack> Inventory = new List<PlayerInfo.MyStack>();
        public string ActiveWeapon, Name, Gender;
        public float Hunger, Health, Happy, Speed, safeRadLevel;
        public int Age;
        public float HungerTimer, HealthTimer, HappyTimer, AgeTimer, secTimer;
        public float mass;
        public float camY;
        public float myRadLevel;
        public Item.ItemProp it;

        public PlayerInfoSL(GameObject Prop)
        {
            Inventory.Clear();

            PlayerInfo Pi = Prop.GetComponent<PlayerInfo>();

            ActiveWeapon = Pi.ActiveWeapon; Name = Pi.Name; Gender = Pi.Gender;
            Hunger = Pi.Hunger; Health = Pi.Health; Happy = Pi.Happy; Speed = Pi.Speed; safeRadLevel = Pi.safeRadLevel;
            Age = Pi.Age;
            HungerTimer = Pi.HungerTimer - Time.timeSinceLevelLoad; HealthTimer = Pi.HealthTimer - Time.timeSinceLevelLoad; HappyTimer = Pi.HappyTimer - Time.timeSinceLevelLoad; secTimer = Pi.secTimer - Time.timeSinceLevelLoad;
            AgeTimer = Pi.AgeTimer - Time.timeSinceLevelLoad;
            mass = Pi.rigidbody.mass;
            camY = Prop.GetComponent<PlayerController>().CameraOffset.y;
            it = Pi.activeSuit;
            Inventory = Pi.Stacks;
            myRadLevel = Pi.myRadLevel;
            // Debug
            Debug.Log(Inventory.Count);
            Debug.Log(Inventory[0].MyItems.Count);
            // Debug

        }

        public string GetString(string arg)
        {
            switch (arg)
            {
                case "ActiveWeapon":
                    return ActiveWeapon;
                case "Name":
                    return Name;
                case "Gender":
                    return Gender;
            }
            return null;
        }

        public float GetFloat(string arg)
        {
            switch (arg)
            {
                case "Mass":
                    return mass;
                case "Hunger":
                    return Hunger;
                case "Health":
                    return Health;
                case "Happy":
                    return Happy;
                case "Speed":
                    return Speed;
                case "safeRadLevel":
                    return safeRadLevel;
                case "Age":
                    return Age;
                case "HungerTimer":
                    return HungerTimer;
                case "HealthTimer":
                    return HealthTimer;
                case "HappyTimer":
                    return HappyTimer;
                case "AgeTimer":
                    return AgeTimer;
                case "camY":
                    return camY;
                case "myRadLevel":
                    return myRadLevel;
            }
            return 0; // =)
        }

        public void GetInventory(GameObject receiver)
        {
            receiver.GetComponent<PlayerInfo>().Stacks = Inventory;
        }
    }


    //===========================================SAVE AND LOAD======================================
    string objectType;
    VectorSL position;
    RotationSL rotation;
    GreenhouseSL green;
    RadZoneSL rz;
    VaultRoomSL vr;
    ItemSL item;
    PlayerInfoSL player;
    public float mass;
    public string objectName;

    public ObjectSaver(GameObject obj, string objectType)
    {
        this.objectType = objectType;
        position = new VectorSL(obj.transform.position);
        rotation = new RotationSL(obj.transform.rotation);
        if (obj.GetComponent<RadZone>())
            rz = new RadZoneSL(obj.gameObject);
        else if (obj.GetComponent<Item>())
            item = new ItemSL(obj.gameObject);
        else if (obj.GetComponent<VaultRoom>())
            vr = new VaultRoomSL(obj.gameObject);
        else if (obj.GetComponent<Greenhouse>())
            green = new GreenhouseSL(obj.gameObject);
        else if (obj.GetComponent<PlayerInfo>())
            player = new PlayerInfoSL(obj.gameObject);
        else if (obj.rigidbody)
            mass = obj.rigidbody.mass;
        objectName = obj.name;
    }

    public string GetObjectType()
    {
        return objectType;
    }
    public Vector3 GetPosition()
    {
        return position.GetPosition();
    }
    public Quaternion GetRotation()
    {
        return rotation.GetRotation();
    }
    public float GetMass()
    {
        return mass;
    }
    public string GetString(string arg)
    {
        return player.GetString(arg);
    }
    public float GetFloat(string arg)
    {
        return player.GetFloat(arg);
    }
    public void GetInventory(GameObject receiver)
    {
        player.GetInventory(receiver);
    }
    public void GetItem(GameObject receiver)
    {
        item.GetItem(receiver);
    }
    public void GetVaultRoom(GameObject receiver)
    {
        vr.GetVaultRoom(receiver);
    }
    public void GetRadZone(GameObject receiver)
    {
        rz.GetRadZone(receiver);
    }
    public void GetGreenhouse(GameObject receiver)
    {
        green.GetGreenhouse(receiver);
    }
}