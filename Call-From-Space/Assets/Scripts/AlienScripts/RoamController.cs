using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RoamController : MonoBehaviour
{
    AlienController alien;

    public int roamingRoomMemory;
    public LayerMask groundLayer;
    public float timeLookingAround = 0;
    float timeToLookAroundFor = 0;
    public float timeRoamingAroundRoom = 0;
    public bool isLookingAround = false;
    public bool isRoamingRoom = true;
    public bool hasNextRoamSpot = false;
    Vector3 nextRoamSpot;
    public bool isRotating = false;
    public float rotatingTime = 0;

    bool ClockWise = true;


    List<Room> recentRooms;
    HashSet<Room> preferedRooms;
    List<Room> rooms = new();
    Room currentRoom;
    Room nextRoom;

    public Vector3 nextPos;
    public Vector3 pos;
    public string roomName;
    public string nextRoomName;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Init(AlienController alien)
    {
        this.alien = alien;
        recentRooms = new(roamingRoomMemory);
        preferedRooms = rooms.ToHashSet();

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
        if (isRoamingRoom)
            RoamAroundRoom();
        else
            WalkToNextRoom();
    }

    void WalkToNextRoom()
    {
        alien.nextSpeed = alien.walkSpeed;
        alien.pathFinder.FollowPath();
        nextPos = nextRoom.center.position;
        pos = transform.position;
        pos.y = nextPos.y;
        nextRoomName = nextRoom.name;
        roomName = currentRoom.name;

        var dist = Vector3.Distance(pos, nextPos);
        if (dist <= alien.turnRadius + .1)
            SetRoomVisited();
    }

    void SetRoomVisited()
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
        isRoamingRoom = true;
    }

    void RoamAroundRoom()
    {
        alien.nextSpeed = alien.tiredSpeed;
        if (timeRoamingAroundRoom > currentRoom.timeToRoamAroundFor)//change
            ChooseNextRoom();
        else
        {
            if (isLookingAround)
                LookAroundRoom();
            else
            {
                if (hasNextRoamSpot)
                    MoveToRoamSpot();
                else
                    ChooseNextRoamSpot();

                timeRoamingAroundRoom += Time.deltaTime;
            }
        }
    }

    void MoveToRoamSpot()
    {
        if (isRotating)
            Rotate();
        else
        {
            alien.MoveTowards(nextRoamSpot);
            var target = nextRoamSpot;
            target.y = transform.position.y;
            if (Vector3.Distance(transform.position, target) <= alien.turnRadius)
            {
                hasNextRoamSpot = false;
                var shouldLookAround = Random.value > .90f; // look around every 10th time
                if (shouldLookAround)
                {
                    timeToLookAroundFor = Random.Range(3, 4);
                    isLookingAround = true;
                }
            }
        }
    }

    void Rotate()
    {
        if (rotatingTime > .9)
        {
            isRotating = false;
            rotatingTime = 0;
        }
        else
        {
            var target = nextRoamSpot;
            target.y = transform.position.y;
            var targetRotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * alien.turnSpeed);
            rotatingTime += Time.deltaTime;
        }
    }

    void LookAroundRoom()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isLookingAround", true);
        if (timeLookingAround > timeToLookAroundFor)
        {
            isLookingAround = false;
            timeLookingAround = 0;
            animator.SetBool("isLookingAround", false);
        }
        else
        {
            int angle = 200;
            if (timeLookingAround < timeToLookAroundFor / 4 || timeLookingAround > timeToLookAroundFor * 3 / 4)
                angle = -200;

            alien.head.Rotate(Vector3.forward, angle * Time.deltaTime / timeToLookAroundFor);
            animator.SetBoneLocalRotation(HumanBodyBones.Neck, alien.head.rotation);
            timeLookingAround += Time.deltaTime;
        }
    }

    void ChooseNextRoom()
    {
        var alienPos = transform.position;
        alienPos.y = alien.pathGraph.YLevel;
        PathNode alienPosNode = new() { pos = alienPos, radius = 0 };


        var possibleNextRooms = currentRoom.NeighboringRooms(rooms);
        var preferedNextRooms = possibleNextRooms
          .Where(p => preferedRooms.Contains(p))
          .ToList();
        List<Room> roomsToChooseFrom;

        if (preferedNextRooms.Count > 0)
            roomsToChooseFrom = preferedNextRooms;
        else
            roomsToChooseFrom = recentRooms;

        // give closer points to player a higher probability 
        var roomsAndDistances = roomsToChooseFrom
            .Select(room => (room, Vector3.Distance(room.center.position, alien.player.transform.position)))
            .ToList();
        roomsAndDistances.Sort((rd1, rd2) => rd1.Item2.CompareTo(rd2.Item2));
        var randomIndex = Mathf.FloorToInt(Mathf.Pow(Random.value, 2) * roomsToChooseFrom.Count) % roomsToChooseFrom.Count;
        nextRoom = roomsAndDistances[randomIndex].room;

        alien.pathFinder.CalculatePathNow(nextRoom.center.position);

        Debug.Log($"done looking, next room: {nextRoom.name} at {nextRoom.center.position}");

        isRoamingRoom = false;
        isLookingAround = false;
        timeRoamingAroundRoom = 0;
        timeLookingAround = 0;
        hasNextRoamSpot = false;
    }

    void ChooseNextRoamSpot()
    {
        var nodes = currentRoom.roamNodes;
        int nodeIdx = 0;

        if (Random.value > 0.9998f) //change direction ~2 times every 10 seconds
            ClockWise = !ClockWise;

        if (ClockWise)
            nodeIdx = (nodeIdx + Random.Range(1, nodes.Count / 2)) % nodes.Count;
        else
            nodeIdx = (nodeIdx - Random.Range(1, nodes.Count / 2)) % nodes.Count;

        var node = nodes[nodeIdx];
        float radius = node.radius * Mathf.Sqrt(Random.value);
        float theta = Random.value * 2 * Mathf.PI;
        float x = node.pos.x + radius * Mathf.Cos(theta);
        float z = node.pos.z + radius * Mathf.Sin(theta);
        nextRoamSpot = new Vector3(x, node.pos.y, z);
        Debug.DrawLine(nextRoamSpot, Vector3.up * 100, Color.green, 100);
        var pos = transform.position;
        pos.y = node.pos.y;
        //check to make sure nothing in way
        if (!Physics.Raycast(nextRoamSpot, pos, Vector3.Distance(nextRoamSpot, pos)))
        {
            hasNextRoamSpot = true;
            isRoamingRoom = true;
        }
    }
    public void UpdateRooms(Transform newSection)
    {
        foreach (Transform room in newSection)
            rooms.Add(room.gameObject.GetComponent<Room>());
    }
}

/*
    set path graph as center points and to points and all others are used in roam
    use navmesh in MoveTowards !check performance!
*/