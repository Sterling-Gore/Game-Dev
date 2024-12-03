using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TaskList : Loadable
{

    int GenPuzzle1State = 0;
    int GenPuzzle2State = 0;
    int GenPuzzle3State = 0;
    int LaserPuzzleState = 0;

    public struct TaskData
    {
        public string text;
        public bool isLargeText;
        public TaskData(string text, bool isLargeText)
        {
            this.text = text;
            this.isLargeText = isLargeText;
        }
    }

    GameObject TaskContainer;
    GameObject TaskTemplate;
    public HashSet<TaskData> tasks;
    // Start is called before the first frame update
    void Start()
    {
        TaskContainer = transform.Find("TaskContainer").gameObject;
        TaskTemplate = TaskContainer.transform.Find("TaskTemplate").gameObject;

        tasks = new HashSet<TaskData>();
        AddTask("Find Your Way Into The Station", true);
    }

    // Update is called once per frame
    public void AddTask(string text, bool isLargeText)
    {
        TaskData task = new TaskData(text, isLargeText);
        tasks.Add(task);
        if (TaskContainer.activeSelf)
            refresh();
    }

    public void DeleteTask(string text)
    {
        foreach (TaskData task in tasks)
        {
            if (task.text == text)
            {
                tasks.Remove(task);
                if (TaskContainer.activeSelf)
                    refresh();
                return;
            }
        }
    }


    public void refresh()
    {
        foreach (Transform child in TaskContainer.transform)
        {
            if (child == TaskTemplate.transform) continue;
            Destroy(child.gameObject);
        }
        int ypos = 0;
        //45 for small task and 60 for large task
        float distance = 45f;

        foreach (TaskData task in tasks)
        {
            //Debug.Log(task);
            RectTransform TaskRectTransform = Instantiate(TaskTemplate.transform, TaskContainer.transform).GetComponent<RectTransform>();
            TaskRectTransform.gameObject.SetActive(true);
            Vector2 additionalPosition = new Vector2(0, ypos * distance);
            TaskRectTransform.anchoredPosition = additionalPosition;
            TaskRectTransform.Find("TaskText").GetComponent<TMPro.TextMeshProUGUI>().text = task.text;
            ypos -= 1;

            if (task.isLargeText)
                distance = 60f;
            else
                distance = 45f;
        }
    }




    //THESE ARE THE STATES:
    public void GenPuzzle1(int state)
    {
        if (GenPuzzle1State < state)
        {
            switch (state)
            {
                case 1: //activate the genA screen
                    AddTask("Search The Station For a Passcode", true);
                    DeleteTask("Find a Power Generator");
                    break;
                case 2:  //pick up the sticky note
                    AddTask("Inspect Sticky Note In Inventory", true);
                    DeleteTask("Find a Power Generator");
                    break;
                case 3: //inspect the sticky note
                    AddTask("Enter The Passcode Into The Generator", true);
                    DeleteTask("Search The Station For a Passcode");
                    DeleteTask("Inspect Sticky Note In Inventory");
                    DeleteTask("Find a Power Generator");
                    break;
                case 4: //enter the passcode into the generator
                    AddTask("Find a Fuel Cell For The Generator", true);
                    DeleteTask("Search The Station For a Passcode");
                    DeleteTask("Inspect Sticky Note In Inventory");
                    DeleteTask("Find a Power Generator");
                    DeleteTask("Enter The Passcode Into The Generator");
                    break;
                case 5: //enter the fuel cell into the generator
                    DeleteTask("Find a Fuel Cell For The Generator");
                    DeleteTask("Search The Station For a Passcode");
                    DeleteTask("Inspect Sticky Note In Inventory");
                    DeleteTask("Find a Power Generator");
                    DeleteTask("Enter The Passcode Into The Generator");
                    AddTask("ESCAPE BACK TO THE SHUTTLE!", true);
                    break;
                default:
                    break;

            }
            GenPuzzle1State = state;
        }
    }

    public void Explosion()
    {
        DeleteTask("ESCAPE BACK TO THE SHUTTLE!");
        AddTask("Find an Escape Pod", false);
        AddTask("Find The Next Power Generator", false);
    }

    

    public void UndoExplosion()
    {
        AddTask("ESCAPE BACK TO THE SHUTTLE!", true);
        DeleteTask("Find an Escape Pod");
        DeleteTask("Find The Next Power Generator");
    }

    public void GenPuzzle2(int state)
    {
        if (GenPuzzle2State < state)
        {
            switch (state)
            {
                case 1: //look at the gooped wall
                    AddTask("Find a Way To Destroy The Foreign Material", true);
                    break;
                case 2:  //pick up the keys
                    DeleteTask("Find a Way To Destroy The Foreign Material");
                    AddTask("Find a Way To Destroy The Foreign Material", true);
                    AddTask("Find The Locker For The Keys", true);
                    break;
                case 3: //use keys on locker
                    DeleteTask("Find The Locker For The Keys");
                    DeleteTask("Find a Way To Destroy The Foreign Material");
                    AddTask("Use The Lighter To Burn The Foreign Material", true);
                    break;
                case 4: //use lighter to burn the goop
                    DeleteTask("Find The Locker For The Keys");
                    DeleteTask("Find a Way To Destroy The Foreign Material");
                    DeleteTask("Use The Lighter To Burn The Foreign Material");
                    break;
                case 5: //look into the generator
                    DeleteTask("Find The Locker For The Keys");
                    DeleteTask("Find a Way To Destroy The Foreign Material");
                    DeleteTask("Use The Lighter To Burn The Foreign Material");

                    DeleteTask("Find The Next Power Generator");
                    AddTask("Solve The Puzzle For The Generator", true);
                    break;
                case 6: //solve the simon says puzzle
                    DeleteTask("Find The Locker For The Keys");
                    DeleteTask("Find a Way To Destroy The Foreign Material");
                    DeleteTask("Use The Lighter To Burn The Foreign Material");

                    DeleteTask("Find The Next Power Generator");
                    DeleteTask("Solve The Puzzle For The Generator");
                    AddTask("Find a Fuel Cell For The Generator", true);
                    break;
                case 7: //enter the fuel cell into the generator
                    DeleteTask("Find The Locker For The Keys");
                    DeleteTask("Find a Way To Destroy The Foreign Material");
                    DeleteTask("Use The Lighter To Burn The Foreign Material");

                    DeleteTask("Find The Next Power Generator");
                    DeleteTask("Solve The Puzzle For The Generator");
                    DeleteTask("Find a Fuel Cell For The Generator");
                    AddTask("Find The Last Power Generator", true);
                    break;
                default:
                    break;

            }
            GenPuzzle2State = state;
        }
    }



    public void GenPuzzle3(int state)
    {
        if (GenPuzzle3State < state)
        {
            switch (state)
            {
                case 1: //activate the genC screen
                    AddTask("Search the room to find a clue for the puzzle", true);
                    DeleteTask("Find The Last Power Generator");
                    break;
                case 2:  //solve the genC puzzle
                    DeleteTask("Find The Last Power Generator");
                    DeleteTask("Search the room to find a clue for the puzzle");
                    AddTask("Find a Fuel Cell For The Generator", true);
                    break;
                case 3: //enter the fuel cell into the generator
                    DeleteTask("Find The Last Power Generator");
                    DeleteTask("Search the room to find a clue for the puzzle");
                    DeleteTask("Find a Fuel Cell For The Generator");

                    DeleteTask("Grab The Charged Fuel Cell");
                    AddTask("Find Room X...", false);
                    break;
                default:
                    break;

            }
            GenPuzzle3State = state;
        }
    }

    public void RoomX()
    {
        DeleteTask("Find Room X...");
    }

    public void LaserPuzzle(int state)
    {
        if (LaserPuzzleState < state)
        {
            switch (state)
            {
                case 1: //walk into collider
                    AddTask("Recharge The Fuel Cell", false);
                    break;
                case 2:  //look at the missing reflectors
                    DeleteTask("Recharge The Fuel Cell");
                    AddTask("Recharge The Fuel Cell", true);
                    AddTask("Find Missing Reflectors And Plug Them In", true);
                    break;
                case 3: //plug in both reflectors
                    DeleteTask("Find Missing Reflectors And Plug Them In");
                    AddTask("Solve The Laser Pathing", false);
                    break;
                case 4://solve the puzzle
                    DeleteTask("Find Missing Reflectors And Plug Them In");
                    DeleteTask("Solve The Laser Pathing");
                    DeleteTask("Recharge The Fuel Cell");
                    AddTask("Grab The Charged Fuel Cell", false);


                    break;
                default:
                    break;

            }
            LaserPuzzleState = state;
        }
    }

    public override void Load(JObject state)
    {
        var me = state[fullName];
        GenPuzzle1State = (int)me["GenPuzzle1State"];
        GenPuzzle2State = (int)me["GenPuzzle2State"];
        GenPuzzle3State = (int)me["GenPuzzle3State"];
        LaserPuzzleState = (int)me["LaserPuzzleState"];
    }

    public override void Save(ref JObject state)
    {
        state[fullName] = new JObject()
        {
            ["GenPuzzle1State"] = GenPuzzle1State,
            ["GenPuzzle2State"] = GenPuzzle2State,
            ["GenPuzzle3State"] = GenPuzzle3State,
            ["LaserPuzzleState"] = LaserPuzzleState
        };
    }
}
