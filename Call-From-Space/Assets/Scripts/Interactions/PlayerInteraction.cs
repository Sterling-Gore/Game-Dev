using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
   public float playerReach = 3f;
   Interactable currentInteractable;
    // Update is called once per frame
    void Update()
    {
        checkInteraction();
        if(Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void checkInteraction()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out hit, playerReach))
        {
            if (hit.collider.tag == "Interactable") //looks for an interactable tag that is on interactable objects
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if (currentInteractable && newInteractable != currentInteractable)
                {
                    //currentInteractable.DisableOutline;
                }

                if (newInteractable.enabled)
                {
                    setNewCurrentInteractable(newInteractable);
                }
                else{
                    DisableCurrentInteractable(); //if new interactable is not enabled
                }
            }
            else{
                DisableCurrentInteractable(); //if the object is not an interactable object
            }
        }
        else//if nothing is in reach
        {
            DisableCurrentInteractable();
        }
    }

    void setNewCurrentInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
        //currentInteractable.EnableOutline;
    }

    void DisableCurrentInteractable(){
        if(currentInteractable)
        {
            //currentInteractable.DisableOutline;
            currentInteractable = null;
        }
    }
}
