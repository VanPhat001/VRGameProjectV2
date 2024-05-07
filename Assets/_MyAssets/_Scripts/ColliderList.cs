using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ColliderList
{
    [SerializeField] public List<Collider> _colliders;

    public void SetEnabled(bool enabled = true)
    {
        foreach (var item in _colliders)
        {
            item.enabled = enabled;
        }
    }
}