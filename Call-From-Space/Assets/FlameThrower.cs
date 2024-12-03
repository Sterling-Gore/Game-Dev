using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : Holdable
{

    public GameObject Fire;
    public AudioSource audioSource;




    override protected void Awake()
    {
        base.Awake();
        Fire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf && ItemGlow.activeSelf)
            ItemGlow.transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        if (localHold)
        {
            MoveObject();
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StopClipping();
                DropObject();
            }
        }
    }
}
