using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialText : MonoBehaviour
{
    public GameObject text;
    public AudioSource interactedSound;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        interactedSound.Play();
        text.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }
}
