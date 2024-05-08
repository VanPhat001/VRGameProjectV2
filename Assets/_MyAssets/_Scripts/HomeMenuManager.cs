using System;
using Keyboard;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _playerNameInput;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _joinGameButton;
    [SerializeField] private Button _helpButton;

    [Header("Normal UI")]
    [SerializeField] private GameObject _normalUI;
    [SerializeField] private Button _quitButton;

    [Header("Main Menu Loading UI")]
    [SerializeField] private GameObject _loadingUI;

    [Header("Quiting UI")]
    [SerializeField] private GameObject _quittingUI;
    [SerializeField] private Button _quittingYesButton;
    [SerializeField] private Button _quittingNoButton;


    void Start()
    {
        _startGameButton.onClick.AddListener(StartGameAsync);
        _joinGameButton.onClick.AddListener(JoinGameAsync);
        _quitButton.onClick.AddListener(QuitGame);
        _quittingYesButton.onClick.AddListener(OnYesButtonClicked);
        _quittingNoButton.onClick.AddListener(OnNoButtonClicked);
        _helpButton.onClick.AddListener(OpenHelpScene);

        GameData.PlayerName = "Player" + UnityEngine.Random.Range(0, 1000);
        SetPlayerNameInput(GameData.PlayerName);
    }

    private void SetPlayerNameInput(string text)
    {
        _playerNameInput.text = text;
    }

    private string GetPlayerNameInput()
    {
        return _playerNameInput.text;
    }

    private async void StartGameAsync()
    {
        ActiveLoadingUI();

        var canCreate = await Multiplayer.Singleton.CreateLobbyAsync("MyLobby");
        if (!canCreate)
        {
            ActiveLoadingUI(false);
            Debug.Log("Can't create room!!!");
            return;
        }

        GameData.IsClient = false;
        GameData.PlayerName = GetPlayerNameInput();
        Loader.LoadScene(Loader.SceneName.LobbyScene);
    }

    private async void JoinGameAsync()
    {
        ActiveLoadingUI();

        var canJoin = await Multiplayer.Singleton.QuickJoinLobbyAsync();
        if (!canJoin)
        {
            ActiveLoadingUI(false);
            Debug.Log("Can't join room!!!");
            return;
        }

        GameData.IsClient = true;
        GameData.PlayerName = GetPlayerNameInput();
        Loader.LoadScene(Loader.SceneName.LobbyScene);
    }

    private void QuitGame()
    {
        ActiveNormalUI(false);
    }


    private void OnYesButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnNoButtonClicked()
    {
        ActiveNormalUI();
    }

    private void OpenHelpScene()
    {
        Loader.LoadScene(Loader.SceneName.HelpScene);
    }

    public void ActiveNormalUI(bool active = true)
    {
        _normalUI.SetActive(active);
        _quittingUI.SetActive(!active);
    }

    public void ActiveLoadingUI(bool active = true)
    {
        _loadingUI.SetActive(active);
    }
}
