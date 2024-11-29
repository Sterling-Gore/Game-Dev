using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System;
public struct PathGraph
{
    public struct Neighbors
    {
        public PathNode a, b;
    }
    public NativeArray<PathNode> pathPoints;

    public NativeArray<Neighbors> neighbors;

    public float YLevel => pathPoints[0].pos.y;

    public static readonly int layerMask = ~(
        Physics.IgnoreRaycastLayer |
        (1 << LayerMask.NameToLayer("AlienLayer"))
    );

    public PathGraph(List<Transform> pathNodes)
    {
        int numChildren = pathNodes.Count;

        pathPoints = new(numChildren + 2, Allocator.Persistent);
        List<Neighbors> pairs = new();

        for (int i = 0; i < numChildren; i++)
        {
            var point1 = pathPoints[i] = new(pathNodes[i]);
            for (int j = i - 1; j >= 0; j--)
            {
                var point2 = pathPoints[j];
                if (HasNothingInBetween(point1.pos, point2.pos))
                {
                    Debug.DrawLine(point1.pos, point2.pos, Color.red, 200);
                    pairs.Add(new() { a = point1, b = point2 });
                }
            }
        }

        neighbors = new(pairs.Count + numChildren + 2, Allocator.Persistent);
        for (int i = 0; i < pairs.Count; i++)
            neighbors[i] = pairs[i];
    }

    public PathGraph WithPositions(Vector3 alienPosition, Vector3 targetPosition)
    {
        int idx = neighbors.Length - pathPoints.Length;
        int totalAdded = 0;

        alienPosition.y = targetPosition.y = YLevel;
        pathPoints[^1] = addPosition(alienPosition, this);
        pathPoints[^2] = addPosition(targetPosition, this);

        if (idx < neighbors.Length && HasNothingInBetween(alienPosition, targetPosition))
            neighbors[idx++] = new() { a = pathPoints[^1], b = pathPoints[^2] };

        while (idx < neighbors.Length)
            neighbors[idx++] = new();

        if (totalAdded == 0)
            Debug.Log("no path to target!!!");

        return this;

        PathNode addPosition(Vector3 position, PathGraph graph)
        {
            PathNode positionNode = new() { pos = position, radius = 0 };
            int added = 0;
            for (int i = 0; i < graph.pathPoints.Length - 2; i++)
            {
                if (added >= graph.pathPoints.Length / 2 || idx > graph.neighbors.Length)
                    break;
                if (HasNothingInBetween(graph.pathPoints[i].pos, position))
                {
                    graph.neighbors[idx++] = new() { a = graph.pathPoints[i], b = positionNode };
                    added++;
                }
            }
            totalAdded += added;
            return positionNode;
        }
    }

    public Dictionary<PathNode, HashSet<PathNode>> ToDictionary()
    {
        Dictionary<PathNode, HashSet<PathNode>> neighborPoints = new(pathPoints.Length);
        for (int i = 0; i < pathPoints.Length; i++)
            neighborPoints[pathPoints[i]] = new();

        for (int i = 0; i < neighbors.Length; i++)
        {
            var pair = neighbors[i];
            if (pair.a != pair.b)
            {
                neighborPoints[pair.b].Add(pair.a);
                neighborPoints[pair.a].Add(pair.b);
            }
        }
        return neighborPoints;
    }

    public void Dispose()
    {
        pathPoints.Dispose();
        neighbors.Dispose();
    }

    public static bool HasNothingInBetween(Vector3 a, Vector3 b)
    {
        var dist = Vector3.Distance(a, b);

        return !Physics.Raycast(a, (b - a) / dist, dist, layerMask);
    }
}

public struct PathNode
{
    public static PathNode None => new();
    public Vector3 pos;
    public float radius;

    public PathNode(Transform node)
    {
        pos = node.position;
        radius = node.localScale.x;
    }
    public override bool Equals(object obj) =>
        obj is PathNode node2 && pos == node2.pos && radius == node2.radius;
    public override int GetHashCode() =>
        Mathf.RoundToInt(pos.x + pos.z + radius);

    public static bool operator ==(PathNode node1, PathNode node2) => node1.pos == node2.pos && node1.radius == node2.radius;
    public static bool operator !=(PathNode node1, PathNode node2) => !(node1 == node2);
}