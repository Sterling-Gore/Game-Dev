using UnityEngine;

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
