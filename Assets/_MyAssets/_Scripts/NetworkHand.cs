using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkHand : NetworkBehaviour {
    [SerializeField] private bool _isLeftHand;
    [SerializeField] private Pistol _gun;

    public bool IsLeftHand => _isLeftHand;
    public Pistol Gun => _gun;

}