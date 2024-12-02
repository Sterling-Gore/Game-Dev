using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDev.Scripts.Oxygen;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement; // Replace 'GameDev' with your actual project root namespace

public class PlayerController : Loadable
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

    [Header("Inventory and Camera UI")]
    public GameObject Inventory_and_camera_UI;
    public GameObject Inventory_UI_Object;
    public GameObject Camera_UI_Object;
    public GameObject Inspector_UI_Object;
    public GameObject TaskList_UI_Object;
    public UI_Inventory uiInventory;
    public Inventory inventory;
    private bool showInventory;

    public GameObject standardScreen;

    [Header("Slope handler")]
    public float maxSlope;
    public float playerHeight = 5;
    private RaycastHit slopeHit;

    [Header("UI")]
    Interactor interactor;
    public GameObject pauseMenuUI;
    public GameObject optionsMenu;
    public GameObject controlsMenu;
    public int UI_Value;

    [Header("Oxygen System")]
    public OxygenSystem oxygenSystem;
    public float runningOxygenCost = 3f;
    public float walkingOxygenCost = .75f;
    public float stationaryOxygenCost = 0.25f;

    public HealthSystem healthSystem;

    [Header("Puzzle UI")]
    public GameObject Generator1_UI;
    public GameObject Generator2_UI;
    public GameObject Generator3_UI;
    public GameObject Electrical_Pannel_UI;

    [Header("Sound System")]
    public float runningSoundRadius;
    public float walkingSoundRadius;

    [Header("Heartbeat Analyzer")]
    public GameObject heartbeat;
    AudioSource heartbeatAudioSource;
    public float maxHeartbeatDistance = 1f;
    public float minHeartbeatDistance = 0.5f;
    public float maxHeartbeatPitch = 2f;
    public float minHeartbeatPitch = 0.5f;

    void Start()
    {
        //start the game with no screens on
        UI_Value = 0;
        healthSystem = GetComponent<HealthSystem>();
        if (healthSystem == null)
        {
            Debug.LogError("HealthSystem component not found on player.");
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        //rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
        speed = standard_speed;
        yscale = transform.localScale.y;

        inventory = new Inventory();
        uiInventory = Inventory_UI_Object.GetComponent<UI_Inventory>();
        showInventory = false;
        //im putting this into the input function
        uiInventory.setInventory(inventory);

        interactor = gameObject.GetComponent<Interactor>();


        if (heartbeat != null)
            heartbeatAudioSource = heartbeat.GetComponent<AudioSource>();
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
            if (oxygenSystem.LosingOxygen)
            {
                // Adjust oxygen based on movement
                if (rb.velocity.magnitude > 0.1f) // Check if the player is moving
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // Running
                        oxygenSystem.DecreaseOxygen(runningOxygenCost);
                        SoundSourcesController.instance.CreateNewSoundSource(transform.position, runningSoundRadius);
                    }
                    else
                    {
                        // Walking
                        oxygenSystem.DecreaseOxygen(walkingOxygenCost);
                        SoundSourcesController.instance.CreateNewSoundSource(transform.position, walkingSoundRadius);
                    }
                }
                else
                {
                    // not moving
                    oxygenSystem.DecreaseOxygen(walkingOxygenCost);
                }
            }
        }
        var aliens = AlienController.aliens
        .Where(alien => alien.isAwareOfPlayer)
        .ToList();
        aliens.Sort((alien1, alien2) => Vector3.Distance(
            alien1.transform.position,
            transform.position
        ).CompareTo(Vector3.Distance(alien2.transform.position, transform.position)));
        if (aliens.Count > 0)
            PlayHeartbeatAudio(aliens[0]);
    }

    void MyInput()
    {
        HorizInput = Input.GetAxisRaw("Horizontal");
        VertInput = Input.GetAxisRaw("Vertical");

        if (!interactor.inUI)
        {
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
        }

        //toggle inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!interactor.isHolding)
            {
                toggle_INV_and_CAM(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (!interactor.isHolding)
            {
                toggle_INV_and_CAM(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ESCAPE();
        }
    }

    public void toggle_INV_and_CAM(bool isInventory)
    {
        //inside a UI
        if (UI_Value == 1)
        {
            ESCAPE();
        }
        Set_UI_Value(1);
        interactor.inUI = true;
        Inventory_and_camera_UI.SetActive(true);
        AudioManager.Instance.PlaySound("Menu-Open");
        // Debug.Log("ADD SOUND INVENTORY ENTER INVENTORY HERE");
        if (isInventory)
        {
            Inventory_UI_Object.SetActive(true);
            uiInventory.refresh();
            Camera_UI_Object.SetActive(false);
            Inspector_UI_Object.SetActive(false);
        }
        else
        {
            Camera_UI_Object.SetActive(true);
            Inventory_UI_Object.SetActive(false);
            Inspector_UI_Object.SetActive(false);
        }
    }

    //regular screen
    public void ToggleNonUIScreen(bool turnOn)
    {
        if (turnOn)
        {
            standardScreen.SetActive(true);
            TaskList_UI_Object.transform.Find("TaskContainer").gameObject.SetActive(true);
            TaskList_UI_Object.GetComponent<TaskList>().refresh();
        }
        else
        {
            standardScreen.SetActive(false);
            TaskList_UI_Object.transform.Find("TaskContainer").gameObject.SetActive(false);
        }
    }

    public void ESCAPE()
    {
        //if you are in inspector
        if (UI_Value == 2)
        {
            //go to the inventory screen
            Inspector_UI_Object.GetComponent<Inspector>().unloadInspector();
            toggle_INV_and_CAM(true);

        }
        //if you are in inventory or generator screen
        else if (UI_Value == 1)
        {
            Set_UI_Value(0);
            Inventory_and_camera_UI.SetActive(false);
            Generator1_UI.SetActive(false);
            Generator2_UI.SetActive(false);
            Generator3_UI.SetActive(false);
            Electrical_Pannel_UI.SetActive(false);
            interactor.inUI = false;
            Debug.Log("ADD SOUND INVENTORY EXIT INVENTORY HERE");
            AudioManager.Instance.PlaySound("Menu-Close");

        }
        // go to the escape menu
        else if (UI_Value == 0)
        {
            Set_UI_Value(-1);
            interactor.inUI = true;
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("ADD ESCAPE MENU ENTER HERE");
            AudioManager.Instance.PlaySound("Menu-Open");
        }
        //leaving the escape menu     if UI_Value == -1
        else if (UI_Value == -1)
        {
            Set_UI_Value(0);
            interactor.inUI = false;
            pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("ADD ESCAPE MENU LEAVE HERE");
            AudioManager.Instance.PlaySound("Menu-Close");
        }
        // leaving controls menu -> pause menu
        else if (UI_Value == -2)
        {
            Set_UI_Value(0);
            interactor.inUI = true;
            controlsMenu.SetActive(false);
            Time.timeScale = 0f;
        }
        //leaving the options menu     if UI_Value == -3
        else
        {
            Set_UI_Value(-1);
            optionsMenu.SetActive(false);
            TaskList_UI_Object.transform.Find("TaskContainer").gameObject.SetActive(false);
        }
    }

    public void Set_UI_Value(int val)
    {
        UI_Value = val;
        if (UI_Value == 0)
        {
            ToggleNonUIScreen(true);
        }
        else
            ToggleNonUIScreen(false);
        Debug.Log(UI_Value);

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

    public override void Load(JObject state)
    {
        LoadTransform(state);
        rb.position = transform.position;
        rb.angularVelocity = rb.velocity = Vector3.zero;

        state[fullName]["items"]
            .Select(fullName => FindItemInPath((string)fullName))
            .ToList()
            .ForEach(item =>
            {
                if (item == null)
                    return;
                if (item.isItem)
                    inventory.AddItem(item);
                else
                    inventory.AddJournal(item);
            });
    }

    public override void Save(ref JObject state)
    {
        SaveTransform(ref state);
        state[fullName]["items"] = new JArray(
            inventory.GetItemList()
                .Concat(inventory.GetJournalList())
                .Select(item => GetFullName(item.transform))
                .ToList()
        );
    }

    Item FindItemInPath(string fullName)
    {
        var idx = fullName.LastIndexOf('/');
        if (idx == -1)
            return null;
        var name = fullName[(idx + 1)..];
        var path = fullName[..idx];

        if (path == "")
        {
            var rootMatches = SceneManager.GetActiveScene().GetRootGameObjects().Where(obj => obj.name == name).ToList();
            if (rootMatches.Count == 0)
                return null;
            return rootMatches[0].GetComponent<Item>();
        }

        var parent = GameObject.Find(path).transform;

        if (parent == null)//parent is disabled
            return FindItemInPath(path);
        return FindInChildren(parent, name).GetComponent<Item>();
    }

    GameObject FindInChildren(Transform parent, string name)
    {
        if (parent.name == name)
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject found = FindInChildren(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

    void PlayHeartbeatAudio(AlienController alien)
    {
        if (Time.timeScale > 0 && !heartbeatAudioSource.isPlaying)
            heartbeatAudioSource.Play();

        float distance = Vector3.Distance(transform.position, alien.transform.position);
        if (distance > maxHeartbeatDistance)
            heartbeatAudioSource.volume = 0;
        else
        {
            float volume = Mathf.Lerp(1, 0, (distance - minHeartbeatDistance) / (maxHeartbeatDistance - minHeartbeatDistance));
            float pitch = Mathf.Lerp(maxHeartbeatPitch, minHeartbeatPitch, (distance - minHeartbeatDistance) / (maxHeartbeatDistance - minHeartbeatDistance));
            heartbeatAudioSource.volume = volume;
            heartbeatAudioSource.pitch = pitch;
        }
    }
}