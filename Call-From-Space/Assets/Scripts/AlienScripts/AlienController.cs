using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    public GameObject player;
    GameObject endingScreen;


    public float attackRadius;
    public float awarenessRadius;
    public bool isAwareOfPlayer = false;
    public float mentalDelay = 5.0f;
    public float timeToLookAroundFor;
    public int roamingPathPointMemory;
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

    void Start()
    {
        endingScreen = GameObject.Find("EndingScreen");

        playerRb = player.GetComponent<Rigidbody>();
        pathGraph = new PathGraph(new() {
            GameObject.Find("AlienPathNodesA").transform,
            GameObject.Find("AlienPathNodesB").transform,
            GameObject.Find("AlienPathNodesC").transform
        });

        pathFinder = new(this);
        roamer = new(this);

        head = GameObject.Find("spine.005").transform;
        curSpeed = nextSpeed = walkSpeed;
    }

    void Update()
    {
        KeepUpright();
        if (!isAwareOfPlayer)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < awarenessRadius)
            {
                isAwareOfPlayer = true;
                AnnounceAwarenessOfPlayer();
            }
            nextSpeed = walkSpeed;
            roamer.RoamAround();
        }
        else
            HuntPlayer();
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

    void AnnounceAwarenessOfPlayer()
    {

    }

    void HuntPlayer()
    {
        var directionToPlayer = player.transform.position - transform.position;
        var distanceToPlayer = Vector3.Magnitude(directionToPlayer);

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
        else
        {
            nextSpeed = walkSpeed;
            pathFinder.CalculatePathPeriodically();
            pathFinder.FollowPath();
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
        transform.Translate(Vector3.forward * dPos);

        return Vector3.Distance(transform.position, target) <= dPos;
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
 * Path will be calculated and alien will follow it until its close enough to player
 * Alien will use path nodes defined under the "AlienPathNodes" gameobject. 
 * 
 * Also if theres a ray from alien to player with nothing in between go straight
 * 
 * 
 * TODO: make attack, make path finding for noise/specific events
 */