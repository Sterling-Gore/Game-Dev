using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RoamController
{
    AlienController alien;
    Dictionary<Vector3, HashSet<Vector3>> pathNeighbors;
    Vector3 nextPathPoint;
    public float timeLookingAround = 0;
    bool isLooking = true;

    List<Vector3> recentPoints;
    HashSet<Vector3> preferedPoints;

    public RoamController(AlienController alien)
    {
        this.alien = alien;
        pathNeighbors = alien.pathGraph
            .WithPositions(alien.transform.position, alien.transform.position)
            .ToDictionary();
        recentPoints = new(alien.roamingPathPointMemory);
        preferedPoints = pathNeighbors.Select(point => point.Key).ToHashSet();
        Debug.Log(alien.transform.position);
        Debug.Log(pathNeighbors.Count);
    }

    public void RoamAround()
    {
        if (isLooking)
            LookAround();
        else
            Walk();
    }

    void Walk()
    {
        if (alien.MoveTowards(nextPathPoint))
        {
            var alienPos = alien.transform.position;
            alien.transform.position = new(nextPathPoint.x, alienPos.y, nextPathPoint.z);
            isLooking = true;
        }
    }
    void ChooseNextPoint()
    {
        var alienPos = alien.transform.position;
        alienPos.y = alien.pathGraph.YLevel;

        var possibleNextPoints = pathNeighbors[alienPos];
        var preferedNextPoints = possibleNextPoints
          .Where(p => preferedPoints.Contains(p))
          .ToList();
        if (possibleNextPoints.Count > 0)
        {
            var randomIndex = Mathf.FloorToInt(Random.value * (possibleNextPoints.Count - 1));
            nextPathPoint = preferedNextPoints[randomIndex];
        }
        else
            nextPathPoint = recentPoints.First(point => possibleNextPoints.Contains(point));
        VisitPoint(nextPathPoint);

        Debug.Log("done looking, next point:");
        Debug.Log(nextPathPoint);
    }

    void VisitPoint(Vector3 point)
    {
        if (preferedPoints.Remove(point))
        {
            if (recentPoints.Count >= alien.roamingPathPointMemory)
            {
                preferedPoints.Add(recentPoints.First());
                recentPoints.RemoveAt(0);
            }
        }
        else
            recentPoints.Remove(point);
        recentPoints.Add(point);
    }

    void LookAround()
    {
        if (timeLookingAround > alien.timeToLookAroundFor)
        {
            ChooseNextPoint();
            isLooking = false;
            timeLookingAround = 0;
        }
        else
        {
            int angle = 200;
            if (timeLookingAround < alien.timeToLookAroundFor / 4 || timeLookingAround > alien.timeToLookAroundFor * 3 / 4)
                angle = -200;
            alien.head.Rotate(Vector3.forward, angle * Time.deltaTime / alien.timeToLookAroundFor);
            timeLookingAround += Time.deltaTime;
        }
    }
}