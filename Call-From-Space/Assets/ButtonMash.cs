using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMash : Interactable
{
    public override string GetDescription()
    {
        return ("Start Button Mash");
    }

    public override void Interact()
    {
        Debug.Log("START");
        StartCoroutine(StartButtonMash());
    }

    IEnumerator StartButtonMash()
    {
        while(true)
        {
            Debug.Log("MEOW");
            yield return new WaitForSeconds(1f);
            while(!Input.GetKeyDown(KeyCode.V))
            {
                //Debug.Log("Yipee");
            }
        }
    }
}
