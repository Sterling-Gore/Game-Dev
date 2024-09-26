using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //public GameObject player;
    public float mouseSensitivity;

    public Transform orientation;
    float xRotation;
    float yRotation;

    [Header("UI")]
    //this grabs the player object which has the interactor script on it
    //the interactor script hold the bool value for if in UI
    public GameObject Player_for_interactor;
    Interactor interactor;


    // Start is called before the first frame update
    void Start()
    {

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        interactor = Player_for_interactor.GetComponent<Interactor>();

    }

    void Update()
    {
        if(interactor.inUI)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            float inputX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
            float inputY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

            yRotation += inputX;
            xRotation -= inputY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
        


    }

   

    
}
