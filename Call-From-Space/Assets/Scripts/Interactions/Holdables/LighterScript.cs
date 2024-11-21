using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterScript : Holdable
{
    // Start is called before the first frame update
    public bool isOpen;
    public Animator animation;
    public GameObject Fire;

    public Transform lightBase;


    override protected void Awake()
    {
        base.Awake();
        isOpen = false;
        Fire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf && ItemGlow.activeSelf)
            ItemGlow.transform.position = new Vector3(lightBase.position.x, lightBase.position.y + 0.25f, lightBase.position.z);
        if (localHold)
        {
            MoveObject();
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                StopClipping();
                DropObject();
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                animation.SetTrigger(isOpen ? "Closed" : "Open");
                isOpen = !isOpen;
                Fire.SetActive(isOpen);
            }
        }

    }



}
