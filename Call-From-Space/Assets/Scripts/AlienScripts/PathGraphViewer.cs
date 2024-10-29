using System.Collections.Generic;
using UnityEngine;
public class PathGraphViewer : MonoBehaviour
{
  List<Transform> nodes = new();
  public bool draw = true;

  void OnDrawGizmos()
  {
    if (!draw)
    {
      if (nodes.Count != 0)
        nodes.Clear();
      return;
    }
    // nodes.Clear();
    if (nodes.Count == 0)
    {
      foreach (Transform section in transform)
        foreach (Transform room in section)
          foreach (Transform node in room)
            nodes.Add(node);
    }
    Gizmos.color = new Color(1, 0, 0, .5f);
    nodes.ForEach(node =>
    {
      var pos = node.position;
      pos.y = 2.658517f;
      Gizmos.DrawSphere(pos, node.localScale.x);
    });

  }
}