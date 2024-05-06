using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    private bool _isPlayerReady = false;

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

        _isPlayerReady = true;
    }
}