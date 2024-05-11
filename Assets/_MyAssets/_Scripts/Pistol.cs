using System;
using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour, IGun
{
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _ammoRate;
    [SerializeField] private float _reloadTime = 2f;
    [SerializeField] private float _ammoSpeed;
    [SerializeField] private int _ammoRemain;
    [SerializeField] private int _ammoAvailable;
    [SerializeField] private int _ammoReloadNumber;
    [SerializeField] private int _ammoAddAmount;
    [SerializeField] private int _ammoAddTime;
    private float _ammoFireTimer = 0;
    private float _reloadTimer = 2f;
    private bool _isReloading = false;
    private bool _isAddAmmo = false;

    private int m_ammoRemain;
    private int m_ammoAvailable;


    public float ReloadTimer => _reloadTimer;

    void Start()
    {
        m_ammoRemain = _ammoRemain;
        m_ammoAvailable = _ammoAvailable;
    }

    void Update()
    {
        _ammoFireTimer -= Time.deltaTime;
        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
        }
    }

    public void Init()
    {
        _ammoRemain = m_ammoRemain;
        _ammoAvailable = m_ammoAvailable;
    }

    public bool Shoot()
    {
        if (!CanShoot())
        {
            return false;
        }

        _ammoFireTimer = _ammoRate;
        _ammoAvailable--;
        NetworkCommunication.Singleton.SpawnBulletServerRpc(
            _firePoint.position,
            _firePoint.rotation,
            _firePoint.forward.normalized * _ammoSpeed
        );

        if (_ammoAvailable <= 0 && _ammoRemain > 0)
        {
            Reload();
        }

        return true;
    }

    public bool CanShoot()
    {
        return !_isAddAmmo && !_isReloading && _ammoFireTimer <= 0 && _ammoAvailable > 0;
    }

    public void Reload()
    {
        if (_isReloading || _isAddAmmo)
        {
            return;
        }

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

    public void AddAmmo(int amount = -1)
    {
        if (_isReloading || _isAddAmmo)
        {
            return;
        }
        Debug.Log("add ammo....");

        if (amount < 0)
        {
            amount = _ammoAddAmount;
        }

        StartCoroutine(AddAmmoCoroutine(amount));
    }

    IEnumerator AddAmmoCoroutine(int amount)
    {
        _isAddAmmo = true;
        yield return new WaitForSeconds(_ammoAddTime);

        _ammoRemain += amount;
        _isAddAmmo = false;
    }
}