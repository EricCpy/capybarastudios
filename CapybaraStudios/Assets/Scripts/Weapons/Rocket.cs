using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public AudioSource explosionSound;
    public GameObject explosionEffect;
    public float force = 10f;
    public float radius = 10f;
    public float impactforce = 700f;
    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision other)
    {
        Explode(other.transform.position);
    }

    private void Explode(Vector3 point)
    {
        if (explosionSound)
        {
            var sound = Instantiate(explosionSound); 
            sound.Play();
            explosionSound = null;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        bool selfknocked = false;
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
                if(!selfknocked && stats.gameObject.tag == "Player") {
                    stats.TakeDamage(99);
                    selfknocked = true;
                    GameObject player = stats.gameObject;
                    Vector3 dir = point - player.transform.position;
                    float percentage = 1 - dir.sqrMagnitude / (float) (radius * radius);
                    float currForce = percentage * impactforce;
                    ImpactReceiver receiver = player.GetComponent<ImpactReceiver>();
                    receiver.AddImpact(dir, currForce);
                } else if(stats.gameObject.tag == "Enemy") {
                    stats.TakeDamage(100);
                }
            }
            
        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        
        Destroy(gameObject);
    }
}
