using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    public SoundSource target;
    public GameObject player;
    GameObject endingScreen;

    [Header("Decision Making")]
    public float attackRadius;
    public bool heardSomething = false;
    public float mentalDelay = 5.0f;
    public int soundSourcesMemory;

    [Header("Roaming")]
    public float timeToLookAroundFor;
    public int roamingPathPointMemory;

    [Header("Movement")]
    public float turnRadius;
    public float turnSpeed;
    public float walkSpeed;
    public float runSpeed;
    public float tiredSpeed;
    public float restingPeriod;
    public float walkingStamina;
    public float runningStamina;

    float curSpeed;
    float nextSpeed;
    float timeInSpeed;

    Rigidbody playerRb;
    public PathGraph pathGraph;

    PathFindingController pathFinder;
    RoamController roamer;
    public Transform head;
    public List<SoundSource> blackListedSoundSources = new();
    public GameObject soundSource;
    bool justHeardSomething;
    PowerLevel powerLevelManager;
    int curPowerLevel = 0;

    void Start()
    {
        endingScreen = GameObject.Find("EndingScreen");

        playerRb = player.GetComponent<Rigidbody>();
        pathGraph = new PathGraph(new() {
            GameObject.Find("AlienPathNodesA").transform
        });

        pathFinder = new(this);
        roamer = new(this);

        head = GameObject.Find("spine.005").transform;
        curSpeed = nextSpeed = walkSpeed;
        SoundSourcesController.GetInstance().SubscribeToSoundSources(this);
        powerLevelManager = GameObject.Find("PowerManager").GetComponent<PowerLevel>();
        curPowerLevel = powerLevelManager.GetCurrentPowerLevel();
    }

    void Update()
    {
        //Debug
        soundSource.transform.position = target.position;

        KeepUpright();
        if (!heardSomething)
        {
            if (target != new SoundSource())
                AnnounceHeardSomething();
            nextSpeed = walkSpeed;
            roamer.RoamAround();
        }
        else
            HuntPlayer();

        AdjustPathGraph();
    }

    void OnDestroy()
    {
        pathFinder.Dispose();
        pathGraph.Dispose();
    }

    void KeepUpright()
    {
        transform.rotation.Set(0, 0, 0, 0);
    }

    void AnnounceHeardSomething()
    {
        justHeardSomething = true;
        heardSomething = true;
        Debug.Log("I hear you");
    }

    void HuntPlayer()
    {
        var directionToPlayer = player.transform.position - transform.position;
        var distanceToPlayer = directionToPlayer.magnitude;

        Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit j, distanceToPlayer - .1f);
        Debug.DrawRay(transform.position, directionToPlayer);
        if (j.rigidbody == playerRb)
        {
            nextSpeed = runSpeed;
            if (distanceToPlayer < attackRadius)
                AttackPlayer();
            else
                GoStraightToPlayer();
        }
        else if (!justHeardSomething && pathFinder.HasArrived())
        {
            blackListedSoundSources.Add(target);
            if (blackListedSoundSources.Count > soundSourcesMemory)
                blackListedSoundSources.RemoveAt(0);
            target = new SoundSource();
            heardSomething = false;
        }
        else
        {
            nextSpeed = walkSpeed;
            pathFinder.CalculatePathPeriodically();
            pathFinder.FollowPath();
        }
        justHeardSomething = false;
    }

    void AdjustPathGraph()
    {
        int powerLevel = powerLevelManager.GetCurrentPowerLevel();
        if (powerLevel != curPowerLevel)
        {
            if (powerLevel == 1)
                pathGraph = new PathGraph(new() {
                    GameObject.Find("AlienPathNodesA").transform,
                    GameObject.Find("AlienPathNodesB").transform
                });
            else if (powerLevel == 2)
                pathGraph = new PathGraph(new() {
                    GameObject.Find("AlienPathNodesA").transform,
                    GameObject.Find("AlienPathNodesB").transform,
                    GameObject.Find("AlienPathNodesC").transform
                });
            curPowerLevel = powerLevel;
        }
    }
    void AttackPlayer()
    {
        var gameOver = endingScreen.transform.GetChild(0).gameObject;
        gameOver.SetActive(true);
    }

    void GoStraightToPlayer()
    {
        MoveTowards(player.transform.position);
        //don't go back to path just recalculate path
        pathFinder.Recalculate();
    }

    /// <returns>true if reached target </returns>
    public bool MoveTowards(Vector3 target)
    {
        target.y = transform.position.y;
        var targetRotation = Quaternion.LookRotation(target - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        var dPos = CurSpeed() * Time.deltaTime;

        var prevPos = transform.position;
        transform.Translate(Vector3.forward * dPos);
        var curPos = transform.position;
        var closestPoint = GetClosestPointToLine(prevPos, (curPos - prevPos).normalized, prevPos - target);

        transform.position = Clamp(closestPoint, prevPos, curPos);

        return Vector3.Distance(transform.position, target) <= dPos;
    }

    Vector3 GetClosestPointToLine(Vector3 origin, Vector3 direction, Vector3 point2origin) =>
        origin - Vector3.Dot(point2origin, direction) * direction;

    Vector3 Clamp(Vector3 point, Vector3 start, Vector3 end)
    {
        var start2end = (end - start).normalized;
        var start2point = (point - start).normalized;
        if (start2point != start2end)
            return start;
        var end2point = (point - start).normalized;
        if (end2point == start2end)
            return end;
        return point;
    }

    float CurSpeed()
    {
        timeInSpeed += Time.deltaTime;
        if (curSpeed == runSpeed && timeInSpeed > runningStamina)
        {
            curSpeed = walkSpeed;
            timeInSpeed = 0;
        }
        else if (curSpeed == walkSpeed && timeInSpeed > walkingStamina)
        {
            curSpeed = tiredSpeed;
            timeInSpeed = 0;
        }
        else if (curSpeed == tiredSpeed && timeInSpeed > restingPeriod)
        {
            curSpeed = nextSpeed;
            timeInSpeed = 0;
        }

        return curSpeed;
    }
}

/*
 * Ideas:
 * To save on CPU usage, only run A* every now and then
 * Path will be calculated and alien will follow it until its close enough to target
 * Alien will use path nodes defined under the "AlienPathNodes" gameobject. 
 * 
 * Also if theres a ray from alien to target with nothing in between go straight
 * 
 * 
 * TODO: make attack, make path finding for noise/specific events
 */