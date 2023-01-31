using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class heliEnter : Interactable
{
    public AudioSource noFuelSound;
    public AudioSource loadFuelSound;
    public GameObject button;
    private GunScript currentGunScript;
    public Weapon currentGun;

    public heliEnter() {
        message = "enter helicopter";
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected override void Interact(GameObject player)
    {
        currentGunScript = player.GetComponent<GunScript>();
        currentGun = currentGunScript.currentWeapon;

        if(currentGunScript.currentWeapon.name == "Fuel Tank") {
            FindObjectOfType<GameManager>().CompleteLevel();
            loadFuelSound.Play();
            SceneManager.LoadScene("AntarcticEndScene");
        }

        else {
            message = "no fuel... find the tank!";
            noFuelSound.Play();
        }
    }
}