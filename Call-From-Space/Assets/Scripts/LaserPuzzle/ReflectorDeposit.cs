using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorDeposit : Interactable
{
    public GameObject HoldableReflector1;
    public GameObject HoldableReflector2;
    public GameObject PlugedInReflector;
    public GameObject player;
    
    void OnTriggerEnter(Collider other)
    {
        if (/*other.CompareTag("FuelCell")*/ other == HoldableReflector1.GetComponent<Collider>() && !player.GetComponent<Interactor>().isHolding){  
            HoldableReflector1.SetActive(false);
            PlugedInReflector.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (/*other.CompareTag("FuelCell")*/ other == HoldableReflector2.GetComponent<Collider>() && !player.GetComponent<Interactor>().isHolding){  
            HoldableReflector2.SetActive(false);
            PlugedInReflector.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public override string GetDescription()
    {
        return "Plug in laser reflector";
    }

    public override void Interact()
    {

    }
}
