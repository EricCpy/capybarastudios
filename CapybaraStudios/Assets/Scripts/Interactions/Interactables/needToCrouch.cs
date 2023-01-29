using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class needToCrouch : MonoBehaviour
{
    public GameObject theAi;
    private NavMeshAgent currentMeshAgent;
    private PlayerMovement movementScript;
    private GameObject thePlayer;
    public GameObject sleepingText;

    // Start is called before the first frame update
    void Start()
    {
        currentMeshAgent = theAi.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {       
        thePlayer = other.gameObject.transform.root.gameObject;
        movementScript = thePlayer.GetComponent<PlayerMovement>();
        if(!movementScript.crouching) {
            currentMeshAgent.enabled = true;
            sleepingText.SetActive(false);
        }
    }
}