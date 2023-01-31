using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class OutOfMap : MonoBehaviour
{
    private void OnTriggerExit(Collider other) {
        var player = other.GetComponent<M_PlayerStats>();
        if(player != null) {
            player.OutOfMapServerRpc(player.OwnerClientId);
        }    
    }
}
