using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelDeposit : Interactable
{
    public GameObject FuelCell;
    public GameObject player;
    public PowerLevel powerLevel;
    public float soundRadius;
    public Vector3 rotation;
    public Vector3 position;


    void OnTriggerEnter(Collider other)
    {
        if (/*other.CompareTag("FuelCell")*/ other == FuelCell.GetComponent<Collider>() && !player.GetComponent<Interactor>().isHolding)
        {
            FuelCell.GetComponent<Rigidbody>().useGravity = false;
            FuelCell.GetComponent<Rigidbody>().isKinematic = true;
            FuelCell.GetComponent<Holdable>().enabled = false;
            FuelCell.transform.position = position;
            FuelCell.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            //lights.SetActive(true);
            //PowerLevel powerLevel = FindObjectOfType<PowerLevel>();
            if (powerLevel != null)
            {
                Debug.Log("Fuel cell deposited");
                powerLevel.GeneratorActivated();
                SoundSourcesController.GetInstance().CreateNewSoundSource(transform.position, soundRadius);
            }
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
