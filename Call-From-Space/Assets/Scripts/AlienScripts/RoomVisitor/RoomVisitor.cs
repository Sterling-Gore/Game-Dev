
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class RoomVisitor : Loadable
{
    [Header("Room Visitor")]
    public int roamingRoomMemory;
    public List<Room> recentRooms;
    public HashSet<Room> preferedRooms;
    public List<Room> rooms;
    public Room currentRoom;
    public Room nextRoom;
    public void Init()
    {
        recentRooms = new(roamingRoomMemory);
        FindCurrentRoom();
    }

    public void FindCurrentRoom() =>
        currentRoom = FindClosestRoomTo(transform.position);

    public Room FindClosestRoomTo(Vector3 target)
    {
        float minDist = 100;
        Room closest = null;
        foreach (Room room in rooms)
        {
            var dist = Vector3.Distance(room.center.position, target);
            if (dist < minDist)
            {
                minDist = dist;
                closest = room;
            }
        }
        return closest;
    }

    public void SetRoomVisited()
    {
        Debug.Log($"{name} visited {nextRoom.center.position}");
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

    virtual public void UpdateRooms(List<Transform> sections)
    {
        rooms = new();
        foreach (Transform section in sections)
            foreach (Transform room in section)
                rooms.Add(room.gameObject.GetComponent<Room>());

        preferedRooms = new(rooms);
        recentRooms.ForEach(room => preferedRooms.Remove(room));
    }

    public bool ChooseNextRoom(System.Func<Room, (Room room, float distance)> roomSelector)
    {
        var possibleNextRooms = currentRoom.NeighboringRooms(rooms);
        var groupedNextRooms = possibleNextRooms
            .GroupBy(p => preferedRooms.Contains(p))
            .ToDictionary(g => g.Key, g => g.ToList());
        var preferedNextRooms = groupedNextRooms.GetValueOrDefault(true, new());
        var unPreferedNextRooms = groupedNextRooms.GetValueOrDefault(false, new());
        List<Room> roomsToChooseFrom;

        if (preferedNextRooms.Count > 0)
            roomsToChooseFrom = preferedNextRooms;
        else
            roomsToChooseFrom = unPreferedNextRooms;

        if (roomsToChooseFrom.Count == 0)
            return false;

        var roomsAndDistances = roomsToChooseFrom
            .Select(roomSelector)
            .ToList();
        roomsAndDistances.Sort((rd1, rd2) => rd1.distance.CompareTo(rd2.distance));
        var randomIndex = Mathf.FloorToInt(Mathf.Pow(Random.value, 2) * roomsToChooseFrom.Count) % roomsToChooseFrom.Count;
        nextRoom = roomsAndDistances[randomIndex].room;

        return true;
    }

    public override void Load(JObject state) { }

    public override void Save(ref JObject state) { }
}