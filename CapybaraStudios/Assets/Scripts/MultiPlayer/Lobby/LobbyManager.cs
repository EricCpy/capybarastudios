using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private TMP_InputField maxPlayers;
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private Toggle isPrivate;
    [SerializeField] private TMP_Dropdown map;
    [SerializeField] private TMP_Dropdown gamemode;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject joiningPanel;
    [SerializeField] private GameObject creatingPanel;
    [SerializeField] private GameObject panelHighlight;
    [SerializeField] private GameObject startBtn;
    [SerializeField] private TMP_Text lobbyCodeToCopy;
    [SerializeField] private TMP_Text waitingMenuLobbyName;
    [SerializeField] private TMP_Text playerCountText;

    private Lobby lobby;
    private string playerId;
    private bool started = false;
    private Transform entryContainer;
    private Transform entryTemplate;
    private List<GameObject> entries;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerId = AuthenticationService.Instance.PlayerId;
    }

    public void Awake()
    {
        entryContainer = lobbyList.transform.Find("EntryContainer");
        entryTemplate = entryContainer.Find("EntryTemplate");
        entries = new List<GameObject>();
        entryTemplate.gameObject.SetActive(false);
    }

    public void ValidateLobbyName()
    {
        string text = lobbyName.text;
        if (text == "")
        {
            lobbyName.text = "My Lobby";
        }
    }

    public void ValidateMaxPlayers()
    {
        string text = maxPlayers.text;
        if (text == "")
        {
            maxPlayers.text = "2";
            return;
        }

        int num = int.Parse(text);
        if (num > 10)
        {
            maxPlayers.text = "10";
        }
        else if (num <= 1)
        {
            maxPlayers.text = "2";
        }
    }

    public async void CreateLobby()
    {
        try
        {
            Debug.Log(isPrivate.isOn);
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate.isOn,
                Data = new Dictionary<string, DataObject> {
                    {"g", new DataObject(DataObject.VisibilityOptions.Public, gamemode.options[gamemode.value].text, DataObject.IndexOptions.S1)},
                    {"m", new DataObject(DataObject.VisibilityOptions.Public, map.options[map.value].text, DataObject.IndexOptions.S2)},
                    {"j", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text, int.Parse(maxPlayers.text), options);
            StartCoroutine(UpdateLobbyTimeoutCoroutine(20));
            Debug.Log("Created Lobby " + lobby.Name + " " + lobby.MaxPlayers + " " + gamemode.options[gamemode.value].text + " " + map.options[map.value].text);
            creatingPanel.SetActive(false);
            panelHighlight.SetActive(false);
            waitingPanel.SetActive(true);
            startBtn.SetActive(true);
            UpdateWaitingMenu();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private IEnumerator UpdateLobbyTimeoutCoroutine(float secs)
    {
        var delay = new WaitForSecondsRealtime(secs);
        while (true)
        {
            try
            {
                Lobbies.Instance.SendHeartbeatPingAsync(lobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            yield return delay;
        }
    }

    private IEnumerator UpdateLobbyInfosCoroutine(float secs)
    {
        var delay = new WaitForSecondsRealtime(secs);
        while (true)
        {
            var t = Task.Run(async () => await UpdateLobbyAsync());
            yield return new WaitUntil(() => t.IsCompleted);
            //falls der Host die Lobby geschlossen hat
            if(lobby == null) {
                waitingPanel.SetActive(false);
                joiningPanel.SetActive(true);
                panelHighlight.SetActive(true);
                StopAllCoroutines();
                ListLobbies();
                break;
            }

            if (lobby.HostId == playerId) startBtn.SetActive(true);
            playerCountText.text = lobby.MaxPlayers - lobby.AvailableSlots + "/" + lobby.MaxPlayers;

            if(lobby.Data["j"].Value != "0") {
                playerCountText.text = "Loading Scene...";
                if(!IsLobbyHost()) {
                    RelayController.Instance.JoinRelay(lobby.Data["j"].Value);
                } else {
                    //Check if everyone has connected and then load map
                    Debug.Log(NetworkManager.Singleton.ConnectedClients.Count);
                    if(lobby.Players.Count == NetworkManager.Singleton.ConnectedClients.Count) {
                        ProjectSceneManager.Instance.LoadScene(lobby.Data["m"].Value, lobby.Data["g"].Value);
                        StopAllCoroutines();
                    }
                }
            } 
            yield return delay;
        }
    }

    public bool IsLobbyHost() {
        return lobby.HostId == playerId;
    }

    private async Task UpdateLobbyAsync()
    {
        try
        {
            lobby = await Lobbies.Instance.GetLobbyAsync(lobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            lobby = null;
        }

    }


    public async void ListLobbies()
    {
        entries.ForEach(entry => Destroy(entry));
        entries.Clear();
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 5,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            var templateHeight = 60f;
            var index = 0;
            foreach (Lobby l in response.Results)
            {
                Debug.Log(l.Id + " " + l.Name + " " + l.MaxPlayers);

                var entryTransform = Instantiate(entryTemplate, entryContainer);
                entries.Add(entryTransform.gameObject);
                var entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * index);
                entryTransform.gameObject.SetActive(true);

                entryTransform.Find("Name").GetComponent<TMP_Text>().text = l.Name;
                entryTransform.Find("PlayerNum").GetComponent<TMP_Text>().text = l.MaxPlayers - l.AvailableSlots + "/" + l.MaxPlayers;
                entryTransform.Find("JoinBtn").GetComponent<Button>().onClick.AddListener(delegate { JoinLobby(l.Id); });

                index++;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobby(string lobbyId)
    {
        try
        {
            lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
            joiningPanel.SetActive(false);
            panelHighlight.SetActive(false);
            waitingPanel.SetActive(true);
            UpdateWaitingMenu();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode()
    {
        try
        {
            lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text);
            joiningPanel.SetActive(false);
            panelHighlight.SetActive(false);
            waitingPanel.SetActive(true);
            UpdateWaitingMenu();
        }
        catch (LobbyServiceException e)
        {
            lobbyCodeInput.text = "Not existing!";
            Debug.Log(e);
        }
    }

    public async void LeaveLobbyAsync()
    {
        try
        {
            StopAllCoroutines();
            if (lobby != null)
            {
                await Lobbies.Instance.RemovePlayerAsync(lobby.Id, playerId);
                lobby = null;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
        waitingPanel.SetActive(false);
        joiningPanel.SetActive(true);
    }

    public async void DeleteLobbyAsync() {
        try
        {
            StopAllCoroutines();
            if (lobby != null)
            {
                if (lobby.HostId == playerId) await Lobbies.Instance.DeleteLobbyAsync(lobby.Id);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void UpdateWaitingMenu()
    {
        lobbyCodeToCopy.text = lobby.LobbyCode;
        waitingMenuLobbyName.text = lobby.Name;
        StartCoroutine(UpdateLobbyInfosCoroutine(10));
    }

    public async void Startgame() {
        if(IsLobbyHost() && !started) {
            started = true;
            try{
                string relayCode = await RelayController.Instance.CreateRelay(lobby);
                Debug.Log(relayCode);
                lobby = await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                    {"j", new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }});

            } catch(LobbyServiceException e) {
                Debug.Log(e);
                started = false;
            }
        }
    }
}