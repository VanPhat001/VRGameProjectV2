using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameEnemySpawner : MonoBehaviour
{
    public static GameEnemySpawner Singleton { get; private set; }

    [SerializeField] private GameObject _warZombiePrefab;
    [SerializeField] private GameObject _copZombiePrefab;
    [SerializeField] private GameObject _skeletonPrefab;
    [SerializeField] private GameObject _healthBoxPrefab;


    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        ServerSpawnHealthBox();
        StartCoroutine(ServerSpawnCoroutine());
    }

    public IEnumerator ServerSpawnCoroutine()
    {
        yield return new WaitForSeconds(2);

        ServerSpawnSkeleton();
    }

    public void ServerSpawnWarZombie()
    {
        var zombie = Instantiate(_warZombiePrefab);
        zombie.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        // zombie.GetComponent<WarZombieManager>().Target = LocalPlayer.Singleton.Head;
        zombie.GetComponent<WarZombieManager>().ServerChangeTarget();
        zombie.GetComponent<NetworkObject>().Spawn();
    }

    public void ServerSpawnCopZombie()
    {
        var zombie = Instantiate(_copZombiePrefab);
        zombie.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        // zombie.GetComponent<WarZombieManager>().Target = LocalPlayer.Singleton.Head;
        zombie.GetComponent<CopZombieManager>().ServerChangeTarget();
        zombie.GetComponent<NetworkObject>().Spawn();
    }

    public void ServerSpawnSkeleton()
    {
        var skeleton = Instantiate(_skeletonPrefab);
        skeleton.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        // zombie.GetComponent<WarZombieManager>().Target = LocalPlayer.Singleton.Head;
        skeleton.GetComponent<SkeletonManager>().ServerChangeTarget();
        skeleton.GetComponent<NetworkObject>().Spawn();
    }

    public void ServerSpawnHealthBox()
    {
        var healthBox = Instantiate(_healthBoxPrefab);
        healthBox.GetComponent<NetworkObject>().Spawn();
    }
}