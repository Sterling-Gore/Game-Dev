using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roomx_Entry_Trigger : MonoBehaviour
{

    public AudioSource Specimen_toxic_gass;
    public GameObject StationDoor;
    public GameObject player;
    public CameraShakeGeneral cameraShake;
    public AudioSource PlantScreech;
    public OxygenSystem Oxygen;
    // Start is called before the first frame update
    
    
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

    void Action()
    {
        Oxygen.LosingOxygen = true;
        Specimen_toxic_gass.enabled = true;
        StationDoor.GetComponent<Animator>().SetTrigger("Closed");
        StationDoor.GetComponent<Collider>().enabled = false;
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().RoomX();
        gameObject.SetActive(false);
    }
}
