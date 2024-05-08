using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpUIManager : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TMP_Text _zombieNameText;
    [SerializeField] private TMP_Text _zombieDetailText;
    [SerializeField] private HelpDemoContainer _helpDemoContainer;

    void Start()
    {
        _backButton.onClick.AddListener(OpenHomeScene);
        _prevButton.onClick.AddListener(PrevDemo);
        _nextButton.onClick.AddListener(NextDemo);

        RenderData();
        _helpDemoContainer.GetCurrentItem().VisibleZombiePrefab();
    }

    private void PrevDemo()
    {
        _helpDemoContainer.PrevItem();
        RenderData();
    }

    private void NextDemo()
    {
        _helpDemoContainer.NextItem();
        RenderData();
    }

    private void OpenHomeScene()
    {
        Loader.LoadScene(Loader.SceneName.HomeScene);
    }

    private void RenderData()
    {
        var item = _helpDemoContainer.GetCurrentItem();

        _zombieNameText.text = item.Name;
        _zombieDetailText.text = item.Description;
    }
}