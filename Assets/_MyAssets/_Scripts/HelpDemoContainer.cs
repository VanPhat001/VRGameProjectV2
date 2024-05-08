using System.Collections.Generic;
using UnityEngine;

public class HelpDemoContainer : MonoBehaviour
{
    [SerializeField] private List<HelpDemoItem> _demoItems;
    public List<HelpDemoItem> DemoItems => _demoItems;

    private int _currentIndex = 0;


    void Awake()
    {
        InitDemoItems();
        VisibleAllDemoItems(false);
    }

    private void InitDemoItems()
    {
        _demoItems = new List<HelpDemoItem>();
        foreach (Transform child in this.transform)
        {
            var item = child.GetComponent<HelpDemoItem>();
            _demoItems.Add(item);
        }
    }

    private void VisibleAllDemoItems(bool visible = true)
    {
        foreach (var item in _demoItems)
        {
            item.VisibleZombiePrefab(visible);
        }
    }

    public HelpDemoItem GetCurrentItem()
    {
        return _demoItems[_currentIndex];
    }

    public void NextItem()
    {
        GetCurrentItem().VisibleZombiePrefab(false);

        _currentIndex++;
        if (_currentIndex >= _demoItems.Count)
        {
            _currentIndex = 0;
        }

        GetCurrentItem().VisibleZombiePrefab();
    }

    public void PrevItem()
    {
        GetCurrentItem().VisibleZombiePrefab(false);

        _currentIndex--;
        if (_currentIndex < 0)
        {
            _currentIndex = _demoItems.Count - 1;
        }

        GetCurrentItem().VisibleZombiePrefab();
    }
}