using Unity.Netcode;
using UnityEngine;

public class NetworkCommunication : NetworkBehaviour {
    public static NetworkCommunication Singleton { get; private set; }

    [SerializeField] private GameObject _ammoPrefab;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _fireSound;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            Singleton = this;
        }
    }

    [ServerRpc(RequireOwnership = false)] 
    public void SpawnBulletServerRpc(Vector3 origin, Quaternion rotation, Vector3 velocity)
    {
        var bullet = Instantiate(_ammoPrefab);
        bullet.GetComponent<NetworkObject>().Spawn();
        bullet.GetComponent<Bullet>().ServerInit(origin, rotation, velocity);

        PlayBulletFireSoundClientRpc();
    }

    [ClientRpc]
    public void PlayBulletFireSoundClientRpc()
    {
        _audioSource.PlayOneShot(_fireSound);
    }



}