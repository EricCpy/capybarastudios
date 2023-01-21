using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class M_Rocket : NetworkBehaviour
{
    public AudioSource explosionSound;
    public GameObject explosionEffect;
    public float force = 10f;
    public float radius = 10f;
    public float impactforce = 400f;
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
            if(IsServer) PlayExplosion();
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        HashSet<int> set = new HashSet<int>();
        foreach (Collider collision in colliders)
        {
            //knockback on not player objects
            Rigidbody rigb = collision.GetComponent<Rigidbody>();

            if (rigb != null)
            {
                rigb.isKinematic = false;
                rigb.AddExplosionForce(force, transform.position, radius, 1f, ForceMode.Impulse);
            }

            M_PlayerStats stats = collision.GetComponentInParent<M_PlayerStats>();
            //damage
            if(stats != null)
            {
                if(!set.Contains(((int)stats.OwnerClientId)) && stats.gameObject.tag == "Player") {
                    //stats.TakeDamage(99);
                    set.Add(((int)stats.OwnerClientId));
                    GameObject player = stats.gameObject;
                    Vector3 dir = point - player.transform.position;
                    float percentage = 1 - dir.sqrMagnitude / (float) (radius * radius);
                    float currForce = percentage * impactforce;
                    ImpactReceiver receiver = player.GetComponent<ImpactReceiver>();
                    receiver.AddImpact(dir, Mathf.Clamp(currForce, 25f, impactforce));
                }
            }
            
        }
        GameObject oj = Instantiate(explosionEffect, transform.position, transform.rotation);
        oj.GetComponent<NetworkObject>().Spawn();

        if(IsServer) DestroyRocket();
    }

    private void DestroyRocket() {
        Destroy(gameObject);
    }

    private void PlayExplosion() {
        var sound = Instantiate(explosionSound); 
        sound.GetComponent<NetworkObject>().Spawn();
        sound.Play();
    }

}
