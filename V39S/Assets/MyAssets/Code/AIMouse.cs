using UnityEngine;
using System.Collections;

public class AIMouse : MonoBehaviour
{
    public Transform tr, bloodSpritePrefab, bloodSprite;
    public MineBotAIMouse path;
    public AIMouseData data;

    public Transform target;
    public float attackDistance, attackRate, damage, searchRate;

    public bool life = true;
    public GameObject Spurt;

    void Awake()
    {
        tr = transform;

        path = GetComponent<MineBotAIMouse>();
        path.player = gameObject;
        path.target = target;
    }

    void Start()
    {
        Invoke("FindTarget", searchRate);
        Invoke("Attack", attackRate);
    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision coll)
    {
        GiveDamage(coll.relativeVelocity.magnitude);
    }

    public void FindTarget()
    {
        int i = Random.Range(0, 4);
        GameObject trg = GameObject.Find("Player");
        if (trg && (trg.transform.position - tr.position).magnitude < 5)
        {
            target = trg.transform;
            path.canSearch = true;
            path.target = target;
            path.canSearch = true;
        }
        else if (i == 0)
        {

        }
        else if (i == 1)
        {
            transform.Rotate(transform.up, Random.Range(-90, 91));
            RaycastHit rhit;
            if (Physics.Raycast(transform.position, transform.forward, out rhit, 1000))
            {
                target = rhit.transform;
                path.canSearch = true;
                path.target = target;
                path.canSearch = true;
            }
        }
        else if (i == 2)
        {
            target = tr;
            path.canSearch = true;
            path.target = target;
            path.canSearch = true;
        }
        else
        {
            tr.localRotation *= Quaternion.Euler(0, Random.Range(-90, 90), 0);
        }
        if (life)
            Invoke("FindTarget", searchRate);
    }

    public void Attack()
    {
        if (target && target.GetComponent<PlayerInfo>() && (target.position - tr.position).magnitude <= attackDistance)
        {
            target.GetComponent<PlayerInfo>().newHealth -= damage;
            target.GetComponent<PlayerInfo>().Health -= damage;
        }
        if (life)
            Invoke("Attack", attackRate);
    }

    public void GiveDamage(float dam)
    {
        data.health -= dam;
        if (data.health <= 0)
        {
            data.health = 0;
            Death();
        }
    }

    public void LowOpacity()
    {
        //renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, renderer.material.color.a - 0.03f);

        //if (renderer.material.color.a <= 0)
            Destroy(gameObject);

        Invoke("LowOpacity", 1);
    }

    public void Death()
    {
        if (!bloodSprite)
        {
            bloodSprite = (Transform)Instantiate(bloodSpritePrefab, tr.position, Quaternion.Euler(90, 0, 0));
            Destroy(GetComponent<TypeHolder>());
            Destroy(GetComponent<MineBotAIMouse>());
            Destroy(GetComponent<Seeker>());
            LowOpacity();
            var Spurt02PS = Instantiate(Spurt, gameObject.transform.position, Quaternion.identity) as GameObject;
            //Spurt.transform.parent = gameObject.gameObject.transform;
        }
        life = false;
    }

    void OnCollisionEnter(Collider coll)
    {
        if (coll.transform.name == "Storage")
            transform.Rotate(transform.up, Random.Range(-90, 91));
    }
}

[System.Serializable]
public class AIMouseData
{
    public float health;
}
