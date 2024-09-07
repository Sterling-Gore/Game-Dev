using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float mouseSensitivity = 2f;
    private float cameraVerticalRotation = 0f;
    // (dont really use it) bool lockedCursor = true;
    

    private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
       offset = transform.position - player.transform.position; 
       Cursor.visible = false;
       Cursor.lockState = CursorLockMode.Locked;

    }

    // Update is called once per frame
    void LateUpdate()
    {

        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraVerticalRotation -= inputY * mouseSensitivity * Time.deltaTime;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;
        
        player.transform.Rotate(Vector3.up * inputX* mouseSensitivity * Time.deltaTime);
        transform.position = player.transform.position + offset;
    }

    
}
