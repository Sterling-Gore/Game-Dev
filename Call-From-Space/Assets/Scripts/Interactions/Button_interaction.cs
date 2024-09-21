using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Button_interaction : Interactable
{
    bool _doorOpened;
    Animator _doorAnimator;
    
    //used to change the color of the shuttle_light
    public Material myMaterial;
    public Texture redTexture;
    public Texture greenTexture;


    void Start()
    {   
        myMaterial.mainTexture = redTexture;
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
            myMaterial.mainTexture = greenTexture;
        }
        _doorOpened = true;
        
    }
    


}
