using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //OLD MOVEMENT
    /*
    public float standard_speed;
    float speed;
    private Rigidbody rb;
    public Transform camera;

    void Start()
    {
        speed = standard_speed;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
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
    
        //(old)transform.position += Vector3.ClampMagnitude(Movement, 1f);
        rb.AddForce(transform.position + Vector3.ClampMagnitude(Movement, 1f));
    }
    
    
    void FixedUpdate()
    {   
        if (Input.GetKey("left shift"))
        {
            speed = standard_speed * 2f;
        }
        else if (Input.GetKey("c"))
        {
            speed = standard_speed * .5f;
        }
        else{
            speed = standard_speed;
        }

        
        
        Movement();
    }
    */

    [Header("Movement")]
    public float standard_speed = 2f;
    float speed;

    float yscale;

    float HorizInput;
    float VertInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public float groundDrag;

    public Transform orientation;

    [Header("Slope handler")]
    public float maxSlope;
    public float playerHeight = 5;
    private RaycastHit slopeHit;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        speed = standard_speed;
        yscale = transform.localScale.y;
    }

    void FixedUpdate()
    {
        MovePlayer();
        
    }

    void Update()
    {
        MyInput();
        SpeedControl();
        rb.drag = groundDrag;
    }

    void MyInput()
    {
        HorizInput = Input.GetAxisRaw("Horizontal");
        VertInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey("left shift"))
        {
            speed = standard_speed * 1.5f;
        }
        else if (Input.GetKey("left ctrl"))
        {
            speed = standard_speed * .5f;
        }
        else{
            speed = standard_speed;
        }
        if (Input.GetKeyDown("left ctrl"))
        {
            transform.localScale = new Vector3(transform.localScale.x, yscale/2, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp("left ctrl")){
            speed = standard_speed;
            transform.localScale = new Vector3(transform.localScale.x, yscale, transform.localScale.z);
        }
        
    }

    void MovePlayer()
    {
        moveDirection = (orientation.forward * VertInput) + (orientation.right * HorizInput);

        if (OnSlope())
            rb.AddForce(GetSlopeMoveDirection() * speed * 10f, ForceMode.Force);

            
        else{
            rb.AddForce(moveDirection.normalized * speed  * 10f, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {

        //limiting speed on slope
        if(OnSlope())
        {
            if(rb.velocity.magnitude > speed)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
                
        }
        else{ //not on slope
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity
            if(flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); 
            }
        }
    }

    bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.1f)){
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlope && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

}
