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
    public float impactforce = 700f;

    Rigidbody rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        Invoke("Explode", delay);
        Destroy(gameObject, 30f);
    }

    void OnCollisionEnter(Collision collision)
    {
        impactSound.Play();
    }

    private void Explode(Vector3 point)
    {
        if(!explosionSound.isPlaying) {
            var sound = Instantiate(explosionSound); 
            sound.Play();
            Destroy(sound.gameObject, 10f);
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
            if(collision.GetComponentInParent(typeof(Turret)))
            {
                Turret stats = collision.GetComponentInParent<Turret>();
                stats.TakeDamage(100);
            }
            
        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
