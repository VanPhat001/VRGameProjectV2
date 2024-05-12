using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class ExplosiveSmoke : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _radius;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSound;

    public void Explosion()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        PlaySound();

        var colliders = Physics.OverlapSphere(this.transform.position, _radius);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("NetworkPlayer"))
            {
                collider.GetComponent<IDamageable>()?.ServerGetHit(_damage);
            }
        }
    }

    void PlaySound()
    {
        _audioSource.PlayOneShot(_explosionSound);
    }
}
