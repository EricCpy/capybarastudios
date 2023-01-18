using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WeaponMultiplayer : Interactable
{

    private Transform _transform;

    //sounds
    public AudioSource gunSound;
    public AudioSource reloadSound;
    public AudioSource pickupSound;
    public AudioSource scopeSound;

    //Gun stats
    public int damage;

    public float initialSpread,
        reloadTime,
        fireRate,
        timeBetweenShooting;
        

    public bool singleReload;
    private float singleReloadTime;

    public int maxAmmo, magazineSize, bulletsPerTap;
    public bool hasAmmo, rapidFireEnabled;
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

    //Bullet Trail
    public Transform BulletFirePoint;
    public TrailRenderer BulletTrail;
    public float zoom = 1f;
    private bool _ai;

    private void Awake()
    {
        singleReloadTime = reloadTime / ((float)magazineSize / bulletsPerTap);
        bulletsLeft = magazineSize;
        readyToShoot = true;
        bulletsShot = bulletsPerTap;
        rapidFireWait = new WaitForSeconds(1 / fireRate);
        message = "Pick up [E]";
    }

    public void init(Animator animatior, Transform transform, TextMeshProUGUI ammoText, TextMeshProUGUI maxAmmoText,
        GameObject hitmarker, float inaccuracy = 1f, bool ai = false)
    {
        _animator = animatior;
        _transform = transform;
        _ammoText = ammoText;
        _maxAmmoText = maxAmmoText;
        _inaccuracy = inaccuracy;
        _ai = ai;
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
        if (singleReload && reloading)
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


        //EventManager.Shot(ray.origin, hit_, transform.root);

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

        bulletsShot = bulletsPerTap;
    }

    //shoot cooldown
    private void ResetShot()
    {
        readyToShoot = true;
    }

    //rapid fire
    public IEnumerator RapidFire()
    {
        var shooter = transform.root;
        if (rapidFireEnabled)
        {
            while (transform.root == shooter)
            {
                Shoot(false);
                yield return rapidFireWait;
            }
        }
        /*else
        {
            Shoot(true);
            yield return null;
        }*/
    }

    //reload
    public void Reload()
    {
        if (bulletsLeft.Equals(magazineSize) || maxAmmo <= 0 || reloadStatus < 1) return;

        reloading = true;
        readyToShoot = true;

        if (singleReload)
        {
            reloadStatus = (float)bulletsLeft / (float)magazineSize;
            StartCoroutine(ReloadSingle());
        }
        else
        {
            reloadSound.Play();
            reloadStatus = 0;
            Invoke("ReloadFinished", reloadTime);
        }
    }

    private IEnumerator ReloadSingle()
    {
        while (reloading && maxAmmo > 0 && bulletsLeft < magazineSize)
        {
            reloadSound.Play();
            yield return new WaitForSeconds(singleReloadTime);
            if (!reloading)
            {
                break;
            }

            bulletsLeft += bulletsPerTap;
            maxAmmo -= bulletsPerTap;
            ShowAmmo();
        }

        pickupSound.Play();
        reloading = false;
    }

    private void ReloadFinished()
    {
        if (reloadSound)
        {
            reloadSound.Stop();
        }

        pickupSound.Play();
        //currentSpread = initialSpread;
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
            _ammoText.SetText((bulletsLeft / bulletsPerTap) + " / " + (magazineSize / bulletsPerTap));
            if (maxAmmo > 0)
                _maxAmmoText.SetText((maxAmmo / bulletsPerTap).ToString());
            else
                _maxAmmoText.SetText("0");
        }
    }

    //Trail
    public IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 start = trail.transform.position;

        while (time < 1)
        {
            //interpolate between 2 points
            trail.transform.position = Vector3.Lerp(start, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

        Destroy(trail.gameObject, trail.time);
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
        StopCoroutine("ReloadSingle");
    }
}
