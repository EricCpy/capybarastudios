using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class heliEnter : Interactable
{
    public GameObject button;
    public AudioSource unlockedSound;
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
        // if(currentGunScript.currentWeapon.name == "keyItem") {
            FindObjectOfType<GameManager>().CompleteLevel();
            unlockedSound.Play();
            SceneManager.LoadScene(4);
        // }
    }
}