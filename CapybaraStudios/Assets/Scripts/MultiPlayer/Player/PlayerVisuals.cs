using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerVisuals : NetworkBehaviour
{
    private NetworkVariable<Color> _netColor = new();
    private NetworkVariable<NetworkString> _netName = new();
    public SkinnedMeshRenderer[] renderers;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject nameBar;
    [SerializeField] private TextMeshProUGUI nameOverlay;
    private void Awake() {
        _netColor.OnValueChanged += OnValueChanged;
        _netName.OnValueChanged += OnNameChanged;
    }

    private void OnValueChanged(Color prev, Color next) {
        foreach(var renderer in renderers) {
            renderer.material.color = next;
        }
    }

    private void OnNameChanged(NetworkString prev, NetworkString next) {
        nameOverlay.text = _netName.Value;
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner) {
            Color random = Random.ColorHSV(0.5f, 0.75f);
            healthBar.SetActive(false);
            nameBar.SetActive(false);
            ColorServerRpc(random);
            NameServerRpc(PlayerPrefs.GetString("CurrentName", "Player"));
        } else {
            foreach(var renderer in renderers) {
                renderer.material.color = _netColor.Value;
            }
            nameOverlay.text = _netName.Value;
        }
    }

    [ServerRpc]
    private void ColorServerRpc(Color color) {
        _netColor.Value = color;
    }

    [ServerRpc]
    private void NameServerRpc(string name) {
        _netName.Value = name;
        GetComponent<M_PlayerStats>().playerName.Value = name;
    }
}
