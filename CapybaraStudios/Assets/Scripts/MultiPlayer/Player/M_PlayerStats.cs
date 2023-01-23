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

    //private Ragdoll ragdoll;


    [SerializeField] private Image healthBar;

    [SerializeField] private GameObject healthObj, nameObj;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private Color color;
    [SerializeField] private float blinkIntensity;
    private float blinkTimer, maxImmunity = 2f;
    private Volume volume;
    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<float> networkHealth = new (100);
    private NetworkVariable<float> respawnTime = new (0);
    private NetworkVariable<float> immunity = new ();
    private NetworkVariable<float> blinkDuration = new (0);
    private bool dead = false;
    private float currentHealth;
    void Start()
    {
        if(IsServer || IsHost) {
            networkHealth.Value = maxHealth;
            immunity.Value = maxImmunity;
        }
        volume = M_Camera.Instance._camera.GetComponentInChildren<Volume>();
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        color = skinnedMeshRenderers[0].material.color;
    }

    void Update()
    {
        Debug.Log(networkHealth.Value);
        if(dead) {
            if(networkHealth.Value == 0) return;
            Respawn();
        } 

        if(IsOwner) {
            if(networkHealth.Value != currentHealth) {
                UpdateHealth();
            }
        }
        Debug.Log("2: " + networkHealth.Value);
        if(networkHealth.Value == 0) {
            Die();
            return;
        }

        if (blinkDuration.Value > 0)
        {
            if(blinkTimer <= 0) blinkTimer = blinkDuration.Value;
            Debug.Log(blinkDuration.Value);
            blinkTimer -= Time.deltaTime;
            float intentsity = Mathf.Clamp01(blinkTimer / blinkDuration.Value) * blinkIntensity;
            foreach (var s in skinnedMeshRenderers)
            {
                s.material.color = color + Color.white * intentsity;
            }
        }

        if(IsHost || IsServer) {
           if(blinkTimer <= 0) blinkDuration.Value = 0;
           if(immunity.Value > 0) immunity.Value -= Time.deltaTime;
        }
        Debug.Log("3: " + networkHealth.Value);
    }

    void Die() {
        dead = true;
        GetComponent<Ragdoll>().EnablePhysics();
        healthObj.SetActive(false);
        nameObj.SetActive(false);
    }

    void Respawn() {
        dead = false;
        GetComponent<Ragdoll>().DeactivatePhysics();
        healthObj.SetActive(true);
        nameObj.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthServerRpc(int health, ulong clientId) {
        // Zusatz:
        //  Spiele Kill Sound für denjenigen, der den Client gekillt hat
        //  Addiere im TabMenü die Kills vom Killer + 1
        Debug.Log(networkHealth.Value);
        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<M_PlayerStats>();
        if(client == null || client.networkHealth.Value <= 0 || client.respawnTime.Value > 0f || client.immunity.Value > 0f) return;
        client.networkHealth.Value += health;
        client.networkHealth.Value = Mathf.Min(health, 100);
        if(client.networkHealth.Value <= 0) {
            client.networkHealth.Value = 0;
            respawnTime.Value = 2f; //setze respawn timer
        } else {
            client.blinkDuration.Value = .4f; //change blink timer
        }
        //change hp bar on this client
    }

    public void UpdateHealth() {
        Debug.Log(networkHealth.Value);
        currentHealth = networkHealth.Value;
        healthIndicator.SetText("+" + currentHealth); //change hp for client in his hud
        updateVignette(); //change vignette in camera
        if(currentHealth == 0) {
            //deathSound.Play(); //spiele sterbe sound
            GetComponent<M_InputManager>().enabled = false;
            GetComponent<M_WeaponAnimationController>().enabled = false;
            GetComponentInChildren<Animator>().enabled = false;
            //öffne KillHud
            //GetComponent<M_HUDcontroller>().Death();
        }
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