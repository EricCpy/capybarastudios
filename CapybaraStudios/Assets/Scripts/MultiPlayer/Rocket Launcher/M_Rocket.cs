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
            if(IsServer) PlayExplosion();
            else PlayExplosionServerRPC();
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collision in colliders)
        {
            //knockback on not player objects
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
        GameObject oj = Instantiate(explosionEffect, transform.position, transform.rotation);
        oj.GetComponent<NetworkObject>().Spawn();

        if(IsServer) DestroyRocket();
        else DestroyRocketServerRPC();
    }

    [ServerRpc]
    private void DestroyRocketServerRPC() {
        DestroyRocket();
    }

    private void DestroyRocket() {
        Destroy(gameObject);
    }

    [ServerRpc]
    private void PlayExplosionServerRPC() {
        PlayExplosion();
    }

    private void PlayExplosion() {
        var sound = Instantiate(explosionSound); 
        sound.GetComponent<NetworkObject>().Spawn();
        sound.Play();
    }

}
