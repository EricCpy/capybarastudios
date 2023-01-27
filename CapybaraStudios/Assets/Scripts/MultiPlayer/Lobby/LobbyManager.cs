using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyName;
    [SerializeField] private TMP_InputField maxPlayers;

    [SerializeField] private GameObject lobbyList;

    // Start is called before the first frame update
    private Lobby lobby;
    private string playerId;


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

    public void ValidateLobbyName(string text)
    {
        if (text == "")
        {
            lobbyName.text = "My Lobby";
        }
    }

    public void ValidateMaxPlayers(string text)
    {
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
        else if (num == 0)
        {
            maxPlayers.text = "2";
        }
    }

    public async void CreateLobby()
    {
        try
        {
            lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName.text, int.Parse(maxPlayers.text));
            StartCoroutine(UpdateLobbyTimeoutCoroutine(15));
            //hier muss auch map etc rein
            Debug.Log("Created Lobby " + lobby.Name + " " + lobby.MaxPlayers);
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
            Lobbies.Instance.SendHeartbeatPingAsync(lobby.Id);
            yield return delay;
        }
    }

    public async void ListLobbies()
    {
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

            var templateHeight = 40f;
            var index = 0;
            foreach (Lobby lobby in response.Results)
            {
                Debug.Log(lobby.Id + " " + lobby.Name + " " + lobby.MaxPlayers);


                var entryTransform = Instantiate(entryTemplate, entryContainer);
                entries.Add(entryTransform.gameObject);
                var entryRectTransform = entryTransform.GetComponent<RectTransform>();
                entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * index);
                entryTransform.gameObject.SetActive(true);

                entryTransform.Find("Name").GetComponent<TMP_Text>().text = lobby.Name;
                entryTransform.Find("PlayerNum").GetComponent<TMP_Text>().text = lobby.AvailableSlots+"/"+lobby.MaxPlayers;
                //entryTransform.Find("JoinBtn") = ;
                
                index++;
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    /*public async JoinLobby() {
        await Lobbies.Instance.JoinLobbyByIdAsync();
    }*/

    private void OnDestroy()
    {
        LeaveLobby();
    }

    public void LeaveLobby()
    {
        try
        {
            StopAllCoroutines();
            if (lobby != null)
            {
                if (lobby.HostId == playerId) Lobbies.Instance.DeleteLobbyAsync(lobby.Id);
                else Lobbies.Instance.RemovePlayerAsync(lobby.Id, playerId);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}