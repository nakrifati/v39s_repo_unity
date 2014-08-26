using UnityEngine;
using System.Collections;

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
    public Collider inv;

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
                if (_hit.transform.root.GetComponent<Item>())
                {
                    inv = _hit.collider;
                    SetDestination();
                }
            }
        }
        if (Input.GetButton("Fire2"))
        {
            bot.canMove = true;
            SetDestination();
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
                    obj.rigidbody.AddForce((transform.forward + Random.insideUnitSphere * 2) * 100);
        }
        CameraOffset.y -= Input.GetAxis("Mouse ScrollWheel") * 4;
        if (CameraOffset.y < camMinY)
            CameraOffset.y = camMinY + 0.01f;
        else if (CameraOffset.y > camMaxY)
            CameraOffset.y = camMaxY - 0.01f;

        if (inv && (inv.transform.position - transform.position).magnitude < 1)
            Grab(inv);

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
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject);
                            return;
                        }
                    }

                    foreach (PlayerInfo.MyStack stk in pi.Stacks) // Если нет таких же предметов в инвентаре, то кидаем в пустой слот
                    {
                        if (stk.MyItems.Count == 0)
                        {
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject);
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
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void Grab(Collider coll)
    {
        bool flag = false;
        foreach (Collider obj in Physics.OverlapSphere(transform.position, 10))
        {
            if (obj == inv)
                flag = true;
        }
        if (flag)
        {
            Item _i = coll.transform.root.GetComponent<Item>();
            if (_i)
            {
                if (_i.Ip.pack) // Если можно сложить в стопку
                {
                    foreach (PlayerInfo.MyStack stk in pi.Stacks) // Если мы нашли такой же предмет в стопке, то кидаем туда
                    {
                        if (stk.MyItems.Count > 0 && stk.MyItems[0].name == _i.Ip.name && stk.MyItems.Count < 20)
                        {
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject);
                            return;
                        }
                    }

                    foreach (PlayerInfo.MyStack stk in pi.Stacks) // Если нет таких же предметов в инвентаре, то кидаем в пустой слот
                    {
                        if (stk.MyItems.Count == 0)
                        {
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject);
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
                            TakeItem(pi.Stacks.IndexOf(stk), coll.transform.root.gameObject);
                            return;
                        }
                    }
                }
            }
            inv = null;
        }
    }

    // Поднять предмет
    public void TakeItem(int stkNum, GameObject obj)
    {
        pi.AddItem(obj.GetComponent<Item>(), stkNum);
        Destroy(obj.gameObject);

        pg.UpdateStacksIcon();
    }

    // Выбросить предмет
    public void DropItem(int _stk, string _name, uint _id)
    {
        if (pi.FindItem(_stk, _id) != null)
        {
            GameObject clone = (GameObject)Instantiate(Resources.Load("Objects/" + _name), MyCamera.position + new Vector3(0, 1, 0), Random.rotation);
            Item it = clone.GetComponent<Item>();
            it.Ip.listable = false;
            if (it.Ip.suit)
                pi.RemoveSuit();
            it.Ip = pi.FindItem(_stk, _id);            
            pi.DeleteItem(_stk, pi.FindItem(_stk, _id));
        }
        pg.UpdateStacksIcon();
    }

    public void CameraMove(Vector3 offset)
    {
        //MyCamera.position = Vector3.Lerp(MyCamera.position, transform.position + offset, Time.deltaTime * CameraSpeed);
        MyCamera.position = Vector3.Lerp(MyCamera.position, transform.position, Time.deltaTime * CameraSpeed);
        MyCamera.GetChild(0).localPosition = offset;
        offset.z = -offset.y / 2;
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
        pi.Health += 75;
        if (pi.Health > 100)
            pi.Health = 100;
        pi.Hunger += 75;
        if (pi.Hunger > 100)
            pi.Hunger = 100;

        pi.DeleteItem(currStack, it);
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