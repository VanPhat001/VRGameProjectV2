using System;
using System.Collections;
using System.Collections.Generic;
using DemoObserver;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class AttackWave
{
    public int WarZombieCount;
    public int CopZombieCount;
    public int SkeletonCount;
    public int TotalZombie => WarZombieCount + CopZombieCount + SkeletonCount;

    public AttackWave(int warZombieCount = 0, int copZombieCount = 0, int skeletonCount = 0)
    {
        WarZombieCount = warZombieCount;
        CopZombieCount = copZombieCount;
        SkeletonCount = skeletonCount;
    }
}

public class GameEnemySpawner : MonoBehaviour
{
    public static GameEnemySpawner Singleton { get; private set; }

    [SerializeField] private GameObject _warZombiePrefab;
    [SerializeField] private GameObject _copZombiePrefab;
    [SerializeField] private GameObject _skeletonPrefab;
    [SerializeField] private GameObject _healthBoxPrefab;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<AttackWave> _attackWaves;

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        ServerSpawnHealthBox();
        // StartCoroutine(ServerSpawnCoroutine());
    }

    /// <summary>
    /// TEST
    /// </summary>
    /// <returns></returns>
    public IEnumerator ServerSpawnCoroutine()
    {
        yield return new WaitForSeconds(2);

        ServerSpawnSkeleton();
        // ServerSpawnWarZombie();
    }

    public int PerformAttackWave(int attackIndex)
    {
        var wave = _attackWaves[attackIndex];

        for (int i = 0; i < wave.SkeletonCount; i++)
        {
            ServerSpawnSkeleton(true);
        }

        for (int i = 0; i < wave.CopZombieCount; i++)
        {
            ServerSpawnCopZombie(true);
        }

        for (int i = 0; i < wave.WarZombieCount; i++)
        {
            ServerSpawnWarZombie(true);
        }

        return wave.TotalZombie;
    }

    public void ServerSpawnWarZombie(bool isRandom = false)
    {
        var zombie = Instantiate(_warZombiePrefab);
        if (isRandom)
        {
            zombie.transform.position = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)].position;
        }
        else
        {
            zombie.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        }
        zombie.GetComponent<WarZombieManager>().ServerChangeTarget();
        zombie.GetComponent<NetworkObject>().Spawn();
    }

    public void ServerSpawnCopZombie(bool isRandom = false)
    {
        var zombie = Instantiate(_copZombiePrefab);
        if (isRandom)
        {
            zombie.transform.position = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)].position;
        }
        else
        {
            zombie.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        }
        zombie.GetComponent<CopZombieManager>().ServerChangeTarget();
        zombie.GetComponent<NetworkObject>().Spawn();
    }

    public void ServerSpawnSkeleton(bool isRandom = false)
    {
        var zombie = Instantiate(_skeletonPrefab);
        if (isRandom)
        {
            zombie.transform.position = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Count)].position;
        }
        else
        {
            zombie.transform.position = LocalPlayer.Singleton.Head.position + LocalPlayer.Singleton.Head.forward.normalized * 10;
        }
        zombie.GetComponent<SkeletonManager>().ServerChangeTarget();
        zombie.GetComponent<NetworkObject>().Spawn();
    }

    public void ServerSpawnHealthBox()
    {
        var healthBox = Instantiate(_healthBoxPrefab);
        healthBox.GetComponent<NetworkObject>().Spawn();
    }
}