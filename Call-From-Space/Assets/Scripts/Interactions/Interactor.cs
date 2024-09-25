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

    

    void Start()
    {
        isHolding = false;
        inUI = false;
    }

    // Update is called once per frame
    void Update()
    {
        //bool successfulHit = false;

        if(!isHolding || !inUI)
        {
            bool successfulHit = false;
            //Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width/2f, Screen.height/2f, 0f));
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                

                if (interactable != null){
                    HandleInteraction(interactable);
                    interactionText.text = interactable.GetDescription();
                    successfulHit = true;
                }

                
            }
            if(!successfulHit)
            {
                interactionText.text = "";
            }
            
        }
        
            
        
    }

    void HandleInteraction(Interactable interactable)
    {
        switch (interactable.interactionType)
        {
            case Interactable.InteractionType.Click:
                if(Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
                break;
            case Interactable.InteractionType.Hold:
                if(Input.GetKey(KeyCode.E))
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
}
