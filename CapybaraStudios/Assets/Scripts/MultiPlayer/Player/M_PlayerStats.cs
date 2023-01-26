using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.Netcode;

public class M_PlayerStats : NetworkBehaviour
{
    public AudioSource deathSound;
    public AudioSource killSound;
    private int damageTaken = 0;
    public TextMeshProUGUI healthIndicator;
    public int damage_done = 0;
    public int kills = 0;
    private M_Ragdoll ragdoll;

    [SerializeField] private Image healthBar;

    [SerializeField] private GameObject healthObj, nameObj;
    [SerializeField] private float blinkIntensity;
    private float blinkTimer, maxImmunity = 2f;
    [SerializeField] private float currentBlinkDuration = .4f;
    private Volume volume;
    [SerializeField] public int maxHealth = 100;
    private M_HUDcontroller hud;
    private NetworkVariable<int> networkHealth = new (100);
    private NetworkVariable<float> immunity = new ();
    private NetworkVariable<float> blinkDuration = new (0);
    private NetworkVariable<ulong> lastDamager = new (0);
    private bool dead = false;
    private float respawnCount = 0f;
    private PlayerVisuals playerVisuals;
    private Color color;
    private M_Weapon weapon;
    private M_InputManager inputManager;

    //Score Zeug
    [HideInInspector] public NetworkVariable<int> networkDmg = new (0);
    [HideInInspector] public NetworkVariable<int> networkDeaths = new (0);
    [HideInInspector] public NetworkVariable<int> networkKills = new (0);
    [HideInInspector] public NetworkVariable<NetworkString> playerName = new();
    private string killer = "";
    public override void OnNetworkSpawn()
    {
        ScoreManager.Instance.players.Add(OwnerClientId, this);
    }

    void Start()
    {
        if(IsServer || IsHost) {
            networkHealth.Value = maxHealth;
            immunity.Value = maxImmunity;
            lastDamager.Value = OwnerClientId;
        }
        if(IsOwner) {
            healthObj.SetActive(false);
            nameObj.SetActive(false);
        }
        inputManager = GetComponent<M_InputManager>();
        hud = GetComponentInChildren<M_HUDcontroller>();
        ragdoll = GetComponent<M_Ragdoll>();
        volume = M_Camera.Instance._camera.GetComponentInChildren<Volume>();
        playerVisuals = GetComponent<PlayerVisuals>();
        weapon = GetComponentInChildren<M_Weapon>();
        blinkDuration.OnValueChanged += OnBlinkChanged;
        networkHealth.OnValueChanged += OnHealthChanged;
        OnHealthChanged(networkHealth.Value, networkHealth.Value);
    }
    void Update()
    {
        if(dead) {
            if(!IsOwner) return;
            respawnCount -= Time.deltaTime;
            if(respawnCount <= 0f) Respawn();
        } 

        if(blinkTimer > 0) {
            blinkTimer -= Time.deltaTime;
            float intentsity = Mathf.Clamp01(blinkTimer / currentBlinkDuration) * blinkIntensity;
            foreach (var s in playerVisuals.renderers)
            {
                s.material.color = color + Color.white * intentsity;
            }
        }

        if(IsHost || IsServer) {
           if(blinkTimer <= 0) blinkDuration.Value = 0;
           if(immunity.Value > 0) immunity.Value -= Time.deltaTime;
        }
    }

    private void OnHealthChanged(int prev, int next) {
        if(IsOwner) UpdateHealth(next);
        else SetHealthBar();
        if(networkHealth.Value == 0) Die();
    }

    private void OnBlinkChanged(float prev, float next) {
        if(next > 0) {
            blinkTimer = next;
            color = playerVisuals.renderers[0].material.color;
        }
    }

    void SetHealthBar() {
        float percent = networkHealth.Value / (float) maxHealth;
        healthBar.rectTransform.localScale = new Vector3(-percent,1,1);
    }

    void Die() {
        respawnCount = 4f;
        dead = true;
        ragdoll.EnablePhysics();
        if(!IsOwner) {
            healthObj.SetActive(false);
            nameObj.SetActive(false);
        }
        GetComponent<M_WeaponAnimationController>().enabled = false;
        if(IsOwner) {
            inputManager.Die();
            deathSound.Play(); //spiele sterbe sound
            hud.Death(killer, true); //öffne KillHud
        }
    }

    void Respawn() {
        dead = false;
        respawnCount = 0f;
        if(IsOwner) {
            RespawnManager.Instance.SetClientToNewSpawnServerRpc(OwnerClientId);
            RespawnServerRpc();
            hud.Death("", false);
            inputManager.Respawn();
            weapon.bulletsLeft = weapon.magazineSize;
            weapon.ShowAmmo();
            deathSound.Stop();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc() {
        networkHealth.Value = maxHealth;
        immunity.Value = maxImmunity;
        RespawnClientRpc();
    }

    [ClientRpc]
    public void RespawnClientRpc() {
        ragdoll.DeactivatePhysics();
        dead = false;
        GetComponent<M_WeaponAnimationController>().enabled = true;
        if(!IsOwner) {
            healthObj.SetActive(true);
            nameObj.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthServerRpc(int health, ulong clientId, ulong damagerId) {
        // Zusatz:
        //  Spiele Kill Sound für denjenigen, der den Client gekillt hat
        //  Addiere im TabMenü die Kills vom Killer + 1
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<M_PlayerStats>();
        var killer = NetworkManager.Singleton.ConnectedClients[damagerId].PlayerObject.GetComponent<M_PlayerStats>();
        if(client == null || client.networkHealth.Value <= 0 || client.immunity.Value > 0f || killer == null) return;
        
        client.lastDamager.Value = damagerId;
        if(client.networkHealth.Value + health <= 0) {
            KillerClientRpc(damagerId, new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {clientId}}});
            if(clientId != damagerId) {
                killer.networkDmg.Value += client.networkHealth.Value;
                killer.networkKills.Value++;
                KillSoundClientRpc(new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {damagerId}}});
            }
            client.networkHealth.Value = 0;
            client.networkDeaths.Value++;
            return;
        } else if(health < 0) {
            client.blinkDuration.Value = currentBlinkDuration; //change blink timer
        }
        client.networkHealth.Value += health;
        client.networkHealth.Value = Mathf.Min(client.networkHealth.Value, 100);
        if(clientId != damagerId) killer.networkDmg.Value += Mathf.Abs(health);
       
    }

    [ServerRpc(RequireOwnership = false)]
    public void OutOfMapServerRpc(ulong clientId) {
        // Zusatz:
        //  Spiele Kill Sound für denjenigen, der den Client gekillt hat
        //  Addiere im TabMenü die Kills vom Killer + 1
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<M_PlayerStats>();
        if(client == null || client.networkHealth.Value <= 0) return;
        KillerClientRpc(client.lastDamager.Value, new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {clientId}}});
        if(clientId != client.lastDamager.Value) {
            var killer = NetworkManager.Singleton.ConnectedClients[client.lastDamager.Value].PlayerObject.GetComponent<M_PlayerStats>();
            killer.networkKills.Value++;
            KillSoundClientRpc(new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {client.lastDamager.Value}}});
        }
        client.networkHealth.Value = 0;
        client.lastDamager.Value = clientId;
        client.networkDeaths.Value++;
    }


    [ClientRpc]
    public void KillerClientRpc(ulong killerId, ClientRpcParams clientRpcParams) {
        if(killerId == OwnerClientId) {
            killer = "";
            return;
        }
        M_PlayerStats killerStats;
        ScoreManager.Instance.players.TryGetValue(killerId, out killerStats);
        if(killerStats == null) {
            killer = "";
            return;
        }
        killer = killerStats.playerName.Value; 
    }

    [ClientRpc]
    public void KillSoundClientRpc(ClientRpcParams clientRpcParams) {
        Debug.Log("kill" + OwnerClientId);
        killSound.Play();
    }

    public void UpdateHealth(float currentHealth) {
        healthIndicator.SetText("+" + currentHealth); //change hp for client in his hud
        updateVignette(currentHealth); //change vignette in camera
    }

    public void updateVignette(float currentHealth)
    {
        Vignette vignette;
        if (volume.profile.TryGet(out vignette))
        {
            float percent = 0.6f * Math.Max((1.0f - (currentHealth / (float)maxHealth)), 0f);
            vignette.intensity.value = percent;
        }
    }
}