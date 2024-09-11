using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    public GameObject player;
    public float standard_speed;
    public float awareness_radius;
    float speed;
    bool isAwareOfPlayer = false;
    
    void Start()
    {
        speed = standard_speed;
    }

    void Update()
    {
        if (!isAwareOfPlayer)
        {
            if(Vector3.Distance(player.transform.position,transform.position)<awareness_radius)
                isAwareOfPlayer = true;
            return;
        }

        MoveTowardsPlayer();
    }

    //TODO: Implement A*
    void MoveTowardsPlayer()
    {
        var directionToPlayer = player.transform.position - transform.position;

        Vector3 movement = speed * Time.deltaTime * new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        transform.position += Vector3.ClampMagnitude(movement, 1f);
    }
}
