using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations.Rigging;

public class GunScript : MonoBehaviour
{
    public Transform gunSlot;
    public TwoBoneIKConstraint rightTarget;
    public TwoBoneIKConstraint leftTarget;
    public RigBuilder rigBuilder;


    public float damage = 10;
    public float range = 100f;
    public int ammo = 30;

    public new Camera camera;

    public GameObject hitmarker;
    public float distance;

    //HUD
    public TextMeshProUGUI ammoText;

    private int controllerMask = ~(1 << 15);

    void Update()
    {
    }

    public void Shoot()
    {
        //Debug.Log("Shoot!");

        ammo--;
        ammoText.text = ammo.ToString() + " / 30";
        if (ammo == 0)
        {
            ammo = 30;
            ammoText.text = ammo.ToString() + " / 30";
        }

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, range,
                controllerMask))
        {
            Debug.Log(hit.transform.name);

            GameObject collisionObject = hit.collider.gameObject;

            if (collisionObject.CompareTag("Head") || collisionObject.CompareTag("Body") ||
                collisionObject.CompareTag("Limbs"))
            {
                //does object have stats?
                if (collisionObject.GetComponent<PlayerStats>() == null) return;
                //deal damage
                float hitMultiplier = 1;
                if (collisionObject.CompareTag("Head")) hitMultiplier = 3;
                if (collisionObject.CompareTag("Limbs")) hitMultiplier = 0.75f;
                int finalDamage = (int)(damage * hitMultiplier);
                collisionObject.GetComponent<PlayerStats>().TakeDamage(finalDamage);

                //Hitmarker
                HitShow();
                Invoke(nameof(HitDisable), 0.2f);
            }
        }
        else
        {
            Debug.Log("Not hit");
        }
    }


    public void HitShow()
    {
        hitmarker.SetActive(true);
    }

    public void HitDisable()
    {
        hitmarker.SetActive(false);
    }

    public void pickUp(GameObject gun)
    {
        Debug.Log(gun.name + " aquired");
        gun.GetComponent<Rigidbody>().isKinematic = true;
        gun.GetComponent<BoxCollider>().enabled = false;
        gun.transform.SetParent(gunSlot);
        leftTarget.data.target = gun.transform.Find("ref_left_hand_target");
        rigBuilder.Build();
        gun.transform.localRotation = Quaternion.Euler(0, 0, 0);
        gun.transform.localPosition = new Vector3(0, 0, 0);
    }
}