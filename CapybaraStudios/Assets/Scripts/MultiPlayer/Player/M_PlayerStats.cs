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
    private float currentHealth, respawnCount = 0f;
    private PlayerVisuals playerVisuals;
    private Color color;
    void Start()
    {
        if(IsServer || IsHost) {
            networkHealth.Value = maxHealth;
            immunity.Value = maxImmunity;
        }
        ragdoll = GetComponent<Ragdoll>();
        volume = M_Camera.Instance._camera.GetComponentInChildren<Volume>();
        playerVisuals = GetComponent<PlayerVisuals>();
        blinkDuration.OnValueChanged += OnBlinkChanged;
    }
    void Update()
    {
        if(dead) {
            if(IsOwner) {
                respawnCount -= Time.deltaTime;
            }
            if(respawnCount > 0f) return;
            Respawn();
        } 
        if(networkHealth.Value != currentHealth) {
            if(IsOwner) UpdateHealth();
            SetHealthBar();
        }
        if(networkHealth.Value == 0) {
            Die();
            return;
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
        if(IsOwner) {
            GetComponent<M_InputManager>().enabled = false;
            GetComponent<M_WeaponAnimationController>().enabled = false;
            GetComponentInChildren<Animator>().enabled = false;
            deathSound.Play(); //spiele sterbe sound
            //öffne KillHud
            //GetComponent<M_HUDcontroller>().Death();
        }
        respawnCount = 2f;
        dead = true;
        ragdoll.EnablePhysics();
        healthObj.SetActive(false);
        nameObj.SetActive(false);
    }

    void Respawn() {
        if(IsOwner) {
            RespawnManager.Instance.SetClientToNewSpawnServerRpc(OwnerClientId);
            GetComponent<M_InputManager>().enabled = false;
            GetComponent<M_WeaponAnimationController>().enabled = false;
            GetComponentInChildren<Animator>().enabled = false;
            deathSound.Stop();
        }
        dead = false;
        respawnCount = 0f;
        ragdoll.DeactivatePhysics();
        healthObj.SetActive(true);
        nameObj.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthServerRpc(int health, ulong clientId) {
        // Zusatz:
        //  Spiele Kill Sound für denjenigen, der den Client gekillt hat
        //  Addiere im TabMenü die Kills vom Killer + 1
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<M_PlayerStats>();
        if(client == null || client.networkHealth.Value <= 0 || client.immunity.Value > 0f) return;
        client.networkHealth.Value += health;
        client.networkHealth.Value = Mathf.Min(client.networkHealth.Value, 100);
        if(client.networkHealth.Value <= 0) {
            client.networkHealth.Value = 0;
        } else {
            client.blinkDuration.Value = currentBlinkDuration; //change blink timer
        }
    }

    public void UpdateHealth() {
        currentHealth = networkHealth.Value;
        healthIndicator.SetText("+" + currentHealth); //change hp for client in his hud
        updateVignette(); //change vignette in camera
    }

    public void updateVignette()
    {
        Vignette vignette;
        if (volume.profile.TryGet(out vignette))
        {
            float percent = 0.6f * Math.Max((1.0f - (currentHealth / (float)maxHealth)), 0f);
            vignette.intensity.value = percent;
        }
    }
}