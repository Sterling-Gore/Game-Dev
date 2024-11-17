using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    int numEntities = 0;
    private Animator doorAnimator; //references to the Animator component on the door
    public AudioSource doorOpenSound;
    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Alien"))
        {
            if (numEntities == 0)
                doorAnimator.SetTrigger("Open");
                doorOpenSound.Play();
            numEntities++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Alien"))
        {
            numEntities--;
            if (numEntities == 0)
                doorAnimator.SetTrigger("Closed");
        }
    }
}
