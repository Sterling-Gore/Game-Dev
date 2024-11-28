using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
  new string name;
  public float timeToRoamAroundFor;
  public Transform center;
  public List<PathNode> roamNodes = new();
  Dictionary<string, PathNode> neightbors = new();

  public void Awake()
  {
    name = transform.name[4..];
    if (center == null)
      center = transform.Find("Center") ?? transform.GetChild(0);
    foreach (Transform node in transform)
    {
      if (node.name.StartsWith("to"))
        neightbors.Add(node.name[2..], new(node));
      else
        roamNodes.Add(new(node));
    }
  }

  public List<Room> NeighboringRooms(IEnumerable<Room> rooms) =>
      rooms.Where(room =>
      {
        if (!neightbors.TryGetValue(room.name, out PathNode neighborToRoom) || !room.neightbors.TryGetValue(name, out PathNode roomToNeighbor))
          return false;

        return PathGraph.HasNothingInBetween(neighborToRoom.pos, roomToNeighbor.pos);
      }).ToList();
}