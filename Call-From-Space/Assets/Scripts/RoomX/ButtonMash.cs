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
        bool flag = true;
        while(flag)
        {
            Debug.Log("MEOW");
            yield return new WaitForSeconds(1f);
            //while(!Input.GetKeyDown(KeyCode.V))
            //{
                yield return null;
                if(Input.GetKeyDown(KeyCode.V)){
                        //stuff to happen on input
                }
                //Debug.Log("Yipee");
                flag = false;
            //}
        }
    }
}
