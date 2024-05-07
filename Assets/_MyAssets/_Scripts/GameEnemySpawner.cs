using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameEnemySpawner : MonoBehaviour
{
    public static GameEnemySpawner Singleton { get; private set; }

    [SerializeField] private GameObject _warZombiePrefab;


    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        StartCoroutine(ServerSpawnCoroutine());
    }

    public IEnumerator ServerSpawnCoroutine()
    {
        yield return new WaitForSeconds(2);

        ServerSpawnWarZombie();
    }

    public void ServerSpawnWarZombie()
    {
        var zombie = Instantiate(_warZombiePrefab);
        zombie.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        // zombie.GetComponent<WarZombieManager>().Target = LocalPlayer.Singleton.Head;
        zombie.GetComponent<WarZombieManager>().ServerChangeTarget();
        zombie.GetComponent<NetworkObject>().Spawn();
    }
}