using UnityEngine;
using My.Core.Singletons;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class ProjectSceneManager : NetworkSingleton<ProjectSceneManager>
{
    public GameObject player;
    public GameObject endHud;
    public GameObject camera;
    [HideInInspector] public string playerId;
    [HideInInspector] public GameType gametype; 
    public bool inGame = false;
    private async void Awake() {
        DontDestroyOnLoad(gameObject);
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () => {
            playerId = AuthenticationService.Instance.PlayerId;
            SceneManager.LoadScene("Menu_Scene");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void FixedUpdate() {
        if(NetworkManager.Singleton == null) return;
        //wenn isClient und inGame, dann öffne ... menü falls networkmanager nicht mehr kommuniziert
        //wenn InGame und IsListening sich auf false setzt, dann öffne Hud zum Disconnecten
        if(!inGame && NetworkManager.Singleton.IsListening) {
            inGame = true;
        } else if(inGame && !NetworkManager.Singleton.IsListening) {
            inGame = false;
            Instantiate(camera); //Kamera wird gelöscht, weil auf NetworkObject -> spawne neue
            Instantiate(endHud); //öffne End Hud
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
    }

    public void LoadScene(string map, string game) {
        if(game == "Deathmatch") {
            gametype = GameType.DeathMatch;
        } else {
            gametype = GameType.LastOneStanding;
        }
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
        spawn.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId, true);
    }

    public void LeaveGame() {
        if(NetworkManager.Singleton != null ) NetworkManager.Singleton.Shutdown();
    }
}