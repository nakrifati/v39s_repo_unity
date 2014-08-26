using UnityEngine;
using System.Collections;

[System.Serializable]
public class Computers : MonoBehaviour
{
    public CompData cd = new CompData();

    void Start()
    {
        cd.comp = CompCreator.Create(cd.compName, cd.health);
        InvokeRepeating("GiveRegularDamage", 1200, 1200);
        InvokeRepeating("GivePlayerDamage", 1, 10);
        InvokeRepeating("GiveDoorDamage", 1, 1);
    }

    void GivePlayerDamage()
    {
        cd.comp.GivePlayerDamage();
        cd.health = cd.comp.GetHealth();
    }

    void GiveDoorDamage()
    {
        cd.comp.GiveDoorDamage();
        cd.health = cd.comp.GetHealth();
    }

    public void GiveRegularDamage()
    {
        cd.comp.GiveDamage(cd.regularDamage);
        cd.health = cd.comp.GetHealth();
    }
}

[System.Serializable]
public class CompData
{
    public ComputerType comp;
    public string compName;
    public float regularDamage, health;
}

public interface ComputerType
{
    void GiveDamage(float dam);
    float GetHealth();
    void GivePlayerDamage();
    void GiveDoorDamage();
    string GetName();
}

public class CompCreator
{
    public static ComputerType Create(string type, float health)
    {
        if (type == "CompAir")
            return new CompAir(health);
        else if (type == "CompDoor")
            return new CompDoor(health);

        return null;
    }
}

[System.Serializable]
public class CompAir : ComputerType
{
    public CompAir(float h)
    {
        health = h;
    }

    float health = 100;

    public void GiveDamage(float dam)
    {
        health -= dam;
        if (health < 0)
            health = 0;
    }

    public float GetHealth()
    {
        return health;
    }

    public string GetName()
    {
        return "CompAir";
    }

    public void GivePlayerDamage()
    {
        float h = health;
        GameObject player = GameObject.Find("Player");
        PlayerInfo pi = null;
        if (player)
            pi = player.GetComponent<PlayerInfo>();

        if (h < 50)
            if (h < 25)
            {
                if (h <= 0)
                    pi.newHealth -= 20;
                else
                    pi.newHealth -= 10;
            }
            else
                pi.newHealth -= 5;

        if (health < 0)
            health = 0;
    }

    public void GiveDoorDamage()
    {
    }

}

[System.Serializable]
public class CompDoor : ComputerType
{

    public CompDoor(float h)
    {
        health = h;
    }

    float health = 100;

    public void GiveDamage(float dam)
    {
        health -= dam;
        if (health < 0)
            health = 0;
    }

    public float GetHealth()
    {
        return health;
    }

    public string GetName()
    {
        return "CompDoor";
    }

    public void GivePlayerDamage()
    {
    }

    public void GiveDoorDamage()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (health <= 50)
                obj.GetComponent<Door>().CanRemoteControl = false;
            else
                obj.GetComponent<Door>().CanRemoteControl = true;
        }
    }

}