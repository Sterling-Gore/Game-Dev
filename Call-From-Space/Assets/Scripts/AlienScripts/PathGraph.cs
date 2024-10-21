using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;
public struct PathGraph
{
    public struct Neighbors
    {
        public Vector3 a, b;
    }
    public NativeArray<Vector3> pathPoints;

    public NativeArray<Neighbors> neighbors;

    public float YLevel => pathPoints[0].y;

    public PathGraph(List<Transform> pathNodeGroups)
    {
        int numChildren = pathNodeGroups.Sum(group => group.childCount);
        pathPoints = new(numChildren + 1, Allocator.Persistent);
        List<Neighbors> pairs = new();

        int idx = 0;
        foreach (var pathNodes in pathNodeGroups)
            foreach (Transform pathNode in pathNodes)
                pathPoints[idx++] = pathNode.position;

        for (int i = 0; i < numChildren; i++)
        {
            var point1 = pathPoints[i];
            for (int j = i - 1; j >= 0; j--)
            {
                var point2 = pathPoints[j];
                if (HasNothingInBetween(point1, point2))
                {
                    pairs.Add(new() { a = point1, b = point2 });
                    Debug.DrawRay(point1, point2 - point1, Color.red, 100);
                }
            }
        }

        neighbors = new(pairs.Count + 1, Allocator.Persistent);
        for (int i = 0; i < pairs.Count; i++)
            neighbors[i] = pairs[i];
    }

    public PathGraph WithPositions(Vector3 alienPosition, Vector3 playerPosition)
    {
        playerPosition.y = alienPosition.y = YLevel;
        var minDistance = float.MaxValue;
        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            if (HasNothingInBetween(pathPoints[i], alienPosition))
            {
                var dist = Vector3.Distance(playerPosition, pathPoints[i]);
                if (dist < minDistance)
                {
                    neighbors[^1] = new() { a = pathPoints[i], b = alienPosition };
                    minDistance = dist;
                }
            }
        }
        Debug.DrawRay(alienPosition, neighbors[^1].a - alienPosition, Color.green, 100);
        pathPoints[^1] = alienPosition;
        return this;
    }
    public Dictionary<Vector3, HashSet<Vector3>> ToDictionary()
    {
        int numChildren = pathPoints.Length;
        Dictionary<Vector3, HashSet<Vector3>> neighborPoints = new(numChildren, new VectorComparer());
        for (int i = 0; i < numChildren; i++)
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
        return !Physics.Raycast(a, (b - a) / dist, dist, Physics.DefaultRaycastLayers);
    }
}

class VectorComparer : IEqualityComparer<Vector3>
{
    public bool Equals(Vector3 vec1, Vector3 vec2) => vec1 == vec2;
    public int GetHashCode(Vector3 vec) => Mathf.RoundToInt(vec.x + vec.z);
}