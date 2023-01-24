using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class M_Weapon : NetworkBehaviour
{
    private Transform _transform;
    //sounds
    public AudioSource gunSound;
    public AudioSource reloadSound;

    //HUD
    private TextMeshProUGUI _ammoText;
    private TextMeshProUGUI _maxAmmoText;

    public Transform BulletFirePoint;

    //Gun stats
    public float reloadTime, timeBetweenShooting;
    public int magazineSize;
    [HideInInspector] public int bulletsLeft;
    bool reloading, readyToShoot;
    private float reloadStatus = 1;
    private Animator _animator;
    public int animationType;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    public void init(Animator animatior, Transform transform, TextMeshProUGUI ammoText, TextMeshProUGUI maxAmmoText,
        GameObject hitmarker)
    {
        _animator = animatior;
        _transform = transform;
        _ammoText = ammoText;
        _maxAmmoText = maxAmmoText;
    }

    public void Shoot()
    {
        if (!readyToShoot || reloading || bulletsLeft <= 0) return;
        if (_animator != null) _animator.SetTrigger("shoot");
        //rocket launcher
        M_Launcher launcher = GetComponent<M_Launcher>();
        launcher.Launch(OwnerClientId);
        //knockback
        var dir = _transform.position - BulletFirePoint.position;
        var force = Mathf.Clamp(launcher.GetKnockbackForce(), 25f, 200f);
        ImpactReceiver impactReceiver = GetComponentInParent(typeof(ImpactReceiver)) as ImpactReceiver;
        impactReceiver.AddImpact(dir, force);

        //magazine
        bulletsLeft--;
        ShowAmmo();
        gunSound.Play();
        readyToShoot = false;
        if (bulletsLeft > 0) Invoke("ResetShot", timeBetweenShooting);
    }

    //shoot cooldown
    private void ResetShot()
    {
        readyToShoot = true;
    }

    //reload
    public void Reload()
    {
        if (reloading || bulletsLeft == magazineSize) return;
        reloading = true;
        reloadSound.Play();
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        reloadSound.Stop();
        readyToShoot = true;
        reloading = false;
        bulletsLeft = magazineSize;
        ShowAmmo();
    }

    public void ShowAmmo()
    {
        _ammoText.SetText("" + bulletsLeft);
    }
}
