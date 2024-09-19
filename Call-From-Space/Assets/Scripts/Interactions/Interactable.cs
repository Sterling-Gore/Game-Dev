using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    public enum InteractionType{
        Click,
        Hold,
        Minigame
    }

    public InteractionType interactionType;

    public abstract string GetDescription();
    public abstract void Interact();

    /*
    //Outline outline;
    public string message;

    public UnityEvent onInteraction;

    // Start is called before the first frame update
    void Start()
    {
        //outline = GetComponent<Outline>();
        //DisableOutline();
    }


    public void Interact()
    {
        onInteraction.Invoke();
    }


    
    //public void DisableOutline()
    //{
    //    outline.enabled = false;
    //}
    //public void EnableOutline()
    //{
    //    outline.enabled = true;
    //}
    */
    

    
}
