using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    Interactor interactor;

    void OnEnable()
    {
        Debug.Log("DeathMenu enabled");
        var player = GameObject.Find("player");
        interactor = player.GetComponent<Interactor>();
        interactor.inUI = true;
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        interactor.inUI = false;

        Time.timeScale = 1f;
    }

    public void GoToCheckPoint()
    {
        GameStateManager.instance.LoadGame(GameStateManager.checkPointFilePath);
        gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("Start");
    }
}
