using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;
using Unity.VisualScripting;

public static class AlienBrain
{
    public struct AlienThought : IJob
    {
        public int maxPathLength, spaceDiscretization;
        public Vector3 alienPosition, playerPosition;
        public PathGraph graph;

        public NativeArray<Vector3> pathToPlayer;
        public NativeArray<int> lengthOfPath;
        public void Execute()
        {
            var newPathToPlayer = new List<Vector3>();

            new AlienPathFinding
            {
                yLevel = graph.pathPoints[0].y,
                playerPosition = playerPosition,
                spaceDiscretization = spaceDiscretization,
                graph = graph.ToDictionary()
            }.FindPath(newPathToPlayer, alienPosition, maxPathLength);

            for (int i = 0; i < newPathToPlayer.Count; ++i)
                pathToPlayer[i] = newPathToPlayer[i];
            lengthOfPath[0] = newPathToPlayer.Count;
        }
    }

    private class AlienPathFinding : AStar<Vector3>
    {
        public float yLevel;
        public Vector3 playerPosition;
        public int spaceDiscretization;
        public Dictionary<Vector3, HashSet<Vector3>> graph;
        protected override void Neighbors(Vector3 p, List<Vector3> neighbors) => neighbors.AddRange(graph[new(p.x, yLevel, p.z)]);

        protected override float Cost(Vector3 p1, Vector3 p2) => Mathf.Pow(p1.x - p2.x, 2) + Mathf.Pow(p1.z - p2.z, 2);

        protected override float Heuristic(Vector3 p) => Mathf.Clamp(Vector3.Distance(p, playerPosition) - spaceDiscretization, 0, Mathf.Infinity);
    }

    public struct PathGraph
    {
        public struct Neighbors
        {
            public Vector3 a, b;
        }
        public NativeArray<Vector3> pathPoints;

        public NativeArray<Neighbors> neighbors;

        public PathGraph(Transform pathNodes)
        {
            int numChildren = pathNodes.childCount;
            pathPoints = new(numChildren + 1, Allocator.Persistent);
            List<Neighbors> pairs = new();

            for (int i = 0; i < numChildren; i++)
            {
                var point1 = pathNodes.GetChild(i).position;
                pathPoints[i] = point1;
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
            playerPosition.y = alienPosition.y = pathPoints[0].y;
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
            Dictionary<Vector3, HashSet<Vector3>> neighborPoints = new(numChildren);
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
}
