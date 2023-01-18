using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Launcher : MonoBehaviour
{
    public Transform firePoint;
    public Rigidbody rocket;

    public float knockbackForce = 100f;

    public float range = 100f;

    public void Launch()
    {
        Rigidbody rocketInstance = Instantiate(rocket, firePoint.position, firePoint.rotation);
        //rocketInstance.GetComponent<Rigidbody>().AddForce(firePoint.forward * range, ForceMode.Impulse);
        rocketInstance.velocity = transform.forward * range;
    }

    public float GetKnockbackForce()
    {
        return knockbackForce;
    }
}
