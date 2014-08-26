using UnityEngine;
using System.Collections;

public class StorageGUIStack : MonoBehaviour
{
    Vector3 startScale;
    Vector3 startPos;
    public Storage st;
    public PlayerController pc;

    void Start()
    {
        st = transform.root.GetComponent<Storage>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        startPos = transform.position;
        startScale = transform.localScale;
    }

    void Update()
    {

    }

    void OnMouseDown()
    {
        pc.pg.clickGuiObj = null;
    }

    void OnMouseDrag()
    {
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0.3f);
        transform.localScale = startScale * 0.1f;
        transform.position = pc.MyCamera.GetChild(0).GetChild(0).camera.ScreenPointToRay(Input.mousePosition).GetPoint(0.1f);
    }

    void OnMouseUp()
    {
        int currNum = st.guiStacks.IndexOf(this);

        if (st.Stacks[currNum].MyItems.Count > 0)
        {
            RaycastHit hit;
            Ray ray = pc.MyCamera.GetChild(0).GetChild(0).camera.ScreenPointToRay(Input.mousePosition);
            ray.origin = transform.position;

            if (Physics.Raycast(ray, out hit, 10))
            {
                if (hit.transform.name == "Stack")
                {
                    GameObject clone = (GameObject)Instantiate((Resources.Load("Objects/" + st.Stacks[currNum].MyItems[st.Stacks[currNum].MyItems.Count - 1].name)), pc.transform.position, Quaternion.identity);
                    clone.GetComponent<Item>().Ip = st.Stacks[currNum].MyItems[st.Stacks[currNum].MyItems.Count - 1];
                    st.Stacks[currNum].MyItems.RemoveAt(st.Stacks[currNum].MyItems.Count - 1);

                    pc.Grab(clone, clone, pc.pi.Stacks); // Сказать игроку взять предмет в свой инвентарь
                }
                else if (hit.transform.name == "StorageGUIStack")
                {
                    if (st.Stacks[st.guiStacks.IndexOf(hit.transform.GetComponent<StorageGUIStack>())].MyItems.Count < 20)
                    {
                        if (st.Stacks[st.guiStacks.IndexOf(hit.transform.GetComponent<StorageGUIStack>())].MyItems.Count > 0) // Если что-то есть
                        {
                            // Если оно того же типа
                            if (st.Stacks[st.guiStacks.IndexOf(hit.transform.GetComponent<StorageGUIStack>())].MyItems[0].name == st.Stacks[currNum].MyItems[0].name && st.Stacks[currNum].MyItems[0].pack)
                            {
                                st.Stacks[st.guiStacks.IndexOf(hit.transform.GetComponent<StorageGUIStack>())].MyItems.Add(new Item.ItemProp(st.Stacks[currNum].MyItems[st.Stacks[currNum].MyItems.Count - 1]));
                                st.Stacks[currNum].MyItems.RemoveAt(st.Stacks[currNum].MyItems.Count - 1);
                            }
                        }
                        else // Если ничего нет, то на равность названий итемов можно не проверять
                        {
                            st.Stacks[st.guiStacks.IndexOf(hit.transform.GetComponent<StorageGUIStack>())].MyItems.Add(new Item.ItemProp(st.Stacks[currNum].MyItems[st.Stacks[currNum].MyItems.Count - 1]));
                            st.Stacks[currNum].MyItems.RemoveAt(st.Stacks[currNum].MyItems.Count - 1);
                        }
                    }
                }
            }
        }

        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1);
        transform.localScale = startScale;
        transform.position = startPos;

        pc.pg.clickGuiObj = null;

        st.UpdateStacksIcon();
    }

}
