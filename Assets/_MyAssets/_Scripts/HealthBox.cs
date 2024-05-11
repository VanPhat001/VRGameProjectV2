using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HealthBox : NetworkBehaviour
{
    [SerializeField] private float _healthAmount;
    [SerializeField] private Collider _collider;
    private NetworkVariable<bool> _netActive = new NetworkVariable<bool>(true);
    private const float TRIGGER_RATE = 20;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetVisible();
        _netActive.OnValueChanged += (_, val) => SetVisible(val);
    }

    private IEnumerator ServerTriggerNetActiveCoroutine()
    {
        _netActive.Value = false;
        yield return new WaitForSeconds(TRIGGER_RATE);

        _netActive.Value = true;
    }

    private void SetVisible(bool visible = true)
    {
        _collider.enabled = visible;
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(visible);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        if (!other.gameObject.CompareTag("NetworkPlayer"))
        {
            return;
        }

        var healable = other.transform.GetComponent<IHealable>();
        healable?.ServerHeal(_healthAmount);

        StartCoroutine(ServerTriggerNetActiveCoroutine());
    }
}