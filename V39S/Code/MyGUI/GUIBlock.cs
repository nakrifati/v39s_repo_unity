using UnityEngine;
using System.Collections.Generic;

public class GUIBlock : MonoBehaviour
{
    public Texture2D normal;
    public Texture2D hold;
    public Texture2D pressed;

    public PlayerGUI playerGUI;
    public GUIContainer parent;
    public Vector3 startPos;
    public bool isPressed;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {

    }

    public void Click(int button)
    {
        playerGUI.Click(this, button);
        // TODO ������� � ����� �������
    }

    public void Drag()
    {
        playerGUI.Drag(this);
        // TODO ������� � ����� �������
    }

    public void DragUp()
    {
        playerGUI.DragUp(this);
    }
}
