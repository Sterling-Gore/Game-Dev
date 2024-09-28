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
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;


    }

    // Update is called once per frame
    /*
    void LateUpdate()
    {
        //this is all for mouse movements
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraVerticalRotation -= inputY * mouseSensitivity * Time.deltaTime;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        
        player.transform.Rotate(Vector3.up * inputX* mouseSensitivity * Time.deltaTime);

        //this follows the player around
        transform.position = player.transform.position + offset;
    }
    */

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
