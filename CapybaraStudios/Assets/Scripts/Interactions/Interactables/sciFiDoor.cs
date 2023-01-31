using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sciFiDoor : Interactable
{
    public GameObject button;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public AudioSource doorSound;
    private bool open = false;

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
        doorSound.Play();
        Debug.Log(player.name + "interacted with " + gameObject.name);
        var buttonRenderer = button.GetComponent<Renderer>();
        if(!open) {
            leftDoor.transform.localPosition = new Vector3(0, 0, 4);
            rightDoor.transform.localPosition = new Vector3(0, 0, -4);
            open = true;
        }
        else {
            leftDoor.transform.localPosition = new Vector3(0, 0, 0);
            rightDoor.transform.localPosition = new Vector3(0.007935028f, -0.1499473f, -1.433132f);
            open = false;
        }
    }
}
