
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class WatcherController : RoomVisitor
{
    [Header("Watcher")]
    public GameObject player;
    bool isStaringAtPlayer = false;
    bool shouldMove = false;
    public bool isVisible = false;
    Dictionary<Room, List<Vector3>> anchorPoints = new();
    int ignorePlayerLayer;
    public Transform neck;

    void Start()
    {
        Debug.Log("watcher awake");
        UpdatePowerLevel(PowerLevel.instance.currentPowerLevel);
        PowerLevel.instance.SubscribeToUpdates(powerLevel => UpdatePowerLevel(powerLevel));
        FindCurrentRoom();
        nextRoom = currentRoom;
        SetRoomVisited();
        ignorePlayerLayer = ~(
            1 << LayerMask.NameToLayer("PlayeyLayer")
        );
    }
    void Update()
    {
        if (isStaringAtPlayer)
            StareAtPlayer();
        if (shouldMove)
            GoToNextRoom();
        CheckIfVisible();
    }

    void CheckIfVisible()
    {
        var pointOnCamera = Camera.main.WorldToScreenPoint(transform.position);
        // Debug.Log(pointOnCamera);
        var isInCameraView = new Rect(0, 0, Screen.width, Screen.height).Contains(pointOnCamera);
        var playerPos = player.transform.position;
        var watcherPos = transform.position;
        playerPos.y += 1;
        watcherPos.y -= 1;
        var dir = playerPos - watcherPos;
        var nothingInTheWay = !Physics.Raycast(watcherPos, dir, dir.magnitude, ignorePlayerLayer);
        Debug.DrawRay(watcherPos, dir);
        var canBeSeen = nothingInTheWay && isInCameraView;
        if (!isVisible && canBeSeen)
            SetVisible();
        else if (isVisible && !canBeSeen)
            SetNotVisible();
    }

    void SetVisible()
    {
        Debug.Log($"can be seen by {Camera.current.name}");
        isStaringAtPlayer = true;
        shouldMove = false;
        isVisible = true;
    }

    void SetNotVisible()
    {
        Debug.Log($"can not be seen by {Camera.current.name}");
        isStaringAtPlayer = false;
        shouldMove = true;
        isVisible = false;
    }

    void UpdatePowerLevel(int powerLevel)
    {
        List<Transform> curSections = new(3);
        switch (powerLevel)
        {
            case 3:
                goto case 2;
            case 2:
                curSections.Add(GameObject.Find("SectionC").transform);
                goto case 1;
            case 1:
                curSections.Add(GameObject.Find("SectionB").transform);
                goto case 0;
            case 0:
                curSections.Add(GameObject.Find("SectionA").transform);
                break;
        }
        UpdateRooms(curSections);
    }

    override public void UpdateRooms(List<Transform> sections)
    {
        base.UpdateRooms(sections);
        foreach (Transform anchorPoint in GameObject.Find("AnchorPoints").transform)
        {
            var pos = anchorPoint.position;
            var closestRoom = FindClosestRoomTo(pos);
            if (anchorPoints.TryGetValue(closestRoom, out List<Vector3> v))
                v.Add(pos);
            else
                anchorPoints[closestRoom] = new() { pos };
        }
    }

    void StareAtPlayer()
    {
        neck.LookAt(player.transform);
        neck.Rotate(Vector3.forward, 180);
    }

    void GoToNextRoom()
    {
        if (ChooseNextRoom(room => (room, 1f)) && nextRoom != currentRoom)
        {
            shouldMove = false;
            var possiblePoints = anchorPoints.GetValueOrDefault(currentRoom, new());
            if (possiblePoints.Count > 0)
            {
                var randomIndex = Mathf.FloorToInt(Random.value * possiblePoints.Count) % possiblePoints.Count;
                transform.position = possiblePoints[randomIndex];
                SetRoomVisited();
                return;
            }
        }
        Debug.LogWarning("watcher could not find next room");
    }

    public override void Load(JObject state) =>
        LoadTransform(state);

    public override void Save(ref JObject state) =>
        SaveTransform(ref state);
}