using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
  string name;
  public float timeToRoamAroundFor;
  public Transform center;
  public List<PathNode> roamNodes = new();
  HashSet<string> neightborNames = new();

  public void Awake()
  {
    name = transform.name[4..];
    if (center == null)
      center = transform.Find("Center");
    if (center == null)
      center = transform.GetChild(0);
    foreach (Transform node in transform)
    {
      if (node.name.StartsWith("to"))
        neightborNames.Add(node.name[2..]);
      else
        roamNodes.Add(new(node));
    }
  }

  public List<Room> NeighboringRooms(List<Room> rooms) =>
      rooms.Where(room => neightborNames.Contains(room.name)).ToList();
}