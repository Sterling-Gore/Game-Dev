using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWin : MonoBehaviour
{
    public GameObject player;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            player.GetComponent<Interactor>().inUI = true;
            SceneManager.LoadSceneAsync("WinScreen");
        }
    }
}