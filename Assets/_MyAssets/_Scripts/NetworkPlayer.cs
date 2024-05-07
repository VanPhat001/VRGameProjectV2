using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour, IDamageable
{
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
    [SerializeField] private InputActionProperty _leftFireInput;
    [SerializeField] private InputActionProperty _rightFireInput;

    private bool _isPlayerReady = false;
    private NetworkHand _leftNetworkHand;
    private NetworkHand _rightNetworkHand;

    public static NetworkPlayer Singleton { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        StartCoroutine(AttackPlayerBody());
        if (IsOwner)
        {
            Singleton = this;
        }

        LocalPlayer.Singleton.VisibleCharacterRenderer(false);
    }



    private void Update()
    {
        if (!IsOwner || !_isPlayerReady)
        {
            return;
        }

        SyncPlayerTransform();
        Shoot();
    }

    private void SyncPlayerTransform()
    {
        _origin.position = LocalPlayer.Singleton.Origin.position;
        _origin.rotation = LocalPlayer.Singleton.Origin.rotation;

        _head.position = LocalPlayer.Singleton.Head.position;
        _head.rotation = LocalPlayer.Singleton.Head.rotation;

        _leftHand.position = LocalPlayer.Singleton.LeftHand.position;
        _leftHand.rotation = LocalPlayer.Singleton.LeftHand.rotation;

        _rightHand.position = LocalPlayer.Singleton.RightHand.position;
        _rightHand.rotation = LocalPlayer.Singleton.RightHand.rotation;
    }

    private IEnumerator AttackPlayerBody()
    {
        yield return new WaitUntil(() => this.transform.childCount >= 3);

        _origin = this.transform;
        _head = this.transform.GetChild(0);
        _leftHand = this.transform.GetChild(1);
        _rightHand = this.transform.GetChild(2);

        _leftNetworkHand = _leftHand.GetComponent<NetworkHand>();
        _rightNetworkHand = _rightHand.GetComponent<NetworkHand>();

        _isPlayerReady = true;
    }

    private void Shoot()
    {
        if (_leftFireInput.action.ReadValue<float>() > 0)
        {
            _leftNetworkHand.Gun.Shoot();
        }

        if (_rightFireInput.action.ReadValue<float>() > 0)
        {
            _rightNetworkHand.Gun.Shoot();
        }
    }

    public void ServerGetHit(float damage)
    {
    }
}