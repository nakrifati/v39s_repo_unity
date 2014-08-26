using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public GameObject projectile, fakeBullet;
    public float damage, precision, fireRate, nextFire, bulletSpeed;
    public Item it;
    public PlayerInfo pi;

    Vector3 point, normal;
    Transform tr;

    void Start()
    {
        pi = transform.root.GetComponent<PlayerInfo>();
        tr = transform;
        it = GetComponent<Item>();
    }

    void Update()
    {

    }

    public void Shoot(Vector3 target)
    {
        if (it.Ip.magazin > 0)
        {
            if (nextFire <= Time.time)
            {
                RaycastHit hit;

                if (Physics.Raycast(tr.position, (target - tr.position) + Random.insideUnitSphere * (1 / precision), out hit, 500))
                {
                    if (hit.transform.root.GetComponent<AIMouse>())
                        hit.transform.root.GetComponent<AIMouse>().GiveDamage(damage);

                    point = hit.point;
                    normal = hit.normal;

                    GameObject bullet = (GameObject)Instantiate(fakeBullet, tr.position, Random.rotation);
                    bullet.GetComponent<ProjectileMove>().to = point;
                    bullet.GetComponent<ProjectileMove>().speed = bulletSpeed;
                    Destroy(bullet.gameObject, 1f);

                    Invoke("CreateProjectile", 1 / bulletSpeed);
                }

                it.Ip.magazin -= 1;
                nextFire = Time.time + fireRate;
            }
        }
        else Reload();
        pi.pg.UpdateStacksIcon();
    }

    public void Reload()
    {
        Item.ItemProp ip = pi.FindItem(it.name + "_magazin", pi.Stacks);
        if (ip.name == it.name + "_magazin")
        {
            it.Ip.magazin = ip.magazin;
            pi.DeleteItem(ip, pi.Stacks);
        }
    }

    public void CreateProjectile()
    {
        Instantiate(projectile, point, Quaternion.LookRotation(Vector3.Reflect(point - tr.position, normal)));
    }
}