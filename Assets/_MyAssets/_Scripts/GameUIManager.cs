using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private Image _healthbarImage;
    [SerializeField] private Text _healthbarText;
    [SerializeField] private GameObject _endGameUI;
    [SerializeField] private GameObject _addAmmoUI;

    public static GameUIManager Singleton { get; private set; }

    void Awake()
    {
        Singleton = this;
    }

    /// <summary>
    /// hp in [0..100]
    /// </summary>
    public void SetHealthbarValue(float hp)
    {
        _healthbarImage.fillAmount = hp / 100f;
        _healthbarText.text = hp.ToString();
    }

    void OnDestroy()
    {
        if (Singleton == this)
        {
            Singleton = null;
        }
    }

    public void ShowEndGameUI(bool active = true)
    {
        _endGameUI.SetActive(active);
    }

    public void ShowAddAmmoUI(bool active = true)
    {
        _addAmmoUI.SetActive(active);
    }
}