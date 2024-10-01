using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;

public class AlienController : MonoBehaviour
{
    public GameObject player;

    float speed;
    public float standardSpeed;

    public bool isAwareOfPlayer = false;
    public float awarenessRadius;
    public float attackRadius;

    public float mentalDelay = 5.0f;
    public float timeSinceLastThought;
    JobHandle thoughtHandle;
    bool hasStartedThinking = false;

    List<AlienBrain.Point> pathToPlayer = new();
    public int pathIndex = 0;
    const int maxPathLength = 30000;
    public int spaceDiscretization = 3;

    Rigidbody playerRb;

    AlienBrain.SpaceShipPoints pointsInSpaceShip;
    NativeArray<AlienBrain.Point> memoryBuffer;
    NativeArray<int> memoryBufferLengthUsed;

    GameObject endingScreen;
    
    void Start()
    {
        endingScreen = GameObject.Find("EndingScreen");
        endingScreen.SetActive(false);

        speed = standardSpeed;
        playerRb = player.GetComponent<Rigidbody>();
        pointsInSpaceShip = AlienBrain.GetPointsInSpaceShip(spaceDiscretization);
        memoryBuffer = new(maxPathLength, Allocator.Persistent);
        memoryBufferLengthUsed = new(1, Allocator.Persistent);
        
        thoughtHandle.Complete();
    }

    void Update()
    {
        if (!isAwareOfPlayer)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < awarenessRadius)
                isAwareOfPlayer = true;
        }
        else
            MoveTowardsPlayer();
    }

    void OnDestroy()
    {
        memoryBuffer.Dispose();
        memoryBufferLengthUsed.Dispose();
        pointsInSpaceShip.Dispose();
    }

    void MoveTowardsPlayer()
    {
        var directionToPlayer = player.transform.position - transform.position;
        var distanceToPlayer = Vector3.Magnitude(directionToPlayer);
        if (distanceToPlayer < attackRadius)
            AttackPlayer();
        else
        {
            //Physics.Raycast(transform.position, directionToPlayer, out RaycastHit j, distanceToPlayer - .1f);
            Physics.Raycast(transform.position+ (Vector3.up*.1f), directionToPlayer, out RaycastHit j, distanceToPlayer - .1f);
            //Debug.
            Debug.DrawRay(transform.position, directionToPlayer);
            if (j.rigidbody==playerRb)
            {// nothing in the way of alien and player
                directionToPlayer /= distanceToPlayer;
                var movement = speed * Time.deltaTime * new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
                transform.position += Vector3.ClampMagnitude(movement, 1f);
                //don't go back to path just recalculate path
                pathIndex = 0;
            }
            else
            {
                CalculatePathPeriodically();
                FollowPathToPlayer();
            }
        }
    }

    void AttackPlayer()
    {
        if(endingScreen.active) return;
        endingScreen.SetActive(true);
    }

    void FollowPathToPlayer()
    {
        if (pathIndex <= 0)
            return;
        var directionToPath = pathToPlayer[pathIndex].AsVector3() - transform.position;
        Debug.DrawRay(transform.position, directionToPath);
        Debug.DrawRay(new Vector3(), Vector3.up * 100);
        pathToPlayer.ForEach(point => Debug.DrawRay(point.AsVector3(), Vector3.up * 100, Color.blue));

        directionToPath.y = 0;
        var distanceToPath = directionToPath.magnitude;
        directionToPath.Normalize();
        var dPos = speed * Time.deltaTime;
        var movement = dPos * directionToPath;
        transform.position+= Vector3.ClampMagnitude(movement, 1f);

        
        if (distanceToPath<=dPos)//close enough to path point, move on
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
                Debug.Log("finished execution of path finding");
                Debug.Log(memoryBufferLengthUsed[0]);
                CopyPathToPlayer();
            }

            timeSinceLastThought = 0;

            int posX = Mathf.RoundToInt(transform.position.x);
            int posZ = Mathf.RoundToInt(transform.position.z);
            posX -= posX % spaceDiscretization;
            posZ -= posZ % spaceDiscretization;

            thoughtHandle = new AlienBrain.AlienThought
            {
                maxPathLength = maxPathLength,
                spaceDiscretization = spaceDiscretization,
                alienPosition = new Vector3(posX, 0, posZ),
                playerPosition = player.transform.position,
                pathToPlayer = memoryBuffer,
                pointsInSpaceShip = pointsInSpaceShip,
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
        if (timeSinceLastThought < mentalDelay || pathIndex > 1)
            return false;
        if (pathToPlayer.Count == 0)
            return true;
        
        var pathEnd = pathToPlayer[0].AsVector3();
        var directionToPlayer = player.transform.position - pathEnd;
        Physics.Raycast(pathEnd, directionToPlayer, out RaycastHit j, directionToPlayer.magnitude - .1f);
        
        return j.rigidbody != playerRb;
    }
}

/*
 * Ideas:
 * To save on CPU usage, only run A* every now and then
 * Path will be calculated and alien will follow it until its close enough to player
 * Alien will think of space as discrete 1x1x1 cubes
 * 
 * Also if theres a ray from alien to player with nothing in between go straight
 * 
 * Precalculate graph of spaceship to decrease use of raycast //kinda like navmesh but it works for jobs
 * 
 * TODO: make attack, add roaming, make path finding for noise/specific events
 */