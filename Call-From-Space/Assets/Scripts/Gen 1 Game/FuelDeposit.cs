using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelDeposit : Interactable
{
    public GameObject FuelCell;
    public GameObject player;
    

    void OnTriggerEnter(Collider other)
    {
        if (/*other.CompareTag("FuelCell")*/ other == FuelCell.GetComponent<Collider>() && !player.GetComponent<Interactor>().isHolding){  
           FuelCell.GetComponent<Rigidbody>().useGravity = false;
           FuelCell.GetComponent<Rigidbody>().isKinematic = true;
           FuelCell.GetComponent<Holdable>().enabled = false;
           FuelCell.transform.position = new Vector3(-55.318f, 3.705f, -3.61f);
           FuelCell.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    public override string GetDescription()
    {
        return "Deposit Fuel Cell";
    }

    public override void Interact()
    {

    }
}
