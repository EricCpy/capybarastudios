using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBody : MonoBehaviour
{
    public Transform target;

    private void Update() 
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, this.transform.position.y, target.position.z);
            this.transform.LookAt(targetPosition);
        }
    }

    IEnumerator FocusPlayer(PlayerMovement newTarget, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if(newTarget != null)
        {
            target = newTarget.Target;
        }
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log(other.name + " entered turret area");
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement newTarget = other.gameObject.GetComponent<PlayerMovement>();
            StartCoroutine(FocusPlayer(newTarget, 1f));
        }
    }
    private void OnTriggerExit(Collider other) {
        //Debug.Log(other.name + " left turret area");
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }
}
