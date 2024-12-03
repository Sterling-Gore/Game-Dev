using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public void gotostart()
    {
        SceneManager.LoadSceneAsync("start");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
