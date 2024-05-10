using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LobbyPlayerSpawner : MonoBehaviour
{
    void Start()
    {
        if (GameData.IsClient) // client
        {
            var canStartClient = NetworkManager.Singleton.StartClient();
            Debug.Log(canStartClient ? "Start Client" : "Can't Start Client");
        }
        else // host
        {
            var canStartHost = NetworkManager.Singleton.StartHost();
            Debug.Log(canStartHost ? "Start Host" : "Can't Start Host");
        }

        if (NetworkManager.Singleton.IsServer)
        {
            ServerDespawnAllNonPlayerNetworkObjects();
        }
        NetworkPlayer.Singleton?.InitNetworkPlayer();
    }

    void ServerDespawnAllNonPlayerNetworkObjects()
    {
        var netObjs = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.ToList();
        foreach (var netObj in netObjs)
        {
            if (!netObj.gameObject.tag.StartsWith("NetworkPlayer"))
            {
                netObj.Despawn();
            }
        }
    }
}