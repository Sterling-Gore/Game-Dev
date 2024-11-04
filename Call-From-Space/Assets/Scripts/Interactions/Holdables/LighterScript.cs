using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterScript : Holdable
{
    // Start is called before the first frame update
    bool isOpen;
    public Animator animation;
    public GameObject Fire;

    public float WeightOfObject = 5f;
    public string NameOfObject = "Lighter";
    void Awake()
    {
        isOpen = false;
        Fire.SetActive(false);
    }

    // Update is called once per frame
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
            if (Input.GetKeyDown(KeyCode.Mouse0) ) 
            {
                animation.SetTrigger(isOpen ? "Closed" : "Open");
                isOpen = !isOpen;
                Fire.SetActive(isOpen);
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
