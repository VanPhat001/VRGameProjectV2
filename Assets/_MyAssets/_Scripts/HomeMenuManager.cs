using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuManager : MonoBehaviour
{
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _joinGameButton;

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

        // if (!NetworkManager.Singleton.StartHost())
        // {
        //     ActiveLoadingUI(false);
        //     Debug.Log("Can't Start Host!!!");
        //     return;
        // }
        GameData.IsClient = false;
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

        // if (!NetworkManager.Singleton.StartClient())
        // {
        //     ActiveLoadingUI(false);
        //     Debug.Log("Can't Start Client!!!");
        //     return;
        // }

        GameData.IsClient = true;
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
