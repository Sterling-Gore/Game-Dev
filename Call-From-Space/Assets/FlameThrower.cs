using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrower : Holdable
{

    public GameObject Fire;
    public AudioSource audioSource;
    public bool isTangled;




    override protected void Awake()
    {
        isTangled = true;
        base.Awake();
        Fire.SetActive(false);
    }

    public override string GetDescription()
    {
        if(isTangled)
            return ("Flamethrower is tangled in the vine");
        return ("Press [E] to grab the " + objName);
    }

    public override void Interact()
    {
        if(!isTangled)
            PickUpObject();
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
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Fire.SetActive(true);
            Ray ray = new Ray(transform.position, transform.forward);
            Debug.DrawRay(transform.position, transform.forward * 10f, Color.green);
            if (Physics.Raycast(ray, out RaycastHit hit, 100))
            {
                //Debug.Log("YERRRRRR");
                BurningStem burnstem = hit.collider.GetComponent<BurningStem>();
                FlameThrower test = hit.collider.GetComponent<FlameThrower>();
                if(burnstem != null)
                {
                    Debug.Log("FOUND");
                    burnstem.burnPlant();
                }
                if(test != null)
                    Debug.Log("ERROR");
            }
        }
        else
        {
            Fire.SetActive(false);
        }
    }
}
