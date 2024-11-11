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

    public string objName = "";
    public float weight = 0;

    public GameObject ItemGlow;
    bool DoneGlowing;



    Interactor interactor;

    // Start is called before the first frame update
    void Start()
    {
        DoneGlowing = false;
        Physics.IgnoreCollision(HoldObject.GetComponent<Collider>(), player.transform.Find("Player Model").GetComponent<Collider>(), true);
        //currentlyHolding = false;
        ObjRb = HoldObject.GetComponent<Rigidbody>();

        //this gets the interactor script from the player, that way we can turn off interactins while holding an object
        interactor = player.GetComponent<Interactor>();
        localHold = false;

    }




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
        if(!DoneGlowing)
            ItemGlow.SetActive(false);
        GameObject screen = player.GetComponent<PlayerController>().standardScreen;
        screen.transform.Find("Controls").gameObject.SetActive(false);
        screen.transform.Find("ControlsHolding").gameObject.SetActive(true);

        switch (objName)
        {
            case "Lighter":
                screen.transform.Find("ControlsHolding").Find("SPECIAL_EFFECTS").Find("Lighter").gameObject.SetActive(true);
                break;
            default:
                break;
        }

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
        if(!DoneGlowing)
            ItemGlow.SetActive(true);
        GameObject screen = player.GetComponent<PlayerController>().standardScreen;
        screen.transform.Find("Controls").gameObject.SetActive(true);
        screen.transform.Find("ControlsHolding").gameObject.SetActive(false);
        screen.transform.Find("ControlsHolding").Find("SPECIAL_EFFECTS").Find("Lighter").gameObject.SetActive(false);

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

    public void StopGlowEffect()
    {
        DoneGlowing = true;
        ItemGlow.SetActive(false);
    }
}

