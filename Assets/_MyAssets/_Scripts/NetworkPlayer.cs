using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using DemoObserver;

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
        _netHP.OnValueChanged += (_, val) =>
        {
            UpdateHealthbar();
            if (val == 0)
            {
                VisibleNetPlayer(false);
            }
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
        if (!Loader.IsScene(Loader.SceneName.GameScene) || HP <= 0)
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
        if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
        {
            GameUIManager.Singleton?.SetHealthbarValue(HP);
        }
    }

    public void UpdatePlayerName()
    {
        _networkHead?.SetPlayerNameText(_netPlayerName.Value.ToString());
    }

    public void ServerGetHit(float damage)
    {
        if (HP == 0)
        {
            return;
        }

        _netHP.Value = Mathf.Clamp(HP - damage, 0, 100);
        Debug.Log("ClientId " + OwnerClientId + " HP " + HP);
        UpdateHealthbar();

        if (HP == 0)
        {
            this.PostEvent(EventID.OnNetworkPlayerDeath);
        }
    }

    public void ServerHeal(float amount)
    {
        _netHP.Value = Mathf.Clamp(_netHP.Value + amount, 0, 100);
        Debug.Log("ClientId " + OwnerClientId + " [HEAL] HP " + _netHP.Value);
        UpdateHealthbar();
    }

    void VisibleNetPlayer(Transform parent, bool visible)
    {
        if (parent == null)
        {
            return;
        }

        if (parent.TryGetComponent<Renderer>(out var renderer))
        {
            renderer.enabled = visible;
        }

        foreach (Transform child in parent)
        {
            VisibleNetPlayer(child, visible);
        }
    }

    public void VisibleNetPlayer(bool visible)
    {
        // if (IsOwner)
        // {
        //     LocalPlayer.Singleton.SetDetectCollisions(visible);
        // }

        this.transform.GetComponent<Collider>().enabled = visible;
        VisibleNetPlayer(this.transform, visible);
    }

    [ServerRpc(RequireOwnership = false)]
    public void InitNetworkPlayerServerRpc()
    {
        this.transform.GetComponent<Collider>().enabled = true;
        _netHP.Value = 100;
    }

    public void InitNetworkPlayer()
    {
        InitNetworkPlayerServerRpc();
        VisibleNetPlayer(true);
    }
}