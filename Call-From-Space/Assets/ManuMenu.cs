using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingMenu : MonoBehaviour {

    public void PlayGame() {
        SceneManager.LoadSceneAsync("Ship");
    }

    public void ExitGame() {
        Application.Quit();
    }
}
