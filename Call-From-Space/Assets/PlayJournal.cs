using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayJournal : Interactable
{
    // Start is called before the first frame update
    public AudioSource Audio;

    public override string GetDescription()
    {
        
        return ("Pick up last journal entry");
    }

    public override void Interact()
    {
        
        Audio.enabled = true;
    }
}
