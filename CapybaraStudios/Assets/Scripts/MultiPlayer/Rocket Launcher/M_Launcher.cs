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

    public void Launch()
    {
        if(IsServer) Spawn();
        else SpawnServerRPC();
        
    }

    private void Spawn() {
        GameObject rocketInstance = Instantiate(rocket, firePoint.position, firePoint.rotation);
        //rocketInstance.GetComponent<Rigidbody>().AddForce(firePoint.forward * range, ForceMode.Impulse);
        rocketInstance.GetComponent<Rigidbody>().velocity = transform.forward * range;
        rocketInstance.GetComponent<NetworkObject>().Spawn();
        //rocketInstance.GetComponent<NetworkObject>().SpawnWithOwnership();
    }

    [ServerRpc]
    public void SpawnServerRPC() {
        Spawn();
    }

    public float GetKnockbackForce()
    {
        return knockbackForce;
    }
}
