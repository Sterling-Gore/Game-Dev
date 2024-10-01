using System.Collections;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SceneManagement;
public class CutSceneStartGame : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        //Only specify the sceneName or sceneBuildIntex will load the scene with the single mode
        SceneManager.LoadScene("Ship", LoadSceneMode.Single);
    }
}
