using System;
using UnityEngine;
using TMPro;

public class M_GunScript : MonoBehaviour
{
    public Transform gunSlot;

    public Camera camera;
    public Animator animator;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI maxAmmoText;
    public GameObject hitmarker;
    public WeaponAnimationController weaponAnimator;
    public GameObject[] weapons = new GameObject[3];
    public Weapon currentWeapon;
    public int currentSlot = 0;
    Coroutine fireCoroutine;
    private PlayerLook cameraScript;

    private void Awake()
    {
        cameraScript = GetComponent<PlayerLook>();
        weaponAnimator.refresh();
    }

    //right click events
    public void StartSpecial()
    {
        switch (currentWeapon.specialWeaponType)
        {
            case 0: //normal
                print(currentWeapon.zoom);
                cameraScript.Zoom(currentWeapon.zoom);
                break;
            case 1: //grapling gun
                weapons[currentSlot].GetComponent<GrapplingGun>().Hook();
                break;
            case 2: //sniper
                cameraScript.Zoom(currentWeapon.zoom);
                currentWeapon.ZoomIn();
                break;
            default:
                break;
        }
    }

    public void StopSpecial()
    {
        switch (currentWeapon.specialWeaponType)
        {
            case 0: //normal
                cameraScript.Zoom(0f);
                break;
            case 1: //grapling gun
                weapons[currentSlot].GetComponent<GrapplingGun>().StopHook();
                break;
            case 2: //sniper
                cameraScript.Zoom(0f);
                currentWeapon.ZoomOut();
                break;
            default:
                break;
        }
    }
    public void StopFiring()
    {
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }
    }

    public void StartFiring()
    {
        fireCoroutine = StartCoroutine(currentWeapon.RapidFire());
    }

    public void Reload()
    {
        currentWeapon.Reload();
    }

    public void Shoot()
    {
        currentWeapon.Shoot(true);
    }

    public float getReloadStatus()
    {
        return currentWeapon.getReloadStatus();
    }
}