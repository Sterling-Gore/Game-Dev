using System.Collections.Generic;
using UnityEngine;
using System.Linq;

class MovingToNextRoom : State
{
    public const States state = States.MovingToNextRoom;
    public Vector3 nextPos;
    public Vector3 pos;
    int timesLookedForRoom = 0;
    bool foundNextRoom = false;
    public MovingToNextRoom(RoamController roamer, AlienController alien) : base(roamer, alien)
    {
        ChooseNextRoom();
        Debug.Log($"moving to next room: {roamer.nextRoom}");
    }
    override public void Update()
    {
        if (!foundNextRoom)
        {
            OnStuck();
            return;
        }
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
        foundNextRoom = roamer.ChooseNextRoom(room =>
            (room, Vector3.Distance(room.center.position, alien.player.transform.position))
        );
        if (foundNextRoom)
        {
            alien.pathFinder.CalculatePathNow(roamer.nextRoom.center.position);
            Debug.Log($"done looking, next room: {roamer.nextRoom.name} at {roamer.nextRoom.center.position}");
        }
        else
        {
            Debug.Log("couldn't find room to go to");
            roamer.GoToNextState(States.RoamingRoom);
        }
    }
}
