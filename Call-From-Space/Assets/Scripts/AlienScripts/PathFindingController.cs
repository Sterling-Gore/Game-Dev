using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class PathFindingController
{
    AlienController alien;
    List<Vector3> pathToPlayer = new();
    public int pathIndex = 0;

    const int maxPathLength = 30000;
    float timeSinceLastThought;
    bool hasStartedThinking = false;

    JobHandle thoughtHandle;

    NativeArray<Vector3> memoryBuffer;
    NativeArray<int> memoryBufferLengthUsed;

    public PathFindingController(AlienController alien)
    {
        this.alien = alien;
        memoryBuffer = new(maxPathLength, Allocator.Persistent);
        memoryBufferLengthUsed = new(1, Allocator.Persistent);

        thoughtHandle.Complete();
    }

    public void FollowPath()
    {
        if (pathIndex < 0 || pathToPlayer.Count == 0)
            return;
        var nextPathPoint = pathToPlayer[pathIndex];
        var directionToPath = nextPathPoint - alien.transform.position;

        Debug.DrawRay(alien.transform.position, directionToPath, Color.cyan);
        pathToPlayer.ForEach(point => Debug.DrawRay(point, Vector3.up * 100, Color.blue));

        if (alien.MoveTowards(nextPathPoint))
            pathIndex--;
    }

    public void CalculatePathPeriodically()
    {
        timeSinceLastThought += Time.deltaTime;

        if (ShouldRecalculatePath())
        {
            if (hasStartedThinking)
                CopyPathToPlayer();

            thoughtHandle = new AlienThought
            {
                maxPathLength = maxPathLength,
                alienPosition = alien.transform.position,
                playerPosition = alien.player.transform.position,
                pathToPlayer = memoryBuffer,
                graph = alien.pathGraph.WithPositions(alien.transform.position, alien.player.transform.position),
                lengthOfPath = memoryBufferLengthUsed
            }.Schedule();

            timeSinceLastThought = 0;
            hasStartedThinking = true;
        }
    }

    bool ShouldRecalculatePath() => timeSinceLastThought > alien.mentalDelay || pathIndex <= 0;
    public void Recalculate() => pathIndex = 0;

    void CopyPathToPlayer()
    {
        thoughtHandle.Complete();

        pathToPlayer.Clear();
        pathToPlayer.Capacity = memoryBufferLengthUsed[0];
        for (int i = 0; i < memoryBufferLengthUsed[0]; ++i)
            pathToPlayer.Add(memoryBuffer[i]);

        pathIndex = pathToPlayer.Count - 1;
    }

    public void Dispose()
    {
        thoughtHandle.Complete();
        memoryBuffer.Dispose();
        memoryBufferLengthUsed.Dispose();
    }
}
public struct AlienThought : IJob
{
    public int maxPathLength;
    public Vector3 alienPosition, playerPosition;
    public PathGraph graph;

    public NativeArray<Vector3> pathToPlayer;
    public NativeArray<int> lengthOfPath;
    public void Execute()
    {
        var newPathToPlayer = new List<Vector3>();

        new AlienPathFinding
        {
            yLevel = graph.YLevel,
            playerPosition = playerPosition,
            graph = graph.ToDictionary()
        }.FindPath(newPathToPlayer, alienPosition, maxPathLength);

        for (int i = 0; i < newPathToPlayer.Count; ++i)
            pathToPlayer[i] = newPathToPlayer[i];
        lengthOfPath[0] = newPathToPlayer.Count;
    }
}

class AlienPathFinding : AStar<Vector3>
{
    public float yLevel;
    public Vector3 playerPosition;
    public Dictionary<Vector3, HashSet<Vector3>> graph;

    protected override void Neighbors(Vector3 p, List<Vector3> neighbors) => neighbors.AddRange(graph[new(p.x, yLevel, p.z)]);
    protected override float Cost(Vector3 p1, Vector3 p2) => Mathf.Pow(p1.x - p2.x, 2) + Mathf.Pow(p1.z - p2.z, 2);
    protected override float Heuristic(Vector3 p) => Vector3.Distance(p, playerPosition);
}
