using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouWin : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.instance.SaveGame(GameStateManager.checkPointFilePath);
            PlantScreech.Play();
            cameraShake.StartShake(2f, 0.05f);
            Action();
        }
    }
}
