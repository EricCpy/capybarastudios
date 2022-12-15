using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAttackPlayerState : AIState
{
    private Vector3 oldPlayerPos = Vector3.zero;
    private float currentTime = 0f; 
    private Quaternion currRotation;
    private Quaternion lookRotation;
    public void Enter(AIAgent agent)
    {
        currRotation = agent.transform.rotation;
        currentTime = agent.config.rotationSeconds;
    }

    public void Exit(AIAgent agent)
    {
      
    }

    public AIStateId GetId()
    {
       return AIStateId.AttackPlayer;
    }

    public void Update(AIAgent agent)
    {
        //wenn player out of range geht, dann wechsle zum chase modus
        if(!oldPlayerPos.Equals(agent.player.position)) {
            currentTime = 0f;
            Vector3 targetDir = agent.player.position - agent.transform.position;
            lookRotation = Quaternion.LookRotation(targetDir);
            oldPlayerPos = agent.player.position;
        }

        if(currentTime < agent.config.rotationSeconds) {
            currentTime += Time.deltaTime;
            float percentage = currentTime / agent.config.rotationSeconds;
            currRotation = Quaternion.Lerp(currRotation, lookRotation, Mathf.SmoothStep(0, 1, percentage));
            Vector3 rotation = currRotation.eulerAngles;
            agent.rotationTarget.localRotation = Quaternion.Euler(rotation.x, 0f,0f);
            agent.transform.rotation = Quaternion.Euler(0f,rotation.y,0f);
        }
        
    }
}
