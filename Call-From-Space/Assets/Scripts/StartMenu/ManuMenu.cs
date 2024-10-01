using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour {

    public void PlayGame() {
        SceneManager.LoadSceneAsync("Intro_scene");
    }

    public void ExitGame() {
        Application.Quit();
    }
}
