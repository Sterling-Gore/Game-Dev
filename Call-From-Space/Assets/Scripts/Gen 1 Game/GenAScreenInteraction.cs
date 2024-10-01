using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenAScreenInteraction : Interactable
{

    public GameObject GenUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override string GetDescription()
    {
        
        return ("Generator Screen");
    }

    public override void Interact()
    {
        GenUI.SetActive(true);
        GenUI.GetComponent<GeneratorGame>().interactor.inUI = true;
    }
}
