using TMPro;
using UnityEngine;

public class NetworkHead : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;

    public void SetPlayerNameText(string name)
    {
        _playerNameText.text = name;
    }
}