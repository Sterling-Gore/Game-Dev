using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;

public class AlienController : MonoBehaviour
{
    public GameObject player;
    GameObject endingScreen;

    float speed;
    bool isAwareOfPlayer = false;

    public float standardSpeed;
    public float awarenessRadius;
    public float attackRadius;
    public float mentalDelay = 5.0f;
    public float timeSinceLastThought;
    public int spaceDiscretization = 3;

    JobHandle thoughtHandle;
    bool hasStartedThinking = false;
    List<Vector3> pathToPlayer = new();
    public int pathIndex = 0;
    const int maxPathLength = 30000;

    Rigidbody playerRb;
    AlienBrain.PathGraph pathGraph;
    NativeArray<Vector3> memoryBuffer;
    NativeArray<int> memoryBufferLengthUsed;

    public bool canSeePlayer = false;

    void Start()
    {
        endingScreen = GameObject.Find("EndingScreen");

        speed = standardSpeed;
        playerRb = player.GetComponent<Rigidbody>();
        pathGraph = new AlienBrain.PathGraph(GameObject.Find("AlienPathNodes").transform);
        memoryBuffer = new(maxPathLength, Allocator.Persistent);
        memoryBufferLengthUsed = new(1, Allocator.Persistent);

        thoughtHandle.Complete();
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
            RoamAround();
        }
        else
            MoveTowardsPlayer();
    }

    void OnDestroy()
    {
        memoryBuffer.Dispose();
        memoryBufferLengthUsed.Dispose();
        pathGraph.Dispose();
    }

    void KeepUpright()
    {
        transform.rotation.Set(0, 0, 0, 0);
    }

    void AnnounceAwarenessOfPlayer()
    {

    }

    void RoamAround()
    {

    }

    void MoveTowardsPlayer()
    {
        var directionToPlayer = player.transform.position - transform.position;
        var distanceToPlayer = Vector3.Magnitude(directionToPlayer);

        Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out RaycastHit j, distanceToPlayer - .1f);
        Debug.DrawRay(transform.position, directionToPlayer);
        if (j.rigidbody == playerRb)
        {
            canSeePlayer = true;
            if (distanceToPlayer < attackRadius)
                AttackPlayer();
            else
                GoStraightToPlayer(directionToPlayer / distanceToPlayer);
        }
        else
        {
            canSeePlayer = false;
            CalculatePathPeriodically();
            FollowPathToPlayer();
        }
    }

    void AttackPlayer()
    {
        var gameOver = endingScreen.transform.GetChild(0).gameObject;
        gameOver.SetActive(true);
    }

    void GoStraightToPlayer(Vector3 directionToPlayer)
    {
        var movement = speed * Time.deltaTime * new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        transform.position += Vector3.ClampMagnitude(movement, 1f);
        var playerPosition = player.transform.position;
        playerPosition.y = transform.position.y;
        transform.LookAt(playerPosition);

        //don't go back to path just recalculate path
        pathIndex = 0;
    }

    void FollowPathToPlayer()
    {
        if (pathIndex < 0 || pathToPlayer.Count == 0)
            return;
        var nextPathPoint = pathToPlayer[pathIndex];
        var directionToPath = nextPathPoint - transform.position;
        Debug.DrawRay(transform.position, directionToPath, Color.cyan);
        pathToPlayer.ForEach(point => Debug.DrawRay(point, Vector3.up * 100, Color.blue));

        directionToPath.y = 0;
        var distanceToPath = directionToPath.magnitude;
        directionToPath.Normalize();
        var dPos = speed * Time.deltaTime;
        var movement = dPos * directionToPath;
        transform.position += Vector3.ClampMagnitude(movement, 1f);

        nextPathPoint.y = transform.position.y;
        transform.LookAt(nextPathPoint);

        if (distanceToPath <= dPos)//close enough to path point, move on
            pathIndex--;
    }

    void CalculatePathPeriodically()
    {
        timeSinceLastThought += Time.deltaTime;

        if (ShouldRecalculatePath())
        {
            if (hasStartedThinking)
            {
                thoughtHandle.Complete();
                CopyPathToPlayer();
            }

            timeSinceLastThought = 0;

            thoughtHandle = new AlienBrain.AlienThought
            {
                maxPathLength = maxPathLength,
                spaceDiscretization = spaceDiscretization,
                alienPosition = transform.position,
                playerPosition = player.transform.position,
                pathToPlayer = memoryBuffer,
                graph = pathGraph.WithPositions(transform.position, player.transform.position),
                lengthOfPath = memoryBufferLengthUsed
            }.Schedule();

            hasStartedThinking = true;
        }
    }

    void CopyPathToPlayer()
    {
        pathToPlayer.Clear();
        pathToPlayer.Capacity = memoryBufferLengthUsed[0];
        for (int i = 0; i < memoryBufferLengthUsed[0]; ++i)
            pathToPlayer.Add(memoryBuffer[i]);

        pathIndex = pathToPlayer.Count - 1;
    }

    bool ShouldRecalculatePath()
    {
        if (timeSinceLastThought < mentalDelay || pathIndex > 0)
            return false;
        if (pathToPlayer.Count == 0)
            return true;
        return true;

        // var pathEnd = pathToPlayer[0];
        // var directionToPlayer = player.transform.position - pathEnd;
        // Physics.Raycast(pathEnd, directionToPlayer, out RaycastHit j, directionToPlayer.magnitude);

        // return j.rigidbody != playerRb;
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