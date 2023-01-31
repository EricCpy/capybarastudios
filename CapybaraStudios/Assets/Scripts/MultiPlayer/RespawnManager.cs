using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Core.Singletons;
using Unity.Netcode;
public class RespawnManager : NetworkSingleton<RespawnManager>
{
    [SerializeField] private Transform[] spawns;

    private NetworkVariable<int> currentSpawn = new NetworkVariable<int>(0); 
    
    [ServerRpc(RequireOwnership = false)]
    public void SetClientToNewSpawnServerRpc(ulong clientId) {
        Debug.Log(currentSpawn.Value);
        Transform t = spawns[currentSpawn.Value++ % spawns.Length];
        ChangePositionClientRpc(t.position, new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {clientId}}});
    }

    [ClientRpc]
    private void ChangePositionClientRpc(Vector3 pos, ClientRpcParams clientRpcParams) {
        NetworkManager.Singleton.LocalClient.PlayerObject.gameObject.transform.position = pos;
    }
}
