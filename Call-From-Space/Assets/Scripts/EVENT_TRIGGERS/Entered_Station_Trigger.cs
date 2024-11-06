using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entered_Station_Trigger : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject StationDoor;
    public GameObject player;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {

            StationDoor.GetComponent<Animator>().SetTrigger("Closed");
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().AddTask("Find a Power Generator", false);
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().AddTask("Find An Oxygen Station", false);
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().DeleteTask("Find Your Way Into The Station");
            player.GetComponent<PlayerController>().oxygenSystem.LosingOxygen = true;

            gameObject.SetActive(false);
        }
    }
}
