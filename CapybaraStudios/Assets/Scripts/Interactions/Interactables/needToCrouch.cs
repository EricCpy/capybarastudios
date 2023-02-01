using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;


public class needToCrouch : MonoBehaviour
{
    public GameObject theAi;
    private NavMeshAgent currentMeshAgent;
    private InputManager movementScript;
    private GameObject thePlayer;
    public GameObject sleepingText;

    // Start is called before the first frame update
    void Start()
    {
        currentMeshAgent = theAi.GetComponent<NavMeshAgent>();
     
    }

    void OnTriggerStay(Collider other)
    {
        if(!GameManager.gameManager.currentPlayer.GetComponent<InputManager>().CrouchInput) {
            currentMeshAgent.enabled = true;
            //sleepingText.SetActive(false);
        }
    }
}