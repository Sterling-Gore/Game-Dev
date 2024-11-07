using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRechargeTrigger : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().LaserPuzzle(1);
        }
    }
}
