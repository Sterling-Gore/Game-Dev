using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    
    Animator _doorAnimator;
    void Start()
    {
        
        _doorAnimator = GetComponent<Animator>();
    }

    public void doorUpdate(bool _doorOpened)
    {
        if(_doorOpened)
        {
            _doorAnimator.SetTrigger("closed");
            //onUp.Invoke();
        }
        else
        {
            _doorAnimator.SetTrigger("open");
            //onDown.Invoke();
        }
    }
}
