using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : Loadable
{

    public enum InteractionType
    {
        Click,
        Hold,
        Minigame
    }
    public bool special = false;

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

    public override void Load(JObject state) =>
        LoadTransform(state);

    public override void Save(ref JObject state) =>
        SaveTransform(ref state);
}
