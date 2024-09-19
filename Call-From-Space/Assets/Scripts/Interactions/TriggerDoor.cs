using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{

    private Animator _doorAnimator; //references to the Animator component on the door
    // Start is called before the first frame update
    void Start()
    {
        _doorAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){  // !!!!!!!!  LATER: will add alien tag 
            _doorAnimator.SetTrigger("open");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")){   // !!!!!!!!  LATER: will add alien tag 
            _doorAnimator.SetTrigger("closed");
        }
    }
}
