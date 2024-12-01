using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenScreenInteraction : Interactable
{

    public enum Generator{
        A,
        B,
        C
    }
    public GameObject GenUI;
    public GameObject player;

    public Generator generatorType;


    public override string GetDescription()
    {
        
        return ("Press [E] to interact with the Generator Screen");
    }

    public override void Interact()
    {
        switch (generatorType)
        {
            case Generator.A:
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle1(1);
                break;
            case Generator.B:
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(5);
                break;
            case Generator.C:
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle3(1);
                break;
            default:
                break;

        }
        //GenUI.GetComponent<GeneratorGame>().interactor.inUI = true;
        GenUI.SetActive(true);
        player.GetComponent<Interactor>().inUI = true;
        player.GetComponent<PlayerController>().Set_UI_Value(1);
        
    }
}
