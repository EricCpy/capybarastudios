using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class M_Launcher : NetworkBehaviour
{
    public Transform firePoint;
    public GameObject rocket;

    public float knockbackForce = 100f;

    public float range = 100f;

    public void Launch(ulong from)
    {
        if(IsServer) Spawn(from);
        else SpawnServerRPC(from);
        
    }

    private void Spawn(ulong owner) {
        GameObject rocketInstance = Instantiate(rocket, firePoint.position, firePoint.rotation);
        //rocketInstance.GetComponent<Rigidbody>().AddForce(firePoint.forward * range, ForceMode.Impulse);
        rocketInstance.GetComponent<NetworkObject>().Spawn();
        rocketInstance.GetComponent<Rigidbody>().velocity = transform.forward * range;
        rocketInstance.GetComponent<M_Rocket>().owningPlayer = owner; 
        //rocketInstance.GetComponent<NetworkObject>().SpawnWithOwnership();
    }

    [ServerRpc]
    public void SpawnServerRPC(ulong owner) {
        Spawn(owner);
    }

    public float GetKnockbackForce()
    {
        return knockbackForce;
    }
}
