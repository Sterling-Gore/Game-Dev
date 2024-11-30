using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public float interactionDistance = 3f;
    public Camera mainCam;
    public TMPro.TextMeshProUGUI interactionText;

    public bool isHolding;
    public bool inUI;
    public string holdingName; //this is the name of the current object the player is holding

    public int ignoreAlienCollisionLayer;

    void Start()
    {
        holdingName = "";
        isHolding = false;
        inUI = false;
        ignoreAlienCollisionLayer = (
            1 << LayerMask.NameToLayer("AlienCollisionLayer")
        );

        int DefaultCollisionLayer = (
            1 << LayerMask.NameToLayer("Ignore Raycast")
        );

        ignoreAlienCollisionLayer = ~(ignoreAlienCollisionLayer | DefaultCollisionLayer);
    }

    // Update is called once per frame
    void Update()
    {
        //bool successfulHit = false;
        if (!isHolding)
            holdingName = "";

        if (!inUI)
        {
            bool successfulHit = false;
            //Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 5, Color.green);



            if (Physics.Raycast(ray, out hit, interactionDistance, ignoreAlienCollisionLayer))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                Holdable holdable = hit.collider.GetComponent<Holdable>();

                if (interactable != null && (!isHolding || interactable.special))
                {
                    HandleInteraction(interactable);
                    interactionText.text = interactable.GetDescription();
                    successfulHit = true;
                    if (holdable != null)
                        holdingName = holdable.objName;
                }



            }
            if (!successfulHit)
            {
                interactionText.text = "";
            }

        }
        else
        {
            interactionText.text = "";
        }



    }

    void HandleInteraction(Interactable interactable)
    {
        switch (interactable.interactionType)
        {
            case Interactable.InteractionType.Click:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
                break;
            case Interactable.InteractionType.Hold:
                if (Input.GetKey(KeyCode.E))
                {
                    interactable.Interact();
                }
                break;
            case Interactable.InteractionType.Minigame:
                //make a mini-game here
                break;

            default:
                throw new System.Exception("Unsupported type of interactable");

        }
    }

    public void inside_UI()
    {
        inUI = true;
    }
    public void outside_UI()
    {
        inUI = false;
    }
}
