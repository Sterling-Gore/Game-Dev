using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelDeposit : Interactable
{
    public GameObject FuelCell;
    public GameObject player;
    public GameObject lights;
    public GameObject alien;
    public PowerLevel powerLevel;
    public float soundRadius;

    void OnTriggerEnter(Collider other)
    {
        if (/*other.CompareTag("FuelCell")*/ other == FuelCell.GetComponent<Collider>() && !player.GetComponent<Interactor>().isHolding)
        {
            FuelCell.GetComponent<Rigidbody>().useGravity = false;
            FuelCell.GetComponent<Rigidbody>().isKinematic = true;
            FuelCell.GetComponent<Holdable>().enabled = false;
            FuelCell.transform.position = new Vector3(-55.318f, 3.705f, -3.61f);
            FuelCell.transform.rotation = Quaternion.Euler(0, 0, 90);
            //lights.SetActive(true);
            //PowerLevel powerLevel = FindObjectOfType<PowerLevel>();
            if (powerLevel != null)
            {
                Debug.Log("Fuel cell deposited");
                powerLevel.GeneratorActivated();
                SoundSourcesController.GetInstance().CreateNewSoundSource(transform.position, soundRadius);
            }
            //alien.SetActive(true);
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
