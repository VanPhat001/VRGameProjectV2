using UnityEngine;

public class HelpDemoItem : MonoBehaviour
{
    [SerializeField] private GameObject _zombiePrefab;
    [SerializeField] private string _name;
    [SerializeField, TextArea(5, 30)] private string _description;

    public GameObject ZombiePrefab => _zombiePrefab;
    public string Name => _name;
    public string Description => _description;

    public void VisibleZombiePrefab(bool visible = true)
    {
        _zombiePrefab.SetActive(visible);
    }
}