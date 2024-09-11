using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float standard_speed;
    float speed;
    public new Transform camera;

    void Start()
    {
        speed = standard_speed;
    }

    void Movement()
    {
        //inputs
        float moveHorizontal = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
        float moveVertical = Input.GetAxis ("Vertical") * speed * Time.deltaTime;

        //camera directions
        Vector3 cameraForward = camera.forward;
        Vector3 cameraRight = camera.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        //creating relative camera direction
        Vector3 forwardRelative = moveVertical * cameraForward;
        Vector3 rightRelative = moveHorizontal * cameraRight;

        Vector3 movementDirection = forwardRelative + rightRelative;


        // (OLD) Vector3 Movement = new Vector3 (moveHorizontal, 0, moveVertical);
        Vector3 Movement = new Vector3 (movementDirection.x, 0, movementDirection.z);
    
        transform.position += Vector3.ClampMagnitude(Movement, 1f);
    }
    
    
    void FixedUpdate()
    {   
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = standard_speed * 2f;
        }
        else if (Input.GetKey(KeyCode.C))
        {
            speed = standard_speed * .5f;
        }
        else{
            speed = standard_speed;
        }
        
        Movement();
    }
}
