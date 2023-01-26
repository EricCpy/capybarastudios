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
        if(IsServer) Spawn(from, firePoint.position, firePoint.rotation);
        else SpawnServerRPC(from, firePoint.position, firePoint.rotation);
    }

    private void Spawn(ulong owner, Vector3 pos, Quaternion rot) {
        GameObject rocketInstance = Instantiate(rocket, pos, rot);
        rocketInstance.GetComponent<Rigidbody>().velocity = transform.forward * range;
        rocketInstance.GetComponent<M_Rocket>().owningPlayer = owner; 
        rocketInstance.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void SpawnServerRPC(ulong owner, Vector3 pos, Quaternion rot) {
        Spawn(owner, pos, rot);
    }

    public float GetKnockbackForce()
    {
        return knockbackForce;
    }
}
