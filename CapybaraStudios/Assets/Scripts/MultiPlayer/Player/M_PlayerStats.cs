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
    private Ragdoll ragdoll;

    [SerializeField] private Image healthBar;

    [SerializeField] private GameObject healthObj, nameObj;
    [SerializeField] private float blinkIntensity;
    private float blinkTimer, maxImmunity = 2f;
    [SerializeField] private float currentBlinkDuration = .4f;
    private Volume volume;
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<float> networkHealth = new (100);
    private NetworkVariable<float> immunity = new ();
    private NetworkVariable<float> blinkDuration = new (0);
    private bool dead = false;
    private float respawnCount = 0f;
    private PlayerVisuals playerVisuals;
    private Color color;
    private M_Weapon weapon;

    //Score Zeug
    private NetworkVariable<int> networkDmg = new ();
    private NetworkVariable<int> networkDeaths = new ();
    private NetworkVariable<int> networkKills = new ();

    public override void OnNetworkSpawn()
    {
        ScoreManager.Instance.players.Add(OwnerClientId, this);
    }

    void Start()
    {
        if(IsServer || IsHost) {
            networkHealth.Value = maxHealth;
            immunity.Value = maxImmunity;
        }
        if(IsOwner) {
            healthObj.SetActive(false);
            nameObj.SetActive(false);
        }
        ragdoll = GetComponent<Ragdoll>();
        volume = M_Camera.Instance._camera.GetComponentInChildren<Volume>();
        playerVisuals = GetComponent<PlayerVisuals>();
        weapon = GetComponentInChildren<M_Weapon>();
        blinkDuration.OnValueChanged += OnBlinkChanged;
        networkHealth.OnValueChanged += OnHealthChanged;
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

    private void OnHealthChanged(float prev, float next) {
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
        float percent = networkHealth.Value / maxHealth;
        healthBar.rectTransform.localScale = new Vector3(-percent,1,1);
    }

    void Die() {
        respawnCount = 2f;
        dead = true;
        ragdoll.EnablePhysics();
        if(!IsOwner) {
            healthObj.SetActive(false);
            nameObj.SetActive(false);
        }
        GetComponent<M_WeaponAnimationController>().enabled = false;
        if(IsOwner) {
            GetComponent<M_InputManager>().enabled = false;
            deathSound.Play(); //spiele sterbe sound
            //öffne KillHud
            //GetComponent<M_HUDcontroller>().Death();
        }
    }

    void Respawn() {
        dead = false;
        respawnCount = 0f;
        if(IsOwner) {
            RespawnServerRpc();
            RespawnManager.Instance.SetClientToNewSpawnServerRpc(OwnerClientId);
            GetComponent<M_InputManager>().enabled = true;
            weapon.bulletsLeft = weapon.magazineSize;
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
        if(!IsOwner) {
            healthObj.SetActive(true);
            nameObj.SetActive(true);
        }
        GetComponent<M_WeaponAnimationController>().enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthServerRpc(int health, ulong clientId, ulong damagerId) {
        // Zusatz:
        //  Spiele Kill Sound für denjenigen, der den Client gekillt hat
        //  Addiere im TabMenü die Kills vom Killer + 1
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<M_PlayerStats>();
        if(client == null || client.networkHealth.Value <= 0 || client.immunity.Value > 0f) return;
        client.networkHealth.Value += health;
        client.networkHealth.Value = Mathf.Min(client.networkHealth.Value, 100);
        if(client.networkHealth.Value <= 0) {
            client.networkHealth.Value = 0;
            //Play KillSound for
            if(clientId == damagerId) return;
            KillSoundClientRpc(new ClientRpcParams {Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {damagerId}}});
        } else if(health < 0) {
            client.blinkDuration.Value = currentBlinkDuration; //change blink timer
        }
    }

    [ClientRpc]
    public void KillSoundClientRpc(ClientRpcParams clientRpcParams) {
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