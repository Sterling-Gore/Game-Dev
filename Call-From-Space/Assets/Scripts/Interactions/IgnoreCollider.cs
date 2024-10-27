using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollider : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject alien;

    void Start()
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), player.transform.Find("Player Model").GetComponent<Collider>(), true);
    }
}
