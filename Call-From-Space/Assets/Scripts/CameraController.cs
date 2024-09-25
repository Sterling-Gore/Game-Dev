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


    // Start is called before the first frame update
    void Start()
    {
       //offset = transform.position - player.transform.position; 
       //Cursor.visible = false;
       //Cursor.lockState = CursorLockMode.Locked;


    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        float inputY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += inputX;
        xRotation -= inputY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


    }

   

    
}
