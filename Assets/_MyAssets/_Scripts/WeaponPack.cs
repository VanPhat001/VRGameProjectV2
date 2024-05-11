using UnityEngine;

public class WeaponPack : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        SetIsPlayerStayInWeaponPack(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        SetIsPlayerStayInWeaponPack(other, false);
    }

    void SetIsPlayerStayInWeaponPack(Collider other, bool value)
    {
        if (!other.gameObject.CompareTag("NetworkPlayer"))
        {
            return;
        }

        if (other.gameObject.GetInstanceID() != NetworkPlayer.Singleton.gameObject.GetInstanceID())
        {
            return;
        }

        NetworkPlayer.Singleton.IsPlayerStayInWeaponPack = value;
        Debug.Log("Set value " + value);
    }

}