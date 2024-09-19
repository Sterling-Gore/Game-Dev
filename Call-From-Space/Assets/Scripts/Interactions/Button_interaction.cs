using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Button_interaction : Interactable
{
    bool _doorOpened;
    Animator _doorAnimator;
    //public UnityEvent onUp, onDown;
    //public GameObject door;
    //DoorMovement doorMove;

    // Start is called before the first frame update
    void Start()
    {   
        _doorOpened = false;
        //doorMove = door.GetComponent<DoorMovement>();
        _doorAnimator = GetComponent<Animator>();
        
    }

    

    public override string GetDescription()
    {
        if (!_doorOpened){
            return ("Press [E] to <color=red>open<color=red> the door.");
        }
        return ("");
    }

    public override void Interact()
    {
        if (!_doorOpened){
            _doorAnimator.SetTrigger("open");
        }
        _doorOpened = true;
        
    }
    


}
