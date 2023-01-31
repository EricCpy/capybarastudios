using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject turret;
    public Transform target;
    public Transform body;
    public AudioSource explosionSound;
    public GameObject explosionEffect;
    public int damage = 10;
    public float range = 1000f;
    public float cooldown = .5f;
    public float maxHealth = 250;
    public float radius;
    public bool canShoot;
    private int controllerMask = ~(1 << 15);

    //Shooting
    public Transform firePointLeft;
    public Transform firePointRight;
    public TrailRenderer BulletTrail;
    private bool readyToShoot = true;
    private bool isLeft = true;
    private Weapon weapon;

    private void Start() {
        weapon = GetComponent<Weapon>();
        GetComponent<SphereCollider>().radius = radius;
    }

    private void Update() 
    {
        if (target != null)
        {
            //aim
            body.transform.LookAt(target);
            Debug.DrawLine(firePointLeft.position, target.position);

            if(readyToShoot && canShoot)
            {
                Transform firePoint = isLeft ? firePointLeft : firePointRight;
                weapon.init(null, firePoint, null, null, null);
                ShootWeapon(firePoint);
                isLeft = !isLeft;
                readyToShoot = false;
                Invoke("Cooldown", cooldown);
            }
        }
    }

    private void ShootWeapon(Transform firePoint)
    {
        weapon.BulletFirePoint = firePoint;
        weapon.Shoot(true);
    }

    private void Cooldown()
    {
        readyToShoot = true;
    }

    public void TakeDamage(int damage)
    {
        if(maxHealth >= 0)
        {
            //take damage
            maxHealth -= damage;
            //check if dead
            if(maxHealth <= 0)
            {
                Instantiate(explosionEffect, transform.position, transform.rotation);
                if (explosionSound)
                {
                    var sound = Instantiate(explosionSound); 
                    sound.Play();
                    explosionSound = null;
                }
                Destroy(turret);
            }
        }
    }

    IEnumerator FocusPlayer(PlayerMovement newTarget, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if(newTarget != null)
        {
            target = newTarget.Target;
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log(other.name + " entered turret area");
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement newTarget = other.gameObject.GetComponent<PlayerMovement>();
            StartCoroutine(FocusPlayer(newTarget, 1f));
        }
    }
    private void OnTriggerExit(Collider other) {
        //Debug.Log(other.name + " left turret area");
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }
}
