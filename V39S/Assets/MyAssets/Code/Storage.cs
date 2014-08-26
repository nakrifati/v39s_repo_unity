using UnityEngine;
using System.Collections.Generic;

public class Storage : MonoBehaviour
{
    public List<PlayerInfo.MyStack> Stacks = new List<PlayerInfo.MyStack>();
    public List<StorageGUIStack> guiStacks = new List<StorageGUIStack>();

    void Awake()
    {
        if (Stacks.Count == 0)
            for (int i = 0; i < 10; i++)
            {
                PlayerInfo.MyStack S = new PlayerInfo.MyStack();
                Stacks.Add(S);
            }
    }

    void Start()
    {
        InvokeRepeating("Close", 1, 0.5f);
    }

    void Update()
    {

    }

    public bool CheckDistance()
    {
        if ((GameObject.Find("Player").transform.position - transform.position).magnitude < 2)
            return true;

        return false;
    }

    public void Close()
    {
        if (!CheckDistance())
            transform.GetChild(0).gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        if (CheckDistance())
            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        UpdateStacksIcon();
    }

    public void UpdateStacksIcon()
    {
        for (int i = 0; i < Stacks.Count; i++)
        {
            guiStacks[i].renderer.material.mainTexture = (Texture)Resources.Load("Textures/Items/" + Stacks[i].GetName(), typeof(Texture));

            if (Stacks[i].MyItems.Count > 0)
            {
                if (!Stacks[i].MyItems[0].iznosable)
                    guiStacks[i].transform.GetChild(0).GetComponent<TextMesh>().text = Stacks[i].MyItems.Count.ToString();
                else
                    guiStacks[i].transform.GetChild(0).GetComponent<TextMesh>().text = Stacks[i].MyItems[0].health.ToString();
            }
            else
                guiStacks[i].transform.GetChild(0).GetComponent<TextMesh>().text = null;
        }
        Invoke("UpdateStacksIcon", 2.0f);
    }
}