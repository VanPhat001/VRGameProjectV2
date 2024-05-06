using System;
using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour, IGun
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject _ammoPrefab;
    [SerializeField] private float _ammoRate;
    [SerializeField] private float _reloadTime = 2f;
    [SerializeField] private float _ammoSpeed;
    [SerializeField] private float _ammoDamage;
    [SerializeField] private int _ammoRemain;
    [SerializeField] private int _ammoAvailable;
    [SerializeField] private int _ammoReloadNumber;
    private float _ammoFireTimer = 0;
    private float _reloadTimer = 2f;
    private bool _isReloading = false;

    public float ReloadTimer => _reloadTimer;

    void Update()
    {
        _ammoFireTimer -= Time.deltaTime;
        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
        }
    }


    public bool Shoot()
    {
        if (!CanShoot())
        {
            return false;
        }

        ////////////
        ///shoot///
        ///////////

        return true;
    }

    public bool CanShoot()
    {
        return !_isReloading && _ammoFireTimer <= 0 && _ammoAvailable > 0;
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        _isReloading = true;
        _reloadTimer = _reloadTime;

        yield return new WaitForSeconds(_reloadTime);

        int ammoNeeded = Math.Min(_ammoReloadNumber, _ammoReloadNumber - _ammoAvailable);
        int ammoReality = Math.Min(ammoNeeded, _ammoRemain);
        _ammoAvailable += ammoReality;
        _ammoRemain -= ammoReality;

        _reloadTimer = 0;
        _isReloading = false;
    }
}