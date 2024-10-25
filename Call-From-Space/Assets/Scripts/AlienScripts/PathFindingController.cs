using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.VisualScripting;

public class PathFindingController
{
    AlienController alien;
    List<Vector3> pathToTarget = new();
    public int pathIndex = -1;

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
        if (pathIndex < 0 || pathToTarget.Count == 0)
            return;
        var nextPathPoint = pathToTarget[pathIndex];
        var directionToPath = nextPathPoint - alien.transform.position;
        directionToPath.y = 0;
        var distanceToPath = directionToPath.magnitude;

        Debug.DrawRay(alien.transform.position, directionToPath, Color.cyan);
        Debug.DrawRay(nextPathPoint, -alien.turnRadius * directionToPath.normalized, Color.magenta);

        if (distanceToPath < alien.turnRadius)
            pathIndex--;
        alien.MoveTowards(nextPathPoint);
    }

    public void CalculatePathPeriodically()
    {
        timeSinceLastThought += Time.deltaTime;

        if (ShouldRecalculatePath())
        {
            if (hasStartedThinking)
                CopyPathToTarget();

            thoughtHandle = new AlienThought
            {
                maxPathLength = maxPathLength,
                alienPosition = alien.transform.position,
                targetPosition = alien.target.position,
                pathToTarget = memoryBuffer,
                graph = alien.pathGraph.WithPosition(alien.transform.position),
                lengthOfPath = memoryBufferLengthUsed
            }.Schedule();

            timeSinceLastThought = 0;
            hasStartedThinking = true;
        }
    }

    bool ShouldRecalculatePath() => (
        pathIndex < 0 ||
        timeSinceLastThought > alien.mentalDelay
    );

    void CopyPathToTarget()
    {
        pathToTarget.ForEach(point => Debug.DrawRay(point, Vector3.up * 100, Color.blue, alien.mentalDelay));

        thoughtHandle.Complete();

        pathToTarget.Clear();
        pathToTarget.Capacity = memoryBufferLengthUsed[0];
        for (int i = 0; i < memoryBufferLengthUsed[0]; ++i)
            pathToTarget.Add(memoryBuffer[i]);

        pathIndex = pathToTarget.Count - 1;
    }

    public void Recalculate() => pathIndex = 0;

    public bool HasArrived() =>
        pathIndex < 0 || (pathIndex == 0 && Vector3.Distance(pathToTarget[pathIndex], alien.transform.position) < .5);

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
    public Vector3 alienPosition, targetPosition;
    public PathGraph graph;

    public NativeArray<Vector3> pathToTarget;
    public NativeArray<int> lengthOfPath;
    public void Execute()
    {
        var newPathToTarget = new List<Vector3>();

        new AlienPathFinding
        {
            yLevel = graph.YLevel,
            targetPosition = targetPosition,
            graph = graph.ToDictionary()
        }.FindPath(newPathToTarget, alienPosition, maxPathLength);

        for (int i = 0; i < newPathToTarget.Count; ++i)
            pathToTarget[i] = newPathToTarget[i];
        lengthOfPath[0] = newPathToTarget.Count;
    }
}

class AlienPathFinding : AStar<Vector3>
{
    public float yLevel;
    public Vector3 targetPosition;
    public Dictionary<Vector3, HashSet<Vector3>> graph;

    protected override void Neighbors(Vector3 p, List<Vector3> neighbors) => neighbors.AddRange(graph[new(p.x, yLevel, p.z)]);
    protected override float Cost(Vector3 p1, Vector3 p2) => Mathf.Pow(p1.x - p2.x, 2) + Mathf.Pow(p1.z - p2.z, 2);
    protected override float Heuristic(Vector3 p) => Vector3.Distance(p, targetPosition);
}
