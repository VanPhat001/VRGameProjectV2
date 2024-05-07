using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private Rigidbody _rb;


    public void ServerInit(Vector3 pos, Quaternion rotation, Vector3 velocity)
    {
        transform.position = pos;
        transform.rotation = rotation;
        _rb.velocity = velocity;

        StartCoroutine(ServerReleaseCoroutine(7));
    }

    IEnumerator ServerReleaseCoroutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        ServerRelease();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        // Debug.Log(other.tag);
        var damageable = other.GetComponent<IDamageable>();

        damageable?.ServerGetHit(_damage);

        ServerRelease();
    }

    void ServerRelease()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}