using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class M_SoundDeletion : NetworkBehaviour
{
    public AudioSource audioSource;
    void FixedUpdate()
    {
        if(!audioSource.isPlaying) {
            if(IsServer) DestroySource();
            else DestroySourceServerRpc();
        }
    }

    [ServerRpc]
    private void DestroySourceServerRpc() {
        DestroySource();
    }

    private void DestroySource() {
        Destroy(gameObject);
    }
}
