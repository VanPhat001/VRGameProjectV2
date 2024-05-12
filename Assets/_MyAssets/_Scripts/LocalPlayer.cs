using System;
using System.Collections.Generic;
using IngameDebugConsole;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[Serializable]
public class RayContainer
{
    public XRRayInteractor xRRayInteractor;
    public LineRenderer lineRenderer;
    public XRInteractorLineVisual xRInteractorLineVisual;

    public void SetEnabled(bool enabled = true)
    {
        this.xRInteractorLineVisual.enabled = enabled;
        this.lineRenderer.enabled = enabled;
        this.xRInteractorLineVisual.enabled = enabled;
    }
}

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] private RayContainer _leftRay;
    [SerializeField] private RayContainer _rightRay;
    [SerializeField] private List<Renderer> _characterRenderer;


    [Header("Character Transform")]
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    public Transform Origin => _origin;
    public Transform Head => _head;
    public Transform LeftHand => _leftHand;
    public Transform RightHand => _rightHand;

    public static LocalPlayer Singleton { get; private set; }

    void Awake()
    {
        Singleton = this;

        if (!Application.isEditor)
        {
            DebugLogManager.Instance.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // _leftRay.SetEnabled(false);
        // _rightRay.SetEnabled(false);
    }

    public void VisibleCharacterRenderer(bool visible = true)
    {
        foreach (var item in _characterRenderer)
        {
            item.enabled = visible;
        }
    }

    // public void SetDetectCollisions(bool value)
    // {
    //     var rb = this.GetComponent<Rigidbody>();
    //     rb.detectCollisions = value;
    // }
}