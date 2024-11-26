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

        pathToTarget.ForEach(point => Debug.DrawRay(point.pos, Vector3.up * 100, Color.blue));
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

    public void CalculatePathPeriodically(Vector3 target)
    {
        timeSinceLastThought += Time.deltaTime;

        if (ShouldRecalculatePath())
        {
            if (hasStartedThinking)
                CopyPathToTarget();

            thoughtHandle = StartCalculatingPath(target).Schedule();

            timeSinceLastThought = 0;
            hasStartedThinking = true;
        }
    }

    public void CalculatePathNow(Vector3 target)
    {
        thoughtHandle.Complete();
        StartCalculatingPath(target).Run();
        CopyPathToTarget();
        timeSinceLastThought = 0;
        hasStartedThinking = true;
    }

    AlienThought StartCalculatingPath(Vector3 target) => new()
    {
        maxPathLength = maxPathLength,
        alienPosition = alien.transform.position,
        targetPosition = target,
        pathToTarget = memoryBuffer,
        graph = alien.pathGraph.WithPositions(alien.transform.position, target),
        lengthOfPath = memoryBufferLengthUsed
    };

    bool ShouldRecalculatePath() => (
        pathIndex < 0 ||
        timeSinceLastThought > alien.mentalDelay
    );

    void CopyPathToTarget()
    {
        thoughtHandle.Complete();
        int pathLength = memoryBufferLengthUsed[0];
        List<PathNode> newPathToTarget = new(pathLength);
        for (int i = 0; i < pathLength; ++i)
            newPathToTarget.Add(memoryBuffer[i]);

        var hasReachedFirstPoint = pathIndex < pathToTarget.Count - 1;
        if (hasReachedFirstPoint && pathIndex >= 0)
        {
            var prevPoint = pathToTarget[pathIndex + 1];
            var nextPoint = pathToTarget[pathIndex];

            for (int i = 0; i < newPathToTarget.Count - 1; i++)
            {
                var newNextPoint = newPathToTarget[^(i + 1)];
                var newNextNextPoint = newPathToTarget[^(i + 2)];

                if (prevPoint == newNextPoint && nextPoint == newNextNextPoint)
                {
                    pathLength -= i + 1;
                    Debug.Log("skipping previously visited point");
                    break;
                }
            }
        }

        pathIndex = pathLength - 1;
        pathToTarget = newPathToTarget;
    }

    public void WillRecalculate() => pathIndex = 0;

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
