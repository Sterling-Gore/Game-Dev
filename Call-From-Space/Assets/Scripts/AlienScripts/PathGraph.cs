using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;
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

    static readonly int layerMask = ~(
        Physics.IgnoreRaycastLayer |
        (1 << LayerMask.NameToLayer("AlienLayer"))
    );

    public PathGraph(List<Transform> pathNodes)
    {
        int numChildren = pathNodes.Count;

        pathPoints = new(numChildren + 1, Allocator.Persistent);
        List<Neighbors> pairs = new();

        int idx = 0;
        foreach (Transform pathNode in pathNodes)
            pathPoints[idx++] = new(pathNode);

        for (int i = 0; i < numChildren; i++)
        {
            var point1 = pathPoints[i];
            for (int j = i - 1; j >= 0; j--)
            {
                var point2 = pathPoints[j];
                if (HasNothingInBetween(point1.pos, point2.pos))
                {
                    pairs.Add(new() { a = point1, b = point2 });
                    Debug.DrawRay(point1.pos, point2.pos - point1.pos, Color.red, 100);
                }
            }
        }

        neighbors = new(pairs.Count + numChildren, Allocator.Persistent);
        for (int i = 0; i < pairs.Count; i++)
            neighbors[i] = pairs[i];
    }

    public PathGraph WithPosition(Vector3 alienPosition)
    {
        alienPosition.y = YLevel;
        PathNode alienPositionNode = new() { pos = alienPosition, radius = 0 };
        int idx = neighbors.Length - pathPoints.Length - 1;
        for (int i = 0; i < pathPoints.Length - 1; i++)
            if (HasNothingInBetween(pathPoints[i].pos, alienPosition))
                neighbors[idx++] = new() { a = pathPoints[i], b = alienPositionNode };
        while (idx < neighbors.Length)
            neighbors[idx++] = new();

        pathPoints[^1] = alienPositionNode;
        return this;
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
                neighborPoints[pair.a].Add(pair.b);
                neighborPoints[pair.b].Add(pair.a);
            }
        }
        return neighborPoints;
    }

    public void Dispose()
    {
        pathPoints.Dispose();
        neighbors.Dispose();
    }

    static bool HasNothingInBetween(Vector3 a, Vector3 b)
    {
        var dist = Vector3.Distance(a, b);

        return !Physics.Raycast(a, (b - a) / dist, dist, layerMask);
    }
}

public struct PathNode
{
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