/*                             __         
                              |  |        __
                              |  |       |  |
                              |__|       |__|
                                  _______
*/
using UnityEngine;
using System.Collections.Generic;

public class PlayerGUI : MonoBehaviour
{

    public PlayerController pc;
    public PlayerInfo pi;
    public VaultInfo vi;
    // Char Icons
    public Texture2D charHappy;
    public Texture2D charNormal;
    public Texture2D charScared;
    public Texture2D currCharIcon;
    // Suit Properties
    public Texture2D suitHP;
    public Transform suitBar;
    public Vector3 suitBarStartWidth;

    public int currStack;
    public bool showmenu = false;
    Vector2 menuPos;
    // GUIContainers
    public GUIContainer inventoryMenu, pauseMenu;
    // GUIBlocks
    public GUIBlock exit, charBlock, activeBlock, activeStack;
    public List<GUIBlock> inventory = new List<GUIBlock>();
    public TextMesh age, roomHealth, hunger, health, radiation;

    GameObject clickGuiObj;

    public EventType lastEvent;

    public Collider lastObj;
    //public delegate void FunctionHandler();
    //Dictionary<string, FunctionHandler> commands = new Dictionary<string, FunctionHandler>();


    void Awake()
    {
        //commands.Add("GUISaveAndExit", GUISaveAndExit);

        pc = GetComponent<PlayerController>();
        pi = GetComponent<PlayerInfo>();
        vi = GameObject.Find("Vault").GetComponent<VaultInfo>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MyGUI"))
        {
            GUIContainer container = obj.GetComponent<GUIContainer>();
            if (container)
                container.playerGUI = this;
        }

        // Setup GUI Var
        Transform _t = GameObject.Find("PlayerGUIContainer").transform;
        for (int i = 0; i < _t.childCount; i++)
        {
            Transform stk = _t.GetChild(i);
            if (stk.name == "Stack")
                inventory.Add(stk.GetComponent<GUIBlock>());
        }

        inventoryMenu = _t.FindChild("InventoryMenu").GetComponent<GUIContainer>();
        pauseMenu = _t.FindChild("PauseMenu").GetComponent<GUIContainer>();
        charBlock = _t.FindChild("Character").GetComponent<GUIBlock>();
        exit = _t.FindChild("SaveAndExit").GetComponent<GUIBlock>();

        age = _t.FindChild("Age").GetComponent<TextMesh>();
        roomHealth = _t.FindChild("RoomHealth").GetComponent<TextMesh>();
        hunger = _t.FindChild("Hunger").GetComponent<TextMesh>();
        health = _t.FindChild("Health").GetComponent<TextMesh>();
        radiation = _t.FindChild("Radiation").GetComponent<TextMesh>();
        suitBar = _t.FindChild("Suit");
        suitBarStartWidth = suitBar.localScale;
        // End Setup        
    }

    public void Start()
    {
        UpdateCharIcon();
        ApplyCurrCharIcon();
        UpdateStacksIcon();
        Invoke("UpdateStacksIcon", 0.1f);
    }

    void OnGUI()
    {
        UpdateCharIcon();
        Event e = Event.current;
        RaycastHit hit;
        Ray ray = pc.MyCamera.GetChild(0).GetChild(0).camera.ScreenPointToRay(Input.mousePosition); // O_O


        if (e.isMouse)
        {
            if (Physics.Raycast(ray, out hit, 100, 1 << 12 | 1 << 14))
            {
                if (!clickGuiObj)
                    clickGuiObj = hit.transform.gameObject;
            }

            if (clickGuiObj && clickGuiObj.GetComponent<GUIBlock>())
            {
                if (e.type == EventType.MouseDrag && e.button == 0)
                    clickGuiObj.GetComponent<GUIBlock>().Drag();
                else if (e.type == EventType.MouseUp)
                    if (lastEvent != EventType.MouseDrag)
                        clickGuiObj.GetComponent<GUIBlock>().Click(e.button);
                    else if (e.button == 0)
                        clickGuiObj.GetComponent<GUIBlock>().DragUp();
            }

            // if (e.type != EventType.Ignore)
            lastEvent = e.type;
        }
    }

    ///////////////////////////////
    /////////////////////////////// Events
    ///////////////////////////////

    public void Hover()
    {

    }

    public void Click(GUIBlock block, int button)
    {
        activeBlock = block;

        Debug.Log("Click " + button);
        if (button == 0)
        {
            //commands["GUI" + block.name]();
            Invoke("GUIClick" + block.name, 0);
        }
        else if (button == 1)
        {
            Invoke("GUIRClick" + block.name, 0);
        }
        else if (button == 2)
        {
            Invoke("GUIMClick" + block.name, 0);
        }

    }

    public void Drag(GUIBlock block)
    {
        activeBlock = block;

        Color32 curr = activeBlock.renderer.material.color;
        activeBlock.renderer.material.color = new Color32(curr.r, curr.g, curr.b, 200);

        Debug.Log("Drag");
        Invoke("GUIDrag" + block.name, 0);
    }

    public void DragUp(GUIBlock block)
    {
        activeBlock = block;

        Color curr = activeBlock.renderer.material.color;
        activeBlock.renderer.material.color = new Color(curr.r, curr.g, curr.b, 255);
        Debug.Log("DragUp");
        Invoke("GUIDragUp" + block.name, 0);
    }

    ///////////////////////////////
    /////////////////////////////// End Events
    ///////////////////////////////

    public void UpdateCharIcon()
    {
        if (pi.Happy <= 30)
            currCharIcon = charScared;
        else if (pi.Happy > 30 && pi.Happy <= 70)
            currCharIcon = charNormal;
        else if (pi.Happy > 70)
            currCharIcon = charHappy;

        radiation.text = (System.Math.Round(pi.GetAverageRad() * pi.safeRadLevel, 1)).ToString();
        health.text = System.Math.Round(pi.Health, 2).ToString();
        hunger.text = System.Math.Round(pi.Hunger, 2).ToString();
        age.text = pi.Age.ToString();
        if (pi.GetMyRoom() != null)
            roomHealth.text = System.Math.Round(vi.transform.FindChild(pi.GetMyRoom()).GetComponent<VaultRoom>().Rp.myHealth, 1).ToString();
        else
            roomHealth.text = "";
        SetSuitBar();
    }

    public void ApplyCurrCharIcon()
    {
        charBlock.renderer.material.mainTexture = currCharIcon;
        Invoke("ApplyCurrCharIcon", 2);
    }

    public void UpdateStacksIcon()
    {
        for (int i = 0; i < pi.Stacks.Count; i++)
        {
            inventory[i].renderer.material.mainTexture = (Texture)Resources.Load("Textures/Items/" + pi.Stacks[i].GetName(), typeof(Texture));

            if (pi.Stacks[i].MyItems.Count > 0)
            {
                if (!pi.Stacks[i].MyItems[0].iznosable)
                    inventory[i].transform.GetChild(0).GetComponent<TextMesh>().text = pi.Stacks[i].MyItems.Count.ToString();
                else
                    inventory[i].transform.GetChild(0).GetComponent<TextMesh>().text = pi.Stacks[i].MyItems[0].health.ToString();
            }
            else
                inventory[i].transform.GetChild(0).GetComponent<TextMesh>().text = null;
        }
    }

    public bool IsUserful(Item.ItemProp pr)
    {
        return pr.userful;
    }

    public bool CheckUsable(Item.ItemProp one, GameObject two, bool use = true)
    {
        if (one.name == null)
            return false;

        if (one.recovery && two.GetComponent<VaultRoom>())
        {
            if (use)
                if (two.name == pi.GetMyRoom())
                    pi.Recovery(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()));
            return true;
        }
        if (one.plant && two.GetComponent<Greenhouse>())
        {
            if (use)
                if (two.GetComponent<Greenhouse>().canGrow)
                    pc.Plant(currStack, pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()), two);
            return true;
        }
        if (one.eat && two.GetComponent<PlayerInfo>())
        {
            if (use)
                pc.Eat(currStack, pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()));
            return true;
        }
        if (one.suit && two.GetComponent<PlayerInfo>())
        {
            if (use)
                pi.DressSuit(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()));
            return true;
        }

        return false;
    }

    public bool Use(Item.ItemProp one, GameObject two)
    {
        return CheckUsable(one, two);
    }

    public void SetSuitBar()
    {
        if (pi.activeSuit != null)
        {
            suitBar.gameObject.SetActive(true);
            suitBar.localScale = suitBarStartWidth * (pi.activeSuit.health / 600);
        }
        else
            suitBar.gameObject.SetActive(false);
    }

    // GUIBlocks Functions
    /*                             __         
                                  |  |        __
                                  |  |       |  |
                                  |__|       |__|
                                      _______
    */

    // GUIClick/RClick/MClick - обозначение обработчика, а то, что дальше - имя блока на сцене
    // Пример: GUIClick - тип обработчика. SaveAndExit - имя объекта на сцене
    public void GUIClickSaveAndExit()
    {
        Time.timeScale = 1.0f;
        pauseMenu.gameObject.SetActive(false);

        GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().Save("Save.save");
        Application.LoadLevel("MainMenu");
    }

    public void GUIClickPlant()
    {
        if (pi.Stacks[currStack].MyItems.Count == 0 || !pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).eat)
            return;
        pc.Plant(currStack, pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()), GameObject.FindGameObjectWithTag("Greenhouse"));
        inventoryMenu.gameObject.SetActive(false);
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIClickEat()
    {
        if (pi.Stacks[currStack].MyItems.Count == 0 || !pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).eat)
            return;
        pc.Eat(currStack, pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()));
        inventoryMenu.gameObject.SetActive(false);
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIClickDrop()
    {
        Debug.Log(currStack);
        if (pi.Stacks[currStack].GetName() != "")
        {
            pc.DropItem(currStack, pi.Stacks[currStack].GetName(), pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).id);
            inventoryMenu.gameObject.SetActive(false);
        }
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void Drop()
    {
        Debug.Log(currStack);
        if (pi.Stacks[currStack].GetName() != "")
        {
            pc.DropItem(currStack, pi.Stacks[currStack].GetName(), pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).id);
            inventoryMenu.gameObject.SetActive(false);
        }
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIClickDressSuit()
    {
        pi.DressSuit(pi.FindItem("Suit"));
        activeBlock = null;
        clickGuiObj = null;
        inventoryMenu.gameObject.SetActive(false);
        pi.showSuit = true;
    }

    public void GUIClickRemoveSuit()
    {
        pi.RemoveSuit();
        activeBlock = null;
        clickGuiObj = null;
        inventoryMenu.gameObject.SetActive(false);
        pi.showSuit = false;
    }

    public void GUIClickRecovery()
    {
        pi.Recovery(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()));
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
        inventoryMenu.gameObject.SetActive(false);
    }

    public void GUIClickStack()
    {
        if (pi.Stacks[currStack].MyItems.Count == 0)
        {
            activeBlock = null;
            activeStack = null;
            clickGuiObj = null;
        }

    }

    public void GUIClickDoor()
    {
        Door dr = activeBlock.GetComponent<Door>();
        dr.Opened = !dr.Opened;
        activeBlock = null;
        clickGuiObj = null;
    }

    public void GUIRClickStack() // Переписать! x_x
    {
        activeStack = activeBlock;
        currStack = inventory.IndexOf(activeStack);

        Transform invtr = inventoryMenu.transform;
        if (invtr.gameObject.activeSelf)
        {
            activeBlock = null;
            activeStack = null;
            clickGuiObj = null;
            invtr.gameObject.SetActive(false);
            return;
        }
        invtr.gameObject.SetActive(!invtr.gameObject.activeSelf);
        invtr.localPosition = new Vector3(activeBlock.transform.localPosition.x, invtr.localPosition.y, invtr.localPosition.z);
        currStack = inventory.IndexOf(activeBlock);

        if (pi.Stacks[currStack].MyItems.Count > 0) // Если стек не пустой
        {
            for (int i = 0; i < invtr.childCount; i++)
            {
                invtr.GetChild(i).gameObject.SetActive(false);
            }
            if (IsUserful(pi.Stacks[inventory.IndexOf(activeBlock)].MyItems[0])) // Если то, что в нем юзабельно
            {
                if (pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).plant) // Если это растение
                {
                    invtr.FindChild("Plant").gameObject.SetActive(true);
                    invtr.FindChild("Eat").gameObject.SetActive(true);
                }
                else if (pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).eat) //Если еда
                {
                    invtr.FindChild("Eat").gameObject.SetActive(true);
                }
                else if (pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).suit) // Если это костюм
                {
                    invtr.FindChild("DressSuit").gameObject.SetActive(true);
                    invtr.FindChild("RemoveSuit").gameObject.SetActive(true);
                }
                else if (pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).recovery) // Если это ремкомплект
                {
                    invtr.FindChild("Recovery").gameObject.SetActive(true);
                }
                invtr.FindChild("Drop").gameObject.SetActive(true);
            }
        }
        else
        {
            invtr.gameObject.SetActive(false);
        }
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIClickPause()
    {
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIClickResume()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIDragSaveAndExit()
    {
        GUIClickSaveAndExit();
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIDragStack()
    {
        Collider currObj = null;
        activeBlock.transform.position = pc.MyCamera.GetChild(0).GetChild(0).camera.ScreenPointToRay(Input.mousePosition).GetPoint(0.1f);
        activeBlock.transform.localScale = new Vector3(0.1f, 0.06f, 0.1f);
        activeStack = activeBlock;
        currStack = inventory.IndexOf(activeStack);

        Ray ray = pc.MyCamera.GetChild(0).GetChild(0).camera.ScreenPointToRay(Input.mousePosition); ;
        RaycastHit hit;
        ray.origin = activeBlock.transform.position;


        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).name + "     " + hit.transform.name);

            currObj = hit.collider;
            if (CheckUsable(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()), hit.transform.gameObject, false) && currObj.transform.GetComponent<UseLight>())
                if (!(hit.transform.GetComponent<VaultRoom>() && (hit.transform.name != pi.GetMyRoom())))
                    currObj.transform.GetComponent<UseLight>().SetNewMat();

        }
        else currObj = null;

        if (!lastObj)
            lastObj = currObj;

        if (lastObj != currObj && lastObj.transform.GetComponent<UseLight>())
            lastObj.transform.GetComponent<UseLight>().SetStartMat();

        lastObj = currObj;
    }

    public void GUIDragDoor()
    {
        activeBlock = null;
        clickGuiObj = null;
    }

    public void GUIDragUpStack()
    {
        activeStack = activeBlock;
        currStack = inventory.IndexOf(activeStack);
        if (lastObj && lastObj.transform.GetComponent<UseLight>())
            lastObj.transform.GetComponent<UseLight>().SetStartMat();

        if (pi.Stacks[currStack].MyItems.Count == 0)
        {
            activeBlock.transform.localPosition = activeBlock.startPos;
            activeBlock.transform.localScale = new Vector3(0.3f, 0.06f, 0.3f);
            activeBlock = null;
            activeStack = null;
            clickGuiObj = null;
            return;
        }

        Ray ray = pc.MyCamera.GetChild(0).GetChild(0).camera.ScreenPointToRay(Input.mousePosition); ;
        RaycastHit hit;
        ray.origin = activeBlock.transform.position;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()).name + "     " + hit.transform.name);
            if (hit.transform.name != "Stack" && !Use(pi.FindLastItem(currStack, pi.Stacks[currStack].GetName()), hit.transform.gameObject))
                Drop();
            else if (hit.transform.name == "Stack")
            {
                if (pi.Stacks[inventory.IndexOf(hit.transform.GetComponent<GUIBlock>())].MyItems.Count < 20)
                {
                    if (pi.Stacks[inventory.IndexOf(hit.transform.GetComponent<GUIBlock>())].MyItems.Count > 0) // Если что-то есть
                    {
                        // Если оно того же типа
                        if (pi.Stacks[inventory.IndexOf(hit.transform.GetComponent<GUIBlock>())].MyItems[0].name == pi.Stacks[currStack].MyItems[0].name)
                        {
                            pi.Stacks[inventory.IndexOf(hit.transform.GetComponent<GUIBlock>())].MyItems.Add(new Item.ItemProp(pi.Stacks[currStack].MyItems[pi.Stacks[currStack].MyItems.Count - 1]));
                            pi.Stacks[currStack].MyItems.RemoveAt(pi.Stacks[currStack].MyItems.Count - 1);
                        }
                    }
                    else // Если ничего нет, то на равность названий итемов можно не проверять
                    {
                        pi.Stacks[inventory.IndexOf(hit.transform.GetComponent<GUIBlock>())].MyItems.Add(new Item.ItemProp(pi.Stacks[currStack].MyItems[pi.Stacks[currStack].MyItems.Count - 1]));
                        pi.Stacks[currStack].MyItems.RemoveAt(pi.Stacks[currStack].MyItems.Count - 1);
                    }
                }
            }
        }
        activeBlock.transform.localPosition = activeBlock.startPos;
        activeBlock.transform.localScale = new Vector3(0.3f, 0.06f, 0.3f);
        UpdateStacksIcon();
        activeBlock = null;
        activeStack = null;
        clickGuiObj = null;
    }

    public void GUIDragUpDoor()
    {
        activeBlock = null;
        clickGuiObj = null;
    }

    public void GUIDragUpSaveAndExit()
    {
        activeBlock = null;
        clickGuiObj = null;
    }
}
