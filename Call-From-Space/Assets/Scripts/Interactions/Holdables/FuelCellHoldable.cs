using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCellHoldable : Holdable
{



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


}
