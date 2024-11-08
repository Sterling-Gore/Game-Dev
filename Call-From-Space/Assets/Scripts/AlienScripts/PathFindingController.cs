using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class PathFindingController
{
    AlienController alien;
    public List<PathNode> pathToTarget = new();
    public int pathIndex = -1;

    const int maxPathLength = 30000;
    float timeSinceLastThought;
    bool hasStartedThinking = false;

    JobHandle thoughtHandle;

    NativeArray<PathNode> memoryBuffer;
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
        var directionToPath = nextPathPoint.pos - alien.transform.position;
        directionToPath.y = 0;
        var distanceToPath = directionToPath.magnitude;

        Debug.DrawRay(alien.transform.position, directionToPath, Color.cyan, 10);
        Debug.DrawRay(nextPathPoint.pos, -alien.turnRadius * directionToPath.normalized, Color.magenta);

        if (distanceToPath < alien.turnRadius)
            pathIndex--;
        alien.MoveTowards(nextPathPoint.pos);
    }

    public void CalculatePathPeriodically()
    {
        timeSinceLastThought += Time.deltaTime;

        if (ShouldRecalculatePath())
        {
            if (hasStartedThinking)
                CopyPathToTarget();

            thoughtHandle = StartCalculatingPath(alien.curTarget.pos).Schedule();

            timeSinceLastThought = 0;
            hasStartedThinking = true;
        }
    }

    public void CalculatePathNow(Vector3 target)
    {
        StartCalculatingPath(target).Run();
        CopyPathToTarget();
        timeSinceLastThought = 0;
        hasStartedThinking = true;
    }

    AlienThought StartCalculatingPath(Vector3 target)
    {
        return new()
        {
            maxPathLength = maxPathLength,
            alienPosition = alien.transform.position,
            targetPosition = target,
            pathToTarget = memoryBuffer,
            graph = alien.pathGraph.WithPositions(alien.transform.position, target),
            lengthOfPath = memoryBufferLengthUsed
        };
    }

    bool ShouldRecalculatePath() => (
        pathIndex < 0 ||
        timeSinceLastThought > alien.mentalDelay
    );

    void CopyPathToTarget()
    {
        pathToTarget.ForEach(point => Debug.DrawRay(point.pos, Vector3.up * 100, Color.blue, alien.mentalDelay));

        thoughtHandle.Complete();

        pathToTarget.Clear();
        pathToTarget.Capacity = memoryBufferLengthUsed[0];
        for (int i = 0; i < memoryBufferLengthUsed[0]; ++i)
            pathToTarget.Add(memoryBuffer[i]);

        pathIndex = pathToTarget.Count - 1;
    }

    public void Recalculate() => pathIndex = 0;

    public bool HasArrived() => pathIndex < 0 || pathToTarget.Count == 0 || (
        pathIndex == 0 &&
        Vector3.Distance(pathToTarget[pathIndex].pos, alien.transform.position) < .5
    );

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

    public NativeArray<PathNode> pathToTarget;
    public NativeArray<int> lengthOfPath;
    public void Execute()
    {
        var newPathToTarget = new List<PathNode>();
        PathNode alienPathNode = new() { pos = alienPosition, radius = 0 };
        new AlienPathFinding
        {
            yLevel = graph.YLevel,
            targetPosition = targetPosition,
            graph = graph.ToDictionary()
        }.FindPath(newPathToTarget, alienPathNode, maxPathLength);

        for (int i = 0; i < newPathToTarget.Count; ++i)
            pathToTarget[i] = newPathToTarget[i];
        lengthOfPath[0] = newPathToTarget.Count;
    }
}

class AlienPathFinding : AStar<PathNode>
{
    public float yLevel;
    public Vector3 targetPosition;
    public Dictionary<PathNode, HashSet<PathNode>> graph;

    protected override void Neighbors(PathNode p, List<PathNode> neighbors) =>
        neighbors.AddRange(graph[new()
        {
            pos = new(p.pos.x, yLevel, p.pos.z),
            radius = p.radius
        }]);
    protected override float Cost(PathNode p1, PathNode p2) =>
        Mathf.Pow(p1.pos.x - p2.pos.x, 2) + Mathf.Pow(p1.pos.z - p2.pos.z, 2);
    protected override float Heuristic(PathNode p) =>
        Vector3.Distance(p.pos, targetPosition);
}
