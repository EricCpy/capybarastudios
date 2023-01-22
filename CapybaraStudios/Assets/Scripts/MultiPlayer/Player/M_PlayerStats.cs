using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
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

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private Color color;
    [SerializeField] private float blinkDuration, blinkIntensity;
    private float blinkTimer;
    private Volume volume;

    [SerializeField] private int maxHealth = 100;
    private NetworkVariable<float> networkHealth;
    void Awake()
    {
        skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        color = skinnedMeshRenderers[0].material.color;
        networkHealth = new NetworkVariable<float>(maxHealth);
    }

    void Start()
    {
        volume = M_Camera.Instance._camera.GetComponentInChildren<Volume>();
    }
    
    /*void Update()
    {
        if (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            float intentsity = Mathf.Clamp01(blinkTimer / blinkDuration) * blinkIntensity;
            foreach (var s in skinnedMeshRenderers)
            {
                s.material.color = color + Color.white * intentsity;
            }
        }
    }*/

    [ServerRpc(RequireOwnership = false)]
    public void UpdateHealthServerRpc(int health, ulong clientId) {
        // Zusatz:
        //  Spiele Kill Sound für denjenigen, der den Client gekillt hat
        //  Addiere im TabMenü die Kills vom Killer + 1

        var client = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<M_PlayerStats>();
        if(client == null || client.networkHealth.Value <= 0) return;
        client.networkHealth.Value += health;
        if(client.networkHealth.Value <= 0) {
            client.networkHealth.Value = 0;
            //lasse client sterben
            //sterben = ragdoll + scripts deaktivieren
        } else {
            //change blick timer on this client
        }
        //change hp bar on this client
        

        UpdateHealthClientRpc();
    }

    [ClientRpc]
    public void UpdateHealthClientRpc() {
        //change hp for client in his hud
        //change vignette in camera
        //wenn client hp == 0, dann öffne sterbe hud
        //spiele sterbe sound
    }

    /*public int TakeDamage(int damageAmount)
    {
        if (damageAmount <= 0 || currentHealth <= 0) return 0;
        Debug.Log("Take Damage: " + damageAmount);
        //take damage
        int damage_taken = (currentHealth - damageAmount);
        if (damage_taken < 0)
        {
            damage_taken = 0;
        }

        damage_taken = currentHealth - damage_taken;

        currentHealth -= damageAmount;
        damageTaken += damageAmount;

        blinkTimer = blinkDuration;
        UpdateHealth();
        return damage_taken;
    }

    public void UpdateHealth()
    {
        updateVignette();
        healthIndicator.SetText("+" + currentHealth); //TODO
        if (currentHealth <= 0) die();
    }

    public void die()
    {
        if (dead) return;
        dead = true;
        GetComponent<InputManager>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<PlayerLook>().enabled = false;
        GetComponent<cullHead>().die();
        GetComponent<PlayerInteract>().enabled = false;
        GetComponent<PlayerUI>().enabled = false;
        GetComponent<WeaponAnimationController>().enabled = false;
        GetComponent<HUDcontroller>().Death();

        if (!deathSound.isPlaying && !isAI)
        {
            deathSound.Play();
        }

        if (!killSound.isPlaying && isAI)
        {
            killSound.Play();
        }

        GetComponent<Ragdoll>().EnablePhysics();
    }


    public void Heal(int health)
    {
        StartCoroutine(HealOverTime(health));
        updateVignette();
    }

    public void updateVignette()
    {
        Vignette vignette;
        if (volume.profile.TryGet(out vignette))
        {
            float percent = 0.55f * Math.Max((1.0f - (currentHealth / (float)maxHealth)), 0f);
            vignette.intensity.value = percent;
        }
    }

    private IEnumerator HealOverTime(int health)
    {
        if (health > 0 && currentHealth > 0 && currentHealth < maxHealth)
        {
            UpdateHealth();
            currentHealth += 1;
            UpdateHealth();
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(HealOverTime(health - 1));
        }
    }*/
}