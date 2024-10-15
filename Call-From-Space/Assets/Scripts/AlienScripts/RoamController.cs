using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RoamController
{
  AlienController alien;
  Dictionary<Vector3, HashSet<Vector3>> pathNeighbors;
  Vector3 lastPathPoint;
  Vector3 nextPathPoint;
  float timeToLookAroundFor;
  float timeLookingAround = 0;
  bool isLooking = true;

  public RoamController(AlienController alien)
  {
    this.alien = alien;
    pathNeighbors = alien.pathGraph
        .WithPositions(alien.transform.position, alien.player.transform.position)
        .ToDictionary();
    timeToLookAroundFor = alien.timeToLookAroundFor;
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

    var possibleNextPoints = pathNeighbors[alienPos]
        .Where(point => point != lastPathPoint)
        .ToList();
    if (possibleNextPoints.Count == 0)
      nextPathPoint = lastPathPoint;
    else
    {
      var randomIndex = Mathf.FloorToInt(UnityEngine.Random.value * possibleNextPoints.Count);
      nextPathPoint = possibleNextPoints[randomIndex];
    }
    lastPathPoint = nextPathPoint;
  }

  void LookAround()
  {
    if (timeLookingAround > timeToLookAroundFor)
    {
      ChooseNextPoint();
      isLooking = false;
      timeLookingAround = 0;
    }
    else
    {
      alien.transform.Rotate(Vector3.up, 360 * Time.deltaTime / timeToLookAroundFor);
      timeLookingAround += Time.deltaTime;
    }
  }
}