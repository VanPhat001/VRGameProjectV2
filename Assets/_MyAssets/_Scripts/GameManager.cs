using System.Collections;
using DemoObserver;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _netPlayerAvailable;
    private int _zombieAvailable;
    private int _attackWaveIndex = 0;

    void Start()
    {
        _netPlayerAvailable = Multiplayer.Singleton.ConnectedClientDict.Keys.Count;
        this.RegisterListener(EventID.OnNetworkPlayerDeath, OnNetworkPlayerDeath);
        this.RegisterListener(EventID.OnZombieDeath, OnZombieDeath);

        _zombieAvailable = GameEnemySpawner.Singleton.PerformAttackWave(_attackWaveIndex);
    }


    void OnDestroy()
    {
        this.RemoveListener(EventID.OnNetworkPlayerDeath, OnNetworkPlayerDeath);
        this.RemoveListener(EventID.OnZombieDeath, OnZombieDeath);
    }

    void OnNetworkPlayerDeath(object data)
    {
        _netPlayerAvailable--;
        if (_netPlayerAvailable == 0)
        {
            Debug.Log("GAME OVER!!!!!");
            GameUIManager.Singleton.ShowEndGameUI();

            if (NetworkManager.Singleton.IsServer)
            {
                StartCoroutine(ServerGoToLobbySceneCoroutine());
            }
        }
    }

    private void OnZombieDeath(object data)
    {
        _zombieAvailable--;
        if (_zombieAvailable <= 0)
        {
            _attackWaveIndex++;
            _zombieAvailable = GameEnemySpawner.Singleton.PerformAttackWave(_attackWaveIndex);
        }
    }

    IEnumerator ServerGoToLobbySceneCoroutine()
    {
        yield return new WaitForSeconds(3);

        Loader.NetworkLoadScene(Loader.SceneName.LobbyScene);
    }
}