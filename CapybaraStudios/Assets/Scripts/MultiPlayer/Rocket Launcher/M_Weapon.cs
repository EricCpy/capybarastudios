using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class M_Weapon : Interactable
{
    private Transform _transform;

    //sounds
    public AudioSource gunSound;
    public AudioSource reloadSound;
    public AudioSource pickupSound;

    //Gun stats
    public float reloadTime, timeBetweenShooting;
    public int maxAmmo, magazineSize;
    public bool hasAmmo;
    [HideInInspector] public int bulletsLeft, bulletsShot;
    bool reloading, readyToShoot;
    private float reloadStatus = 1;

    //HUD
    private TextMeshProUGUI _ammoText;
    private TextMeshProUGUI _maxAmmoText;

    private WaitForSeconds rapidFireWait;
    private int controllerMask = ~(1 << 15);
    private Animator _animator;
    public int weaponSlot;
    public int animationType;

    private float _inaccuracy = 1f;
    //_inaccuracy for extra ai inaccuracy, player inaccuracy = 0

    public int specialWeaponType;
    // 0 ist für nicht special Weapon
    // 1 ist für Grappling Gun
    // 2 ist für sniper
    // 3 ist für rocket launcher

    public Transform BulletFirePoint;
    private bool _ai;

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

    private void Update()
    {
        if (reloadStatus < 1) reloadStatus += Time.deltaTime / reloadTime;
        else if (reloadStatus > 1) reloadStatus = 1;
    }

    protected override void Interact(GameObject player)
    {
        player.GetComponent<GunScript>().PickUp(gameObject);
        pickupSound.Play();
    }

    public void Shoot(bool first)
    {
        if (reloading)
        {
            cancelReload();
            readyToShoot = false;
            Invoke("ResetShot", timeBetweenShooting);
            return;
        }

        if (!readyToShoot || reloading || bulletsLeft <= 0) return;

        if (_animator != null) _animator.SetTrigger("shoot");
        readyToShoot = false;

        
        //rocket launcher
        Launcher launcher = GetComponent<Launcher>();
        launcher.Launch();
        //knockback
        var dir = transform.parent.transform.position - BulletFirePoint.position;
        var force = Mathf.Clamp(launcher.GetKnockbackForce(), 25f, 200f);
        ImpactReceiver impactReceiver = GetComponentInParent(typeof(ImpactReceiver)) as ImpactReceiver;
        impactReceiver.AddImpact(dir, force);
        //


        //magazine
        bulletsLeft--;
        bulletsShot--;
        ShowAmmo();
        gunSound.Play();
        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            readyToShoot = true;
            Shoot(false);
            return;
        }

        Invoke("ResetShot", timeBetweenShooting);
    }

    //shoot cooldown
    private void ResetShot()
    {
        readyToShoot = true;
    }

    //reload
    public void Reload()
    {
        if (bulletsLeft.Equals(magazineSize) || maxAmmo <= 0 || reloadStatus < 1) return;

        reloading = true;
        readyToShoot = true;
        reloadSound.Play();
        reloadStatus = 0;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        if (reloadSound)
        {
            reloadSound.Stop();
        }

        pickupSound.Play();

        if ((maxAmmo + bulletsLeft) < magazineSize)
        {
            bulletsLeft = maxAmmo + bulletsLeft;
            maxAmmo = 0;
        }
        else
        {
            maxAmmo -= magazineSize - bulletsLeft;
            bulletsLeft = magazineSize;
        }

        ShowAmmo();

        reloading = false;
    }

    public void ShowAmmo()
    {
        if (!_ammoText || !_maxAmmoText) return;
        if (!hasAmmo)
        {
            _ammoText.SetText("∞");
            _maxAmmoText.SetText("");
        }
        else
        {
            _ammoText.SetText((bulletsLeft) + " / " + (magazineSize));
            if (maxAmmo > 0)
                _maxAmmoText.SetText((maxAmmo).ToString());
            else
                _maxAmmoText.SetText("0");
        }
    }

    public float getReloadStatus()
    {
        return reloadStatus;
    }

    public void cancelReload()
    {
        if (reloadSound)
        {
            reloadSound.Stop();
        }

        reloading = false;
        readyToShoot = true;
        reloadStatus = 1;
        CancelInvoke("ReloadFinished");
    }
}
