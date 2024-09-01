using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Movement()
    {
        float moveHorizontal = Input.GetAxis ("Horizontal");
        float moveVertical = Input.GetAxis ("Vertical");
        Vector3 Movement = new Vector3 (moveHorizontal, 0, moveVertical);
    
        transform.position += Movement * speed * Time.deltaTime;
    }
    
    
    void FixedUpdate()
    {
        Movement();
    }
}
