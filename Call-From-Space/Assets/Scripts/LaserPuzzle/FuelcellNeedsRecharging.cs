using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelcellNeedsRecharging : Interactable
{
    public override string GetDescription()
    {
        return "Fuel Cell Needs To Be Charged";
    }

    public override void Interact()
    {
        //nothing
    }
}
