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
    private bool exploded = false;
    public ulong owningPlayer;
    Vector3 oldVelocity = Vector3.zero;
    // Start is called before the first frame update
    void Awake()
    {
        rig = GetComponent<Rigidbody>();
        rig.freezeRotation = true;
    }

    void Start() {
        oldVelocity = rig.velocity;
        if(IsServer) Destroy(gameObject, 30f);
    }

    void OnCollisionEnter(Collision other)
    {   
        M_PlayerStats obj = other.gameObject.GetComponentInParent<M_PlayerStats>();
        if(exploded || (obj != null && obj.OwnerClientId == owningPlayer)) return;
        Explode();
        exploded = true;
    }

    private void FixedUpdate() {
        if(rig.velocity != oldVelocity) rig.velocity = oldVelocity;
    }

    private void Explode()
    {
        if(IsServer) PlayExplosion();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        HashSet<ulong> set = new HashSet<ulong>();
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
                if(set.Add(stats.OwnerClientId) && stats.gameObject.tag == "Player") {
                    KnockbackClientRpc(transform.position, new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {stats.OwnerClientId}}});
                    if(stats.OwnerClientId != owningPlayer) stats.UpdateHealthServerRpc(-49, stats.OwnerClientId, owningPlayer);
                    else stats.UpdateHealthServerRpc(-2, stats.OwnerClientId, owningPlayer);
                }
            }
            
        }
        GameObject oj = Instantiate(explosionEffect, transform.position, transform.rotation);
        oj.GetComponent<NetworkObject>().Spawn();

        if(IsServer) DestroyRocket();
    }

    [ClientRpc]
    private void KnockbackClientRpc(Vector3 point, ClientRpcParams clientRpcParams) {
        GameObject player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        Vector3 position = player.GetComponent<MultiPlayerMovement>().center.position;
        //torso ist leicht verschoben
        Vector3 dir = position - point; 
        float percentage = 1 - dir.sqrMagnitude / (float) (radius * radius);
        float currForce = percentage * impactforce;
        ImpactReceiver receiver = player.GetComponent<ImpactReceiver>();
        receiver.AddImpact(dir, Mathf.Clamp(currForce, impactforce * 0.1f, impactforce));
    }

    private void DestroyRocket() {
        Destroy(gameObject);
    }

    private void PlayExplosion() {
        var sound = Instantiate(explosionSound); 
        sound.GetComponent<NetworkObject>().Spawn();
    }
    
}
