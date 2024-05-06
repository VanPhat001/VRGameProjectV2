using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DestroyOnLoaded : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyLoadingCanvas());
    }

    IEnumerator DestroyLoadingCanvas()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        Destroy(this.gameObject);
    }
}