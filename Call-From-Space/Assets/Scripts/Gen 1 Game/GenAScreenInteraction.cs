using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenAScreenInteraction : Interactable
{

    public GameObject GenUI;
    public GameObject player;

    public override string GetDescription()
    {
        
        return ("Generator Screen");
    }

    public override void Interact()
    {
        //GenUI.GetComponent<GeneratorGame>().interactor.inUI = true;
        GenUI.SetActive(true);
        player.GetComponent<Interactor>().inUI = true;
        player.GetComponent<PlayerController>().Set_UI_Value(1);
        
    }
}
