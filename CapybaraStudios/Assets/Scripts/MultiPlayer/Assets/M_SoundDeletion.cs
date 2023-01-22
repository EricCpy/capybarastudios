using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class M_SoundDeletion : NetworkBehaviour
{
    public AudioSource audioSource;
    void FixedUpdate()
    {
        if(!audioSource.isPlaying) {;
            if(IsServer) DestroySource();
        }
    }
    
    private void DestroySource() {
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
