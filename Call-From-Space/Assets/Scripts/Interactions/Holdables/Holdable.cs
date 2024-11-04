using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Holdable : Interactable
{
    //for grabing the interactir script
    public GameObject HoldObject;
    public GameObject player;
    public Transform holdPos;
    public Camera cam;
    //private bool currentlyHolding;
    private Rigidbody ObjRb;

    protected bool localHold; //checks if you are holding object locally attached to script

    string objName = "";
    float weight = 0;



    Interactor interactor;

    // Start is called before the first frame update
    void Start()
    {
        Physics.IgnoreCollision(HoldObject.GetComponent<Collider>(), player.transform.Find("Player Model").GetComponent<Collider>(), true);
        //currentlyHolding = false;
        ObjRb = HoldObject.GetComponent<Rigidbody>();

        //this gets the interactor script from the player, that way we can turn off interactins while holding an object
        interactor = player.GetComponent<Interactor>();
        localHold = false;


        objName = OverrideName();
        weight = OverrideWeight();
    }

    public abstract string OverrideName();
    public abstract float OverrideWeight();




    public override string GetDescription()
    {
        
        return (objName);
    }

    public override void Interact()
    {
        pickUpObject();
        
    }


    //functions for the holdable
    protected void MoveObject()
    {
        HoldObject.transform.position = holdPos.transform.position ;
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    protected void pickUpObject()
    {
        HoldObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        HoldObject.transform.rotation = holdPos.transform.rotation;
        interactor.isHolding = true;
        localHold = true;
        ObjRb.isKinematic = true;
        HoldObject.transform.position = new Vector3(0,0,0);
        HoldObject.transform.parent = holdPos.transform;
        //---being used in start()---
        //Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider.GetComponent<Collider>(), true);

        //TransformDirection(Vector3.forward)

        
    }

    protected void DropObject()
    {
        ObjRb.isKinematic = false;
        HoldObject.transform.parent = null;
        interactor.isHolding = false;
        localHold = false;


    }

    protected void StopClipping() //called when dropping item
    {
        var clipRange = Vector3.Distance(HoldObject.transform.position, cam.transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            HoldObject.transform.position = cam.transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}

