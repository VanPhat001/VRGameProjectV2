using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class ExplosiveSmoke : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float radius;

    public void Explosion()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        var colliders = Physics.OverlapSphere(this.transform.position, radius);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("NetworkPlayer"))
            {
                collider.GetComponent<IDamageable>()?.ServerGetHit(damage);
            }
        }
    }
}
