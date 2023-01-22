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
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        if(IsServer) Destroy(gameObject, 30f);
    }


    void OnCollisionEnter(Collision other)
    {
        if(exploded) return;
        Explode();
        exploded = true;
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
                if(set.Add((stats.OwnerClientId)) && stats.gameObject.tag == "Player") {
                    Debug.Log("lll");
                    KClientRpc(transform.position, new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {stats.OwnerClientId}}});
                }
            }
            
        }
        GameObject oj = Instantiate(explosionEffect, transform.position, transform.rotation);
        oj.GetComponent<NetworkObject>().Spawn();

        if(IsServer) DestroyRocket();
    }

    [ClientRpc]
    private void KClientRpc(Vector3 point, ClientRpcParams clientRpcParams) {
        Debug.Log(clientRpcParams.Send.TargetClientIds);
        GameObject player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        Vector3 position = player.GetComponent<MultiPlayerMovement>().center.position;
        //torso ist leicht verschoben
        Debug.Log("pos of player: " + position);
        Debug.Log("pos of hit: " + point);
        Vector3 dir = position - point; 
        Debug.Log("dir: " + dir.normalized);
        Debug.DrawLine(position, point, Color.blue, 60f);
        Debug.DrawRay(point, dir.normalized * 30f, Color.red, 40f);
        float percentage = 1 - dir.sqrMagnitude / (float) (radius * radius);
        float currForce = percentage * impactforce;
        ImpactReceiver receiver = player.GetComponent<ImpactReceiver>();
        receiver.AddImpact(dir, Mathf.Clamp(currForce, impactforce / 4f, impactforce));
    }

    private void DestroyRocket() {
        Destroy(gameObject);
    }

    private void PlayExplosion() {
        var sound = Instantiate(explosionSound); 
        sound.GetComponent<NetworkObject>().Spawn();
    }
    
}
