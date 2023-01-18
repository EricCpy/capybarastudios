using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Rocket : MonoBehaviour
{
    //public GameObject explosionEffect;
    public float force = 10f;
    public float radius = 10f;

    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
            Explode();
    }

    private void Explode()
    {
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
