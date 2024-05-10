using System.Collections;
using DemoObserver;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int _netPlayerAvailable;

    void Start()
    {
        _netPlayerAvailable = Multiplayer.Singleton.ConnectedClientDict.Keys.Count;
        this.RegisterListener(EventID.OnNetworkPlayerDeath, OnNetworkPlayerDeath);
    }

    void OnDestroy()
    {
        this.RemoveListener(EventID.OnNetworkPlayerDeath, OnNetworkPlayerDeath);
    }

    void OnNetworkPlayerDeath(object data)
    {
        _netPlayerAvailable--;
        if (_netPlayerAvailable == 0)
        {
            Debug.Log("GAME OVER!!!!!");

            if (NetworkManager.Singleton.IsServer)
            {
                StartCoroutine(ServerGoToLobbySceneCoroutine());
            }
        }
    }

    IEnumerator ServerGoToLobbySceneCoroutine()
    {
        yield return new WaitForSeconds(3);

        Loader.NetworkLoadScene(Loader.SceneName.LobbyScene);
    }
}