using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorRotater : Interactable
{
    private Animator animation;
    int animationSequence;

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animator>();
        animationSequence = 0;
    }


    public override string GetDescription()
    {
        return "Turn Reflector";
    }

    public override void Interact()
    {
        changeAnimation();
    }

    void changeAnimation()
    {
        switch(animationSequence)
        {
            case 0:
                animation.SetTrigger("NorthWest");
                break;
            case 1:
                animation.SetTrigger("North");
                break;
            case 2:
                animation.SetTrigger("NorthEast");
                break;
            case 3:
                animation.SetTrigger("East");
                break;
            case 4:
                animation.SetTrigger("SouthEast");
                break;
            case 5:
                animation.SetTrigger("South");
                break;
            case 6:
                animation.SetTrigger("SouthWest");
                break;
            case 7:
                animation.SetTrigger("West");
                break;
        }
        animationSequence = (animationSequence + 1) % 8;
        //if(animationSequence == 8)
        //{
        //    animationSequence = 0;
        //}
    }
}
