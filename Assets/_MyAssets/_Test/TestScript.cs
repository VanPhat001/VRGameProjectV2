using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestScript : MonoBehaviour
{
    public CFX_SpawnSystem cfx_SpawnSystem;
    public GameObject effectPrefab;

    public InputActionProperty xbutton;
    public InputActionProperty ybutton;

    void Start()
    {

    }

    float _timer = .4f;
    void Update()
    {
        SpawnEffect();
        TestButton();
    }

    private void TestButton()
    {
        
        // Debug.Log(xbutton.action.ReadValue<float>() > 0.4f);
        // if (xbutton.action.ReadValue<float>() > 0)
        // {
        //     Debug.Log("x button");
        // }

        // if (ybutton.action.ReadValue<float>() > 0)
        // {
        //     Debug.Log("y button");
        // }
    }

    void SpawnEffect()
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
