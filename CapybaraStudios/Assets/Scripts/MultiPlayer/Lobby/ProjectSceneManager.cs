using UnityEngine;
using My.Core.Singletons;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;

public class ProjectSceneManager : NetworkSingleton<ProjectSceneManager>
{
    public GameObject player;

    public void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
    }

    public void LoadScene(string map, string gameType) {
        Debug.Log("lllaa");
        NetworkManager.SceneManager.LoadScene(map, LoadSceneMode.Single);
    }

    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        if(sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted) {
            Debug.Log(NetworkManager.Singleton.LocalClientId);
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong playerId) {
        var spawn = Instantiate(player);
        spawn.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);
    }

    public void LeaveGame() {
        if(NetworkManager.Singleton != null ) NetworkManager.Singleton.Shutdown();
    }
}