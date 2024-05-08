using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour, IDamageable, IHealable
{
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
    [SerializeField] private InputActionProperty _leftFireInput;
    [SerializeField] private InputActionProperty _rightFireInput;
    private NetworkVariable<float> _netHP = new NetworkVariable<float>(100);
    private NetworkVariable<FixedString128Bytes> _netPlayerName = new NetworkVariable<FixedString128Bytes>("[TEST] PlayerName", writePerm: NetworkVariableWritePermission.Owner);

    public float HP => _netHP.Value;

    private bool _isPlayerReady = false;
    private NetworkHand _leftNetworkHand;
    private NetworkHand _rightNetworkHand;
    private NetworkHead _networkHead;

    public static NetworkPlayer Singleton { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        StartCoroutine(AttackPlayerBody());

        UpdateHealthbar();
        _netHP.OnValueChanged += (_, _) =>
        {
            UpdateHealthbar();
        };

        // UpdatePlayerName();
        _netPlayerName.OnValueChanged += (_, _) =>
        {
            UpdatePlayerName();
        };

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
        _networkHead = _head.GetComponent<NetworkHead>();

        if (IsOwner)
        {
            _netPlayerName.Value = GameData.PlayerName;
        }
        else
        {
            UpdatePlayerName();
        }

        _isPlayerReady = true;
    }

    private void Shoot()
    {
        if (!Loader.IsScene(Loader.SceneName.GameScene))
        {
            return;
        }

        if (_leftFireInput.action.ReadValue<float>() > 0)
        {
            _leftNetworkHand.Gun.Shoot();
        }

        if (_rightFireInput.action.ReadValue<float>() > 0)
        {
            _rightNetworkHand.Gun.Shoot();
        }
    }

    public void UpdateHealthbar()
    {
        // no code
    }

    public void UpdatePlayerName()
    {
        _networkHead?.SetPlayerNameText(_netPlayerName.Value.ToString());
    }

    public void ServerGetHit(float damage)
    {
        _netHP.Value = Mathf.Clamp(_netHP.Value - damage, 0, 100);
        Debug.Log("ClientId " + OwnerClientId + " HP " + _netHP.Value);
        UpdateHealthbar();
    }

    public void ServerHeal(float amount)
    {
        _netHP.Value = Mathf.Clamp(_netHP.Value + amount, 0, 100);
        Debug.Log("ClientId " + OwnerClientId + " [HEAL] HP " + _netHP.Value);
        UpdateHealthbar();
    }
}