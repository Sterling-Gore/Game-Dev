using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCellHoldable : Holdable
{

    public float WeightOfObject = 5f;
    public string NameOfObject = "Fuel Cell";

    void Update()
    {
        if(localHold) 
        {
            MoveObject();
            if (Input.GetKeyDown(KeyCode.Mouse1) ) 
            {
                StopClipping();
                DropObject();
            }
        }
    }

    public override string OverrideName()
    {
        return NameOfObject;
    }
    public override float OverrideWeight()
    {
        return WeightOfObject;
    }
}
