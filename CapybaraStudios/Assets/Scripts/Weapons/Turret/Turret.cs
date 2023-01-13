using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform target;
    public Transform body;
    public Transform firePointLeft;
    public Transform firePointRight;

    private void Update() 
    {
        if (target != null)
        {
            Aim();
            Debug.DrawLine(firePointLeft.position, target.position);

            //Shooting
        }
    }

    private void Aim()
    {
        float targetPlaneAngle = vector3AngleOnPlane(target.position, transform.position, -body.transform.up, body.transform.forward);
        Vector3 newRotation = new Vector3(0, targetPlaneAngle, 0);
        body.transform.Rotate(newRotation, Space.Self);
    }

    float vector3AngleOnPlane(Vector3 from, Vector3 to, Vector3 planeNormal, Vector3 toZeroAngle)
    {
        Vector3 projectedVector = Vector3.ProjectOnPlane(from - to, planeNormal);
        float projectedVectorAngle = Vector3.SignedAngle(projectedVector, toZeroAngle, planeNormal);

        return projectedVectorAngle;
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.name + " entered turret area");
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerMovement newTarget = other.gameObject.GetComponent<PlayerMovement>();
            if(newTarget != null)
            {
                target = newTarget.Target;
            }
            
        }
    }
    private void OnTriggerExit(Collider other) {
        Debug.Log(other.name + " left turret area");
        if (other.gameObject.CompareTag("Player"))
        {
            target = null;
        }
    }
}
