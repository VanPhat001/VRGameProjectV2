using UnityEngine;

public class TestScript : MonoBehaviour
{
    public CFX_SpawnSystem cfx_SpawnSystem;
    public GameObject effectPrefab;

    void Start()
    {

    }

    float _timer = .4f;
    void Update()
    {
        if (!CFX_SpawnSystem.AllObjectsLoaded)
        {
            return;
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _timer = .4f;
            var effect = CFX_SpawnSystem.GetNextObject(effectPrefab);
            effect.transform.position = new Vector3(
                UnityEngine.Random.Range(-15, 15),
                0,
                UnityEngine.Random.Range(-15, 15)
            );
        }
    }
}
