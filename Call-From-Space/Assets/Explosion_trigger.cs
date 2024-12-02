using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_trigger : MonoBehaviour
{
    public GameObject StationDoor;
    public GameObject player;
    public GameObject ShuttleLights;
    public AudioSource Explosion;
    public CameraShakeGeneral cameraShake;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {


            
            Explosion.enabled = true;
            StationDoor.GetComponent<Animator>().SetTrigger("Closed");
            cameraShake.StartShake(2f, .8f);
            ShuttleLights.SetActive(false);
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().explosion();
            

            gameObject.SetActive(false);
        }
    }
}
