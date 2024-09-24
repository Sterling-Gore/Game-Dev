using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holdable : Interactable
{
    //for grabing the interactir script
    public GameObject playerInteractor;
    //for grabing the player's collision mesh
    public GameObject playerCollider;
    public Transform holdPos;
    public Camera cam;
    public float weight = 0f;
    //private bool currentlyHolding;
    private Rigidbody ObjRb;
    private int LayerNumber;



    Interactor interactor;

    // Start is called before the first frame update
    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("HoldLayer");
        Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider.GetComponent<Collider>(), true);
        //currentlyHolding = false;
        ObjRb = gameObject.GetComponent<Rigidbody>();

        //this gets the interactor script from the player, that way we can turn off interactins while holding an object
        interactor = playerInteractor.GetComponent<Interactor>();
    }

    // Update is called once per frame
    void Update()
    {
        if(interactor.isHolding) 
        {
            MoveObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) ) 
            {
                StopClipping();
                DropObject();
            }
        }
    }

    public override string GetDescription()
    {
        
        return ("");
    }

    public override void Interact()
    {
        pickUpObject();
        
    }


    //functions for the holdable
    void MoveObject()
    {
        transform.position = holdPos.transform.position;
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    void pickUpObject()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = holdPos.transform.rotation;
        interactor.isHolding = true;
        ObjRb.isKinematic = true;
        gameObject.transform.position = new Vector3(0,0,0);
        gameObject.transform.parent = holdPos.transform;
        gameObject.layer = LayerNumber;
        //---being used in start()---
        //Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider.GetComponent<Collider>(), true);

        //TransformDirection(Vector3.forward)

        
    }

    void DropObject()
    {
        //---being used in start()---
        //Physics.IgnoreCollision(GetComponent<Collider>(), playerCollider.GetComponent<Collider>(), false);
        gameObject.layer = 0;
        ObjRb.isKinematic = false;
        gameObject.transform.parent = null;
        interactor.isHolding = false;


    }

    void StopClipping() //called when dropping item
    {
        var clipRange = Vector3.Distance(gameObject.transform.position, cam.transform.position);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            gameObject.transform.position = cam.transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }
}

