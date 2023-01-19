using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dungeonDoor : Interactable
{
    public GameObject button;
    public GameObject door;
    private bool open = false;
    public AudioSource buttonSound;
    public int yRotationOpen;
    public int yRotationClose;

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void Interact(GameObject player)
    {
        Debug.Log(player.name + "interacted with " + gameObject.name);
        if(open == false) {
            door.transform.localRotation = Quaternion.Euler(0, yRotationOpen, 0);
            buttonSound.Play();
            open = true;
        }
        else {
            door.transform.localRotation = Quaternion.Euler(0, yRotationClose, 0);
            buttonSound.Play();
            open = false;
        }
    }
}
