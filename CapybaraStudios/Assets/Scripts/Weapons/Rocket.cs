using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public AudioSource explosionSound;
    //public GameObject explosionEffect;
    public float force = 10f;
    public float radius = 10f;
    
    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        Explode();
    }

    private void Explode()
    {
        if (explosionSound)
        {
            var sound = Instantiate(explosionSound); 
            sound.Play();
            explosionSound = null;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collision in colliders)
        {

            //knockback
            Rigidbody rigb = collision.GetComponent<Rigidbody>();

            if (rigb != null)
            {
                rigb.isKinematic = false;
                rigb.AddExplosionForce(force, transform.position, radius, 1f, ForceMode.Impulse);
            }

            //damage
            if(collision.GetComponentInParent(typeof(PlayerStats)))
            {
                PlayerStats stats = collision.GetComponentInParent<PlayerStats>();
                stats.TakeDamage(100);
            }
            
        }
        //Instantiate(explosionEffect, transform.position, transform.rotation);
        
        Destroy(gameObject);
    }
}
