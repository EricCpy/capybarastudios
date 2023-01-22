using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode;
public class M_AutoDeletion : NetworkBehaviour
{
    public float delay = 2f;

    void FixedUpdate()
    {
        if(IsServer) DestroySource();
    }
    
    private void DestroySource() {
        Destroy(gameObject, delay);
    }
}