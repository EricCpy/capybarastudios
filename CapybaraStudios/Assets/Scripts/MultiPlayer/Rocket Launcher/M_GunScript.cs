using System;
using UnityEngine;
using TMPro;

public class M_GunScript : MonoBehaviour
{
    private Camera _camera;
    public Animator animator;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI maxAmmoText;
    public GameObject hitmarker;
    public M_Weapon currentWeapon;
    Coroutine fireCoroutine;

    private void Start() {
        _camera = M_Camera.Instance._camera;
        currentWeapon.init(animator, _camera.transform, ammoText, maxAmmoText, hitmarker);
    }

    public void Shoot()
    {
        currentWeapon.Shoot();
    }

    public void Reload()
    {
        currentWeapon.Reload();
    }
    public float getReloadStatus()
    {
        return currentWeapon.getReloadStatus();
    }
}