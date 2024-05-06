using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private TMP_Text _text;

    void Start()
    {
        var visibleText = GameData.IsClient;

        _text.gameObject.SetActive(visibleText);
        _startButton.gameObject.SetActive(!visibleText);

        _startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        Loader.NetworkLoadScene(Loader.SceneName.GameScene);
    }
}