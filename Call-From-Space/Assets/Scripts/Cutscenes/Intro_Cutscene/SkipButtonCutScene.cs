using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipButtonCutSceneClass : MonoBehaviour
{

    public void ClickHere(){
        // Console.WriteLine("Clicked");
    }

    public void LoadShipScene()
    {
        SceneManager.LoadScene("Ship", LoadSceneMode.Single);
    }

}