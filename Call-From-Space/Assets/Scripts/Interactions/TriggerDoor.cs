using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{

    private Animator _doorAnimator; //references to the Animator component on the door
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("YAAAAAAAAAAAAS");
        if (other.CompareTag("Player") || other.CompareTag("Alien"))
        {
            _doorAnimator.SetTrigger("Open");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Alien"))
        {
            _doorAnimator.SetTrigger("Closed");
        }
    }
}
