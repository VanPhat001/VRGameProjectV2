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
            var canStartHost =NetworkManager.Singleton.StartHost();
            Debug.Log(canStartHost ? "Start Host" : "Can't Start Host");
        }
    }
}