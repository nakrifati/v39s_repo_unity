using UnityEngine;
using System.Collections;


public class Item : MonoBehaviour
{
    [System.Serializable]
    public class ItemProp
    {
        public string name;
        public uint id;
        public float health, rDrag, rAngularDrag, magazin;
        public bool pack, userful, eat, plant, suit, recovery, spares, active, listable, iznosable, weapon; // iznosable *_*

        public ItemProp(string _name, float _health, bool _pack)
        {
            name = _name;
            id = (uint)Random.Range(0, 4294967290);
            health = _health;
            pack = _pack;
        }

        public ItemProp(ItemProp obj)
        {
            name = obj.name;
            id = obj.id;
            health = obj.health;
            rDrag = obj.rDrag;
            rAngularDrag = obj.rAngularDrag;
            pack = obj.pack;
            userful = obj.userful;
            eat = obj.eat;
            plant = obj.plant;
            suit = obj.suit;
            recovery = obj.recovery;
            spares = obj.spares;
            active = obj.active;
            listable = obj.listable;
            iznosable = obj.iznosable;
            weapon = obj.weapon;
            magazin = obj.magazin;
        }
    }

    public ItemProp Ip;

    public Item(string _name, float _health, bool _pack)
    {
        Ip = new ItemProp(_name, _health, _pack);
    }

    public void Start()
    {
        if (Ip.listable)
        {
            if (!transform.FindChild("Leaves"))
            {
                GameObject clone = (GameObject)Instantiate(Resources.Load("Objects/Leaves"), transform.position, Quaternion.identity);
                clone.transform.parent = transform;
            }
        }
        if (Ip.plant)
            gameObject.AddComponent<Plant>();
    }
}
