using UnityEngine;
using System.Collections;

public class GUIContainer : MonoBehaviour
{
    public PlayerGUI playerGUI;
    public GUIContainer parent;

    void Start()
    {
        SetProperties();
    }

    void Update()
    {

    }

    public void SetProperties()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GUIBlock obj = transform.GetChild(i).GetComponent<GUIBlock>();
            GUIContainer cont = transform.GetChild(i).GetComponent<GUIContainer>();

            if (obj)
            {
                obj.parent = this;
                obj.playerGUI = playerGUI;
            }
            else if (cont)
            {
                cont.parent = this;
                cont.playerGUI = playerGUI;
                cont.SetProperties();
            }
        }
    }
}
