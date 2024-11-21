using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RoamController : MonoBehaviour
{
    AlienController alien;

    public int roamingRoomMemory;
    public float timeToLookAroundFor = 0;
    public Vector3 nextRoamSpot;

    public List<Room> recentRooms;
    public HashSet<Room> preferedRooms;
    public List<Room> rooms = new();
    public Room currentRoom;
    public Room nextRoom;

    public int nodeIdx = 0;

    public Animator animator;

    LinkedList<State> prevStates = new();
    const int prevStatesCapacity = 5;
    public State curState;
    public States state;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Init(AlienController alien)
    {
        this.alien = alien;
        recentRooms = new(roamingRoomMemory);
        preferedRooms = rooms.ToHashSet();
        FindCurrentRoom();

        curState = new RoamingRoom(this, alien);
    }

    public void FindCurrentRoom()
    {
        float minDist = 100;
        foreach (Room room in rooms)
        {
            var dist = Vector3.Distance(room.center.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                currentRoom = room;
            }
        }
    }

    public void RoamAround()
    {
        animator.SetBool("isWalking", true);
        curState.Update();
    }
    public void SetRoomVisited()
    {
        Debug.Log($"visited {nextRoom.center.position}");
        if (preferedRooms.Remove(nextRoom))
        {
            if (recentRooms.Count >= roamingRoomMemory)
            {
                preferedRooms.Add(recentRooms.First());
                recentRooms.RemoveAt(0);
            }
        }
        else
            recentRooms.Remove(nextRoom);
        recentRooms.Add(nextRoom);

        currentRoom = nextRoom;
        nextRoom = null;
    }
    public void UpdateRooms(List<Transform> newSections)
    {
        rooms = new();

        foreach (Transform newSection in newSections)
            foreach (Transform room in newSection)
                rooms.Add(room.gameObject.GetComponent<Room>());
    }

    public void GoToNextState(States state)
    {
        this.state = state;
        prevStates.AddLast(curState);
        while (prevStates.Count > prevStatesCapacity)
            prevStates.RemoveFirst();
        switch (state)
        {
            case States.LookingAround:
                curState = new LookingAround(this, alien);
                break;
            case States.RoamingRoom:
                curState = new RoamingRoom(this, alien);
                break;
            case States.Rotating:
                curState = new Rotating(this, alien);
                break;
            case States.MovingToNextRoom:
                curState = new MovingToNextRoom(this, alien);
                break;
        }
    }
    public void GoToPreviousState()
    {
        curState = prevStates.Last();
        prevStates.RemoveLast();
        state = curState.state;
    }
}

public enum States
{
    LookingAround, RoamingRoom, Rotating, MovingToNextRoom
}
public abstract class State
{
    public States state;
    public RoamController roamer;
    public AlienController alien;
    public State(RoamController roamer, AlienController alien)
    {
        this.roamer = roamer;
        this.alien = alien;
    }
    abstract public void Update();

    abstract public void OnStuck();
}
class LookingAround : State
{
    public const States state = States.LookingAround;
    public float timeLookingAround = 0;
    public LookingAround(RoamController roamer, AlienController alien) : base(roamer, alien)
    {
        Debug.Log("looking around");
    }

    override public void Update()
    {
        roamer.animator.SetBool("isWalking", false);
        roamer.animator.SetBool("isLookingAround", true);
        if (timeLookingAround > roamer.timeToLookAroundFor)
        {
            roamer.GoToPreviousState();
            roamer.animator.SetBool("isLookingAround", false);
            alien.PlayRandomIdleAudio();
        }
        else
        {
            int angle = 200;
            if (timeLookingAround < roamer.timeToLookAroundFor / 4 || timeLookingAround > roamer.timeToLookAroundFor * 3 / 4)
                angle = -200;

            alien.head.Rotate(Vector3.forward, angle * Time.deltaTime / roamer.timeToLookAroundFor);
            timeLookingAround += Time.deltaTime;
        }
    }

    public override void OnStuck() { }
}
class RoamingRoom : State
{
    public const States state = States.RoamingRoom;
    bool ClockWise = true;
    int timesLookedAround = 0;
    const int timesAllowedToLookAround = 2;
    public float timeRoamingAroundRoom = 0;
    public bool hasNextRoamSpot = false;
    public RoamingRoom(RoamController roamer, AlienController alien) : base(roamer, alien)
    {
        Debug.Log("Roaming around");
    }
    override public void Update()
    {
        alien.nextSpeed = alien.tiredSpeed;
        if (timeRoamingAroundRoom > roamer.currentRoom.timeToRoamAroundFor)
            roamer.GoToNextState(States.MovingToNextRoom);
        else
        {
            alien.PlayRandomWalkAudio();
            if (hasNextRoamSpot)
                MoveToRoamSpot();
            else
                ChooseNextRoamSpot();

            timeRoamingAroundRoom += Time.deltaTime;
        }
    }
    public override void OnStuck()
    {
        ChooseNextRoamSpot();
    }

    void ChooseNextRoamSpot()
    {
        var nodes = roamer.currentRoom.roamNodes;

        if (Random.value > 0.9998f) //change direction ~2 times every 10 seconds
            ClockWise = !ClockWise;

        if (ClockWise)
            roamer.nodeIdx = mod(roamer.nodeIdx + Random.Range(1, nodes.Count / 2), nodes.Count);
        else
            roamer.nodeIdx = mod(roamer.nodeIdx - Random.Range(1, nodes.Count / 2), nodes.Count);

        var node = nodes[roamer.nodeIdx];
        var spot2d = new Vector2(node.pos.x, node.pos.z) + (Random.insideUnitCircle * node.radius);
        roamer.nextRoamSpot = new Vector3(spot2d.x, node.pos.y, spot2d.y);
        Debug.DrawLine(roamer.nextRoamSpot, Vector3.up * 100, Color.green, 100);
        var pos = alien.transform.position;
        pos.y = node.pos.y;

        //check to make sure nothing in way
        var dist = Vector3.Distance(roamer.nextRoamSpot, pos);
        if (!Physics.Raycast(pos, (roamer.nextRoamSpot - pos) / dist, dist, PathGraph.layerMask))
        {
            Debug.DrawLine(roamer.nextRoamSpot, pos, Color.magenta, 100);
            hasNextRoamSpot = true;
            alien.pathFinder.CalculatePathNow(roamer.nextRoamSpot);
        }

        static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }
    }

    void MoveToRoamSpot()
    {
        alien.pathFinder.FollowPath();
        var target = roamer.nextRoamSpot;
        target.y = alien.transform.position.y;
        if (Vector3.Distance(alien.transform.position, target) <= alien.turnRadius)
        {
            hasNextRoamSpot = false;
            var shouldLookAround = Random.value > .90f; // look around every 10th time
            if (shouldLookAround && timesLookedAround < timesAllowedToLookAround)
            {
                roamer.timeToLookAroundFor = Random.Range(3, 4);
                roamer.GoToNextState(States.LookingAround);
                timesLookedAround++;
            }
        }
    }
}
class Rotating : State
{
    public const States state = States.Rotating;
    public float rotatingTime = 0;
    public Rotating(RoamController roamer, AlienController alien) : base(roamer, alien)
    {
        Debug.Log("rotating around");
    }

    override public void Update()
    {
        if (rotatingTime > .9)
        {
            roamer.GoToPreviousState();
            rotatingTime = 0;
        }
        else
        {
            var target = roamer.nextRoamSpot;
            target.y = alien.transform.position.y;
            var targetRotation = Quaternion.LookRotation(target - alien.transform.position);
            alien.transform.rotation = Quaternion.Lerp(alien.transform.rotation, targetRotation, Time.deltaTime * alien.turnSpeed);
            rotatingTime += Time.deltaTime;
        }
    }
    public override void OnStuck() { }
}
class MovingToNextRoom : State
{
    public const States state = States.MovingToNextRoom;
    public Vector3 nextPos;
    public Vector3 pos;
    int timesLookedForRoom = 0;
    public MovingToNextRoom(RoamController roamer, AlienController alien) : base(roamer, alien)
    {
        ChooseNextRoom();
        Debug.Log($"moving to next room: {roamer.nextRoom}");
    }
    override public void Update()
    {
        alien.PlayRandomWalkAudio();
        alien.nextSpeed = alien.walkSpeed;
        alien.pathFinder.FollowPath();
        nextPos = roamer.nextRoom.center.position;
        pos = alien.transform.position;
        pos.y = nextPos.y;

        var dist = Vector3.Distance(pos, nextPos);
        if (dist <= alien.turnRadius + .1)
        {
            roamer.SetRoomVisited();
            roamer.GoToNextState(States.RoamingRoom);
        }
        else if (alien.pathFinder.pathIndex == -1)
        {
            if (timesLookedForRoom == 2)
                roamer.FindCurrentRoom();
            else if (timesLookedForRoom > 2)
            {
                OnStuck();
                return;
            }
            ChooseNextRoom();
            timesLookedForRoom++;
        }
    }
    public override void OnStuck()
    {
        roamer.GoToNextState(States.RoamingRoom);
    }
    void ChooseNextRoom()
    {
        var alienPos = alien.transform.position;
        alienPos.y = alien.pathGraph.YLevel;
        PathNode alienPosNode = new() { pos = alienPos, radius = 0 };


        var possibleNextRooms = roamer.currentRoom.NeighboringRooms(roamer.rooms);
        var preferedNextRooms = possibleNextRooms
          .Where(p => roamer.preferedRooms.Contains(p))
          .ToList();
        List<Room> roomsToChooseFrom;

        if (preferedNextRooms.Count > 0)
            roomsToChooseFrom = preferedNextRooms;
        else
            roomsToChooseFrom = roamer.recentRooms;

        // give closer points to player a higher probability 
        var roomsAndDistances = roomsToChooseFrom
            .Select(room => (room, Vector3.Distance(room.center.position, alien.player.transform.position)))
            .ToList();
        roomsAndDistances.Sort((rd1, rd2) => rd1.Item2.CompareTo(rd2.Item2));
        var randomIndex = Mathf.FloorToInt(Mathf.Pow(Random.value, 2) * roomsToChooseFrom.Count) % roomsToChooseFrom.Count;
        roamer.nextRoom = roomsAndDistances[randomIndex].room;

        alien.pathFinder.CalculatePathNow(roamer.nextRoom.center.position);

        Debug.Log($"done looking, next room: {roamer.nextRoom.name} at {roamer.nextRoom.center.position}");
    }
}