using UnityEngine;

public class AlienController : MonoBehaviour
{
    public GameObject player;
    GameObject endingScreen;


    public float speed;
    public float attackRadius;
    public float awarenessRadius;
    public bool isAwareOfPlayer = false;
    public float mentalDelay = 5.0f;
    public float timeToLookAroundFor;

    Rigidbody playerRb;
    public PathGraph pathGraph;

    PathFindingController pathFinder;
    RoamController roamer;

    void Start()
    {
        endingScreen = GameObject.Find("EndingScreen");

        playerRb = player.GetComponent<Rigidbody>();
        pathGraph = new PathGraph(GameObject.Find("AlienPathNodes").transform);

        pathFinder = new(this);
        roamer = new(this);
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
            if (distanceToPlayer < attackRadius)
                AttackPlayer();
            else
                GoStraightToPlayer();
        }
        else
        {
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
        var directionToTarget = target - transform.position;
        directionToTarget.y = 0;
        var distanceToTarget = directionToTarget.magnitude;
        directionToTarget.Normalize();

        var dPos = speed * Time.deltaTime;
        var movement = dPos * directionToTarget;
        transform.position += Vector3.ClampMagnitude(movement, 1f);

        target.y = transform.position.y;
        transform.LookAt(target);

        return distanceToTarget <= dPos;
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
 * TODO: make attack, add roaming, make path finding for noise/specific events
 */