using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomDetector : MonoBehaviour
{

  public GameObject playerRoomLoc;
  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerRoomLoc.SetActive(true);
      Debug.Log("something1");
    }
  }

  void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      playerRoomLoc.SetActive(false);
      Debug.Log("something3");
    }
  }
}
