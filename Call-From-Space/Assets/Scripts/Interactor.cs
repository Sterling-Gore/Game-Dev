using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface _Interactable {
    public void Interact();
}

//SCRAPPED
public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("E")){
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward); //this casts a ray from the position of the camera to where the camera is looking
            if(Physics.Raycast(r, out RaycastHit hitInfo, InteractRange)){
                if(hitInfo.collider.gameObject.TryGetComponent(out _Interactable interactObj)){
                    interactObj.Interact();
                }
            }
        }
        
    }
}
