using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDev.Scripts.Oxygen; // Replace 'GameDev' with your actual project root namespace

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float standard_speed = 2f;
    float speed;

    float yscale;

    float HorizInput;
    float VertInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public float groundDrag;

    [Header("Camera attributes")]
    public Transform orientation;

    [Header("Inventory")]
    public GameObject InventoryObject;
    public UI_Inventory uiInvetory;
    public Inventory inventory;
    private bool showInventory;

    [Header("Slope handler")]
    public float maxSlope;
    public float playerHeight = 5;
    private RaycastHit slopeHit;

    [Header("UI")]
    Interactor interactor;

    [Header("Oxygen System")]
    public OxygenSystem oxygenSystem;
    public float runningOxygenCost = 2f;
    public float walkingOxygenCost = 0.5f;

    [Header("Sound System")]
    public float runningSoundRadius;
    public float walkingSoundRadius;
    SoundSourcesController soundSources;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        speed = standard_speed;
        yscale = transform.localScale.y;

        inventory = new Inventory();
        uiInvetory = InventoryObject.GetComponent<UI_Inventory>();
        showInventory = false;
        //im putting this into the input function
        uiInvetory.setInventory(inventory);

        interactor = gameObject.GetComponent<Interactor>();

        if (oxygenSystem != null)
        {
            Debug.Log("OxygenSystem is assigned in PlayerController.");
        }
        else
        {
            Debug.LogWarning("OxygenSystem is not assigned in PlayerController.");
        }
        soundSources = SoundSourcesController.GetInstance();
    }

    void FixedUpdate()
    {
        if (!interactor.inUI)
            MovePlayer();
    }

    void Update()
    {
        MyInput();
        SpeedControl();
        rb.drag = groundDrag;

        if (oxygenSystem != null)
        {
            // Adjust oxygen based on movement
            if (rb.velocity.magnitude > 0.1f) // Check if the player is moving
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // Running
                    oxygenSystem.DecreaseOxygen(runningOxygenCost * Time.deltaTime);
                    soundSources.CreateNewSoundSource(transform.position, runningSoundRadius);
                }
                else
                {
                    // Walking
                    oxygenSystem.DecreaseOxygen(walkingOxygenCost * Time.deltaTime);
                    soundSources.CreateNewSoundSource(transform.position, walkingSoundRadius);
                }
            }
        }
        else
        {
            // Debug.LogWarning("OxygenSystem is not assigned in PlayerController.");
        }
    }

    void MyInput()
    {
        HorizInput = Input.GetAxisRaw("Horizontal");
        VertInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey("left shift"))
        {
            speed = standard_speed * 2f;
        }
        else if (Input.GetKey("left ctrl"))
        {
            speed = standard_speed * .5f;
        }
        else
        {
            speed = standard_speed;
        }

        //for the actual scale of the crouch collider
        if (Input.GetKeyDown("left ctrl"))
        {
            transform.localScale = new Vector3(transform.localScale.x, yscale / 2, transform.localScale.z);
            rb.AddForce(Vector3.down * 6f, ForceMode.Impulse);
        }
        else if (Input.GetKeyUp("left ctrl"))
        {
            speed = standard_speed;
            transform.localScale = new Vector3(transform.localScale.x, yscale, transform.localScale.z);
            rb.AddForce(Vector3.down * 6f, ForceMode.Impulse);
        }

        //toggle inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!gameObject.GetComponent<Interactor>().isHolding)
            {
                toggleInventory();
            }
        }
    }

    public void toggleInventory()
    {
        showInventory = !showInventory;
        //if im in inventory, then i am in a UI
        if (showInventory)
            interactor.inUI = true;
        else
            interactor.inUI = false;
        InventoryObject.SetActive(showInventory);

        if (showInventory)
        {
            uiInvetory.refresh();
        }
    }

    void MovePlayer()
    {
        moveDirection = (orientation.forward * VertInput) + (orientation.right * HorizInput);

        if (OnSlope())
            rb.AddForce(GetSlopeMoveDirection() * speed * 10f, ForceMode.Force);
        else
        {
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        //limiting speed on slope
        if (OnSlope())
        {
            if (rb.velocity.magnitude > speed)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
        }
        else
        { //not on slope
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limit velocity
            if (flatVel.magnitude > speed)
            {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.1f))
        {
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