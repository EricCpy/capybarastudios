using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class woodenDoor : Interactable
{
    public GameObject button;
    public GameObject door;
    public AudioSource buttonSound;
    public AudioSource unlockedSound;
    private GunScript currentGunScript;
    public Weapon currentGun;

    public woodenDoor() {
        message = "open door";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact(GameObject player)
    {
        currentGunScript = player.GetComponent<GunScript>();
        currentGun = currentGunScript.currentWeapon;

        if(currentGunScript.currentWeapon.name == "keyItem") {
            door.transform.localRotation = Quaternion.Euler(0, 0, 0);
            button.transform.localPosition = new Vector3(833, -78, 170);
            unlockedSound.Play();
        }
        else {
            message = "locked... find the keys!";
            buttonSound.Play();
        }
    }
}
