using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Entered_Station_Trigger : LoadableTrigger
{
    // Start is called before the first frame update
    public GameObject player;
    public AudioSource toxic_chems;



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.instance.SaveGame(GameStateManager.checkPointFilePath);
            gameObject.SetActive(false);
            Action();
        }
    }

    void Action()
    {
        toxic_chems.enabled = true;
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().AddTask("Find a Power Generator", false);
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().AddTask("Find An Oxygen Station", false);
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().DeleteTask("Find Your Way Into The Station");
        player.GetComponent<PlayerController>().oxygenSystem.LosingOxygen = true;
    }

    public override void Load(JObject state)
    {
        base.Load(state);
        if (!gameObject.activeSelf)
            Action();
    }
}
