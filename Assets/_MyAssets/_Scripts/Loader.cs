using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Loader
{
    public enum SceneName
    {
        HomeScene, LobbyScene, GameScene, HelpScene
    }

    public static void LoadScene(SceneName sceneName)
    {
        SceneManager.LoadScene(sceneName.ToString());
    }

    public static void NetworkLoadScene(SceneName sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), LoadSceneMode.Single);
    }

    public static bool IsScene(SceneName sceneName)
    {
        return SceneManager.GetActiveScene().name.Equals(sceneName.ToString());
    }
}