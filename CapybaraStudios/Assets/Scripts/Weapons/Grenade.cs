using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public AudioSource impactSound;
    public AudioSource explosionSound;

    public GameObject explosionEffect;
    public float delay = 1f;
    public float force = 10f;
    public float radius = 10f;

    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        Invoke("Explode", delay);
    }

    void OnCollisionEnter(Collision collision)
    {
        impactSound.Play();
    }

    private void Explode()
    {
        if(!explosionSound.isPlaying) {
            var sound = Instantiate(explosionSound); 
            sound.Play();
            Destroy(sound.gameObject, 10f);
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
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
