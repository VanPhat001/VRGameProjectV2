using System;
using System.Collections;
using TMPro;
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

    [Header("UI")]
    [SerializeField] private TMP_Text _statusText;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private AudioClip _firingFailureSound;

    private float _ammoFireTimer = 0;
    private float _reloadTimer = 2f;
    private bool _isReloading = false;
    private bool _isAddAmmo = false;
    private Coroutine _playSoundCoroutine = null;

    private int m_ammoRemain;
    private int m_ammoAvailable;


    public int AmmoRemain => _ammoRemain;
    public int AmmoAvailable => _ammoAvailable;
    public float ReloadTimer => _reloadTimer;

    void Start()
    {
        m_ammoRemain = _ammoRemain;
        m_ammoAvailable = _ammoAvailable;

        UpdateStatusText();
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
        UpdateStatusText();
    }

    public bool Shoot()
    {
        if (!CanShoot())
        {
            PlayFiringFailureSound();
            return false;
        }

        _ammoFireTimer = _ammoRate;
        _ammoAvailable--;
        NetworkCommunication.Singleton.SpawnBulletServerRpc(
            _firePoint.position,
            _firePoint.rotation,
            _firePoint.forward.normalized * _ammoSpeed
        );
        PlayFireSound();

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

        UpdateStatusText();
    }

    public void AddAmmo(int amount = -1)
    {
        if (_isReloading || _isAddAmmo)
        {
            return;
        }

        if (amount < 0)
        {
            amount = _ammoAddAmount;
        }

        StartCoroutine(AddAmmoCoroutine(amount));
    }

    IEnumerator AddAmmoCoroutine(int amount)
    {
        _isAddAmmo = true;
        GameUIManager.Singleton.ShowAddAmmoUI();
        UpdateStatusText();
        yield return new WaitForSeconds(_ammoAddTime);

        _ammoRemain += amount;
        _isAddAmmo = false;
        GameUIManager.Singleton.ShowAddAmmoUI(false);
        UpdateStatusText();
    }

    void PlayFireSound()
    {
        _audioSource.PlayOneShot(_fireSound);
    }

    void PlayFiringFailureSound()
    {
        if (_playSoundCoroutine != null)
        {
            return;
        }

        _playSoundCoroutine = StartCoroutine(PlayFiringFailureSoundCoroutine());
    }

    IEnumerator PlayFiringFailureSoundCoroutine()
    {
        _audioSource.PlayOneShot(_firingFailureSound);
        yield return new WaitForSeconds(.3f);

        _playSoundCoroutine = null;
    }

    public void UpdateStatusText()
    {
        SetStatusText($"{AmmoAvailable} / {AmmoRemain}");
    }

    void SetStatusText(string text)
    {
        _statusText.text = text;
    }
}