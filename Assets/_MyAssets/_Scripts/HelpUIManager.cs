using System;
using UnityEngine;
using UnityEngine.UI;

public class HelpUIManager : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _prevButton;
    [SerializeField] private Button _nextButton;

    void Start()
    {
        _backButton.onClick.AddListener(OpenHomeScene);
        _prevButton.onClick.AddListener(PrevDemo);
        _nextButton.onClick.AddListener(NextDemo);
    }

    private void PrevDemo()
    {
    }

    private void NextDemo()
    {
    }

    private void OpenHomeScene()
    {
        Loader.LoadScene(Loader.SceneName.HomeScene);
    }
}