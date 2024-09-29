using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Linq;

public static class AlienBrain
{
    public struct AlienThought : IJob
    {
        public int maxPathLength, spaceDiscretization;
        public Vector3 alienPosition, playerPosition;
        public SpaceShipPoints pointsInSpaceShip;

        public NativeArray<Point> pathToPlayer;
        public NativeArray<int> lengthOfPath;
        public void Execute()
        {
            var currentLocation = new Point
            {
                x = Mathf.RoundToInt(alienPosition.x),
                y = Mathf.RoundToInt(alienPosition.z)
            };
            var newPathToPlayer = new List<Point>();

            new AlienPathFinding
            {
                playerPosition = playerPosition,
                spaceDiscretization = spaceDiscretization,
                pointsInSpaceShip = pointsInSpaceShip
            }.FindPath(newPathToPlayer, currentLocation, maxPathLength);

            for (int i = 0; i < newPathToPlayer.Count; ++i)
                pathToPlayer[i] = newPathToPlayer[i];
            lengthOfPath[0] = newPathToPlayer.Count;
            Debug.Log("length of path calculated");
            Debug.Log(lengthOfPath[0]);
        }
    }

    private class AlienPathFinding : AStar<Point>
    {
        public Vector3 playerPosition;
        public int spaceDiscretization;
        public SpaceShipPoints pointsInSpaceShip;
        protected override void Neighbors(Point p, List<Point> neighbors)
        {
            p.Neighbors(spaceDiscretization).ForEach(point =>
            {
                if (pointsInSpaceShip.Contains(point))
                    neighbors.Add(point);
            });
        }
        protected override float Cost(Point p1, Point p2) => Mathf.Pow(p1.x - p2.x, 2) + Mathf.Pow(p1.y - p2.y, 2);

        protected override float Heuristic(Point p)
        {
            var distanceToPlayer = Vector3.Distance(p.AsVector3(), playerPosition);
            return Mathf.Clamp(distanceToPlayer - spaceDiscretization, 0, Mathf.Infinity);
        }
    }

    public struct Point
    {
        public int x, y;
        public List<Point> Neighbors(int spaceDiscretization) =>
            new()
            {
                new Point { x = x + spaceDiscretization, y = y },
                new Point { x = x - spaceDiscretization, y = y },
                new Point { x = x, y = y + spaceDiscretization },
                new Point { x = x, y = y - spaceDiscretization },
                new Point { x = x + spaceDiscretization, y = y + spaceDiscretization },
                new Point { x = x - spaceDiscretization, y = y + spaceDiscretization },
                new Point { x = x - spaceDiscretization, y = y + spaceDiscretization },
                new Point { x = x - spaceDiscretization, y = y - spaceDiscretization }
            };
        public Vector3 AsVector3() => new(x, 0, y);
        public bool Equals(Point p) => p.x == x && p.y == y;
    }

    public struct SpaceShipPoints
    {
        public NativeArray<bool> pointsInSpaceShip;
        public Point topRight, bottomLeft;
        int width;
        public SpaceShipPoints(HashSet<Point> pointsInSpaceShip, Point topRight, Point bottomLeft)
        {
            int height = topRight.y - bottomLeft.y +1;
            width = topRight.x - bottomLeft.x +1;
            this.pointsInSpaceShip = new((height) * (width), Allocator.Persistent);
            foreach (var p in pointsInSpaceShip)
            {
                int x = p.x - bottomLeft.x;
                int y = p.y - bottomLeft.y;
                this.pointsInSpaceShip[x + (y * width)] = true;
            }
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
        }

        public bool Contains(Point p)
        {
            if (p.x <= bottomLeft.x || p.x >= topRight.x || p.y <= bottomLeft.y || p.y >= topRight.y)
                return false;
            int x = p.x - bottomLeft.x;
            int y = p.y - bottomLeft.y;
            bool outl= pointsInSpaceShip[x + (y * width)];
            return outl;
        }
        
        public void Dispose() => pointsInSpaceShip.Dispose();
    }

    public static SpaceShipPoints GetPointsInSpaceShip(int spaceDiscretization)
    {
        var pathFinder = new AlienPathFinding();
        Point topRight = new() { x = -100, y = -100 };
        Point bottomLeft = new() { x = 100, y = 100 };
        HashSet<Point> pointsInSpaceShip = new();
        Stack<Point> pointStack = new();
        pointStack.Push(new Point { x = 0, y = 0 });
        while (pointStack.Count != 0)
        {
            NeighborsInSpaceShip(pointStack.Pop(), spaceDiscretization).ForEach(point =>
            {
                if (!pointsInSpaceShip.Contains(point))
                {
                    pointsInSpaceShip.Add(point);
                    pointStack.Push(point);
                    if (point.x > topRight.x) topRight.x = point.x;
                    else if (point.x < bottomLeft.x) bottomLeft.x = point.x;
                    if (point.y > topRight.y) topRight.y = point.y;
                    else if (point.y < bottomLeft.y) bottomLeft.y = point.y;
                }
            });
        }
        
        return new SpaceShipPoints(pointsInSpaceShip, topRight, bottomLeft);
    }

    static List<Point> NeighborsInSpaceShip(Point p, int spaceDiscretization)
    {
        return p.Neighbors(spaceDiscretization).Where(point =>
            Physics.Raycast(new Vector3(point.x, 1000f, point.y), Vector3.down)
        ).ToList();
    }
}
