using Unity.Netcode;

public class NetworkCommunication : NetworkBehaviour {
    public static NetworkCommunication Singleton { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            Singleton = this;
        }
    }
}