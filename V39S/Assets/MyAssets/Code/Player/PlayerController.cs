using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public PlayerInfo pi;
    public PlayerGUI pg;
    public MineBotAI bot;
    public Transform MyCamera;
    public Vector3 MovePoint, CameraOffset, CameraExtraOffset;
    public float CameraSpeed, camMinY, camMaxY;

    public GameObject particle;
    protected Animator animator;
    public GameObject particleClone;

    public Vector3 destinationPoint, hp;
    public float stopDistance, speedDamp, angularDamp, DirectionDamp;
    public bool stop = false;
    RaycastHit hit = new RaycastHit();
    public GameObject inv;

    void Start()
    {
        // Debug
        print("I was created (Player)");
        // Debug
        pi = GetComponent<PlayerInfo>();
        pg = GetComponent<PlayerGUI>();
        bot = GetComponent<MineBotAI>();
        animator = GetComponent<Animator>();
        MyCamera = GameObject.Find("MyCamera").transform;
        particleClone = Instantiate(particle.gameObject, transform.position, Quaternion.identity) as GameObject;
        destinationPoint = hp = transform.position;
        GameObject.FindGameObjectWithTag("VaultEnvironment").GetComponent<VaultEnvironment>().Player = gameObject;
        GameObject.FindGameObjectWithTag("VaultEnvironment").GetComponent<VaultEnvironment>().Pc = this;
    }

    void FixedUpdate()
    {
        Ray ray = MyCamera.GetChild(0).camera.ScreenPointToRay(Input.mousePosition);
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            RaycastHit _hit;
            Ray ray = MyCamera.GetChild(0).camera.ScreenPointToRay(Input.mousePosition);
            if (!pg.activeBlock && Physics.Raycast(ray, out _hit, 100))
            {
                if (_hit.transform.root.name != "MyCamera" && (pi.activeWeapon.name != "" && pi.activeWeapon.active))
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(_hit.point.x, transform.position.y, _hit.point.z) - transform.position), 500 * Time.deltaTime);
                }
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            RaycastHit _hit;
            Ray ray = MyCamera.GetChild(0).camera.ScreenPointToRay(Input.mousePosition);
            if (!pg.activeBlock && Physics.Raycast(ray, out _hit, 100))
            {
                if (_hit.transform.root.GetComponent<Item>())
                {
                    inv = _hit.transform.root.gameObject;
                    SetDestination();
                }
                else if (_hit.transform.name != "Door" && _hit.transform.root.name != "Storage" && _hit.transform.root.name != "MyCamera" && (pi.activeWeapon.name != "" && pi.activeWeapon.active))
                {
                    pi.Shoot(_hit.point);
                }
            }
        }



        if (Input.GetButton("Fire2"))
        {
            RaycastHit _hit;
            Ray ray = MyCamera.GetChild(0).camera.ScreenPointToRay(Input.mousePosition);
            if (!pg.activeBlock && Physics.Raycast(ray, out _hit, 100))
            {
                bot.canMove = true;
                SetDestination();
            }
        }


        if (Input.GetButton("Fire3"))
        {
            MyCamera.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * 10, 0);
        }
        if (Input.GetButtonDown("Kick"))
        {
            // TODO анимация
            foreach (Collider obj in Physics.OverlapSphere(transform.position + transform.forward, 1))
                if (obj != collider && obj.rigidbody)
                    obj.rigidbody.AddForce((transform.forward + Random.insideUnitSphere * 2) * 50);
        }
        CameraOffset.y -= Input.GetAxis("Mouse ScrollWheel") * 4;
        if (CameraOffset.y < camMinY)
            CameraOffset.y = camMinY + 0.01f;
        else if (CameraOffset.y > camMaxY)
            CameraOffset.y = camMaxY - 0.01f;

        if (inv && (inv.transform.position - transform.position).magnitude < 1.3f)
            Grab(inv, inv, pi.Stacks);

        float x = Input.GetAxis("Horizontal"), z = Input.GetAxis("Vertical"), a = Input.GetAxis("Acceleration");
        if (x != 0 || z != 0)
        {
            //stop = false;
            //hp = transform.position + new Vector3(x, transform.position.y, z) * (1 + a * 4);

            bot.canMove = true;
            hp = transform.position + (MyCamera.right * x * 1.5f + MyCamera.forward * z * 1.5f) * (1 + a * 4);
        }

        destinationPoint = Vector3.Lerp(destinationPoint, hp, 5 * Time.deltaTime);

        if (Input.GetButtonDown("Save"))
            GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().Save("Save.save");
        else if (Input.GetButtonDown("Load"))
            GameObject.FindGameObjectWithTag("SaveLoad").GetComponent<SaveAndLoad>().Load("Save.save");


        if (Input.GetButton("Grab"))
        {
            Grab();
        }
    }

    public void LateUpdate()
    {
        particleClone.transform.position = destinationPoint;
        CameraMove(CameraOffset + CameraExtraOffset);
        if ((destinationPoint - transform.position).magnitude > 0.5f)
            bot.canMove = true;
        else
            bot.canMove = false;
    }

    public void Grab()
    {
        foreach (Collider coll in Physics.OverlapSphere(transform.position, 1.0f))
        {
            Item _i = coll.transform.root.gameObject.GetComponent<Item>();
            if (_i)
            {
                if (_i.Ip.pack) // Если можно сложить в стопку
                {
                    foreach (PlayerInfo.MyStack stk in pi.Stacks) // Если мы нашли такой же предмет в не полном стопке, то кидаем туда
                    {
                        if (stk.MyItems.Count > 0 && stk.MyItems[0].name == _i.Ip.name && stk.MyItems.Count < 20)
                        {
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject, pi.Stacks);
                            return;
                        }
                    }

                    foreach (PlayerInfo.MyStack stk in pi.Stacks) // Если нет таких же предметов в инвентаре, то кидаем в пустой слот
                    {
                        if (stk.MyItems.Count == 0)
                        {
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject, pi.Stacks);
                            return;
                        }
                    }
                }
                else
                {
                    foreach (PlayerInfo.MyStack stk in pi.Stacks) // Если есть пустые слоты, то кладем в первый такой
                    {
                        if (stk.MyItems.Count == 0)
                        {
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject, pi.Stacks);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void Grab(GameObject coll, GameObject targ, List<PlayerInfo.MyStack> targList)
    {
        bool flag = false;
        foreach (Collider obj in Physics.OverlapSphere(transform.position, 10))
        {
            if (obj.transform.root.gameObject == targ)
            {
                flag = true;
                break;
            }
        }
        if (flag)
        {
            Debug.Log("Flag true");

            Item _i = coll.transform.root.GetComponent<Item>();
            if (_i)
            {

                Debug.Log("I true");

                if (_i.Ip.pack) // Если можно сложить в стопку
                {
                    foreach (PlayerInfo.MyStack stk in targList) // Если мы нашли такой же предмет в стопке, то кидаем туда
                    {
                        if (stk.MyItems.Count > 0 && stk.MyItems[0].name == _i.Ip.name && stk.MyItems.Count < 20)
                        {
                            TakeItem(targList.IndexOf(stk), coll.transform.root.gameObject, targList);
                            return;
                        }
                    }

                    foreach (PlayerInfo.MyStack stk in targList) // Если нет таких же предметов в инвентаре, то кидаем в пустой слот
                    {
                        if (stk.MyItems.Count == 0)
                        {
                            TakeItem(targList.IndexOf(stk), coll.transform.root.gameObject, targList);
                            return;
                        }
                    }
                }
                else
                {
                    foreach (PlayerInfo.MyStack stk in targList) // Если есть пустые слоты, то кладем в первый такой
                    {
                        if (stk.MyItems.Count == 0)
                        {
                            TakeItem(targList.IndexOf(stk), coll.transform.root.gameObject, targList);
                            return;
                        }
                    }
                }
            }
            inv = null;
        }
    }

    // Поднять предмет
    public void TakeItem(int stkNum, GameObject obj, List<PlayerInfo.MyStack> targList)
    {
        pi.AddItem(obj.GetComponent<Item>(), stkNum, targList);
        Destroy(obj.gameObject);

        pg.UpdateStacksIcon();
    }

    // Выбросить предмет
    public void DropItem(int _stk, string _name, uint _id)
    {
        if (pi.FindItem(_stk, _id, pi.Stacks) != null)
        {
            GameObject clone = (GameObject)Instantiate(Resources.Load("Objects/" + _name), MyCamera.position + new Vector3(0, 1, 0), Random.rotation);
            Item it = clone.GetComponent<Item>();
            it.Ip.listable = false;
            if (it.Ip.suit)
                pi.RemoveSuit();
            it.Ip = pi.FindItem(_stk, _id, pi.Stacks);
            pi.DeleteItem(_stk, pi.FindItem(_stk, _id, pi.Stacks), pi.Stacks);
        }
        pg.UpdateStacksIcon();
    }

    public void CameraMove(Vector3 offset)
    {
        //MyCamera.position = Vector3.Lerp(MyCamera.position, transform.position + offset, Time.deltaTime * CameraSpeed);
        MyCamera.position = Vector3.Lerp(MyCamera.position, transform.position, Time.deltaTime * CameraSpeed);
        offset.z = -offset.y / 2;
        MyCamera.GetChild(0).localPosition = offset;        
    }

    //void OnAnimatorMove()
    //{
    //    transform.rotation = animator.rootRotation;
    //    rigidbody.velocity = animator.deltaPosition / Time.deltaTime;//transform.forward * animator.GetFloat("Speed");
    //    //CameraMove(CameraOffset + CameraExtraOffset);
    //}

    public void SetDestination()
    {
        bot.canMove = true;
        //stop = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 500))
        {
            if (hit.transform.gameObject.layer != 12 && hit.transform.gameObject.layer != 9)
                hp = hit.point;
        }
    }

    public void SetDestination(Vector3 pos)
    {
        bot.canMove = true;
        //stop = false;
        hp = pos;
    }

    public void Eat(int currStack, Item.ItemProp it)
    {
        pi.newHealth += 75;
        if (pi.newHealth > 100)
            pi.newHealth = 100;
        pi.newHunger += 75;
        if (pi.newHunger > 100)
            pi.newHunger = 100;

        pi.regenHealth = true; pi.regenHunger = true;

        pi.DeleteItem(currStack, it, pi.Stacks);
        pg.UpdateStacksIcon();
    }

    public void Plant(int currStack, Item.ItemProp it, GameObject greenhouseGo)
    {
        Greenhouse gr = greenhouseGo.GetComponent<Greenhouse>();

        if (gr)
        {
            gr.playerName = pi.Name;
            gr.foodName = it.name;
            gr.playerCurrStack = currStack;
            gr.canSearch = true;

            SetDestination(greenhouseGo.transform.position);
        }
    }
}