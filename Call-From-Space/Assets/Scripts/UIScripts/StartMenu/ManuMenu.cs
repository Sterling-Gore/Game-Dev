using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour
{

    public void PlayGame()
    {
        GameStateManager.instance.NewGame();
        SceneManager.LoadSceneAsync("Intro_scene");
    }

    public void LoadGame()
    {
        GameStateManager.instance.LoadGame(GameStateManager.saveFilePath);
        if (GameStateManager.startedNewGame)
            GameStateManager.instance.LoadGame(GameStateManager.checkPointFilePath);
        SceneManager.LoadSceneAsync("Ship");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
