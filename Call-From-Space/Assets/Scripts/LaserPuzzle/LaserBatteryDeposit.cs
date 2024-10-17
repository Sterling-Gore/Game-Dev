using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBatteryDeposit : Interactable
{
    public GameObject Battery;
    public GameObject Player;
    public override string GetDescription()
    {
        if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Battery.GetComponent<Item>()))
        {
            return "Plug in battery";
        }
        else
        {
            return "find the battery";
        }
    }

    public override void Interact()
    {
        if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Battery.GetComponent<Item>()))
        {
            Player.GetComponent<PlayerController>().inventory.DeleteItem(Battery.GetComponent<Item>());
            Battery.SetActive(true);
            Battery.GetComponent<Rigidbody>().useGravity = false;
            Battery.GetComponent<Rigidbody>().isKinematic = true;
            Battery.GetComponent<Collider>().enabled = false;
            Battery.transform.position = new Vector3(-60.575f, 3.954f, 52.001f);
            Battery.transform.rotation = Quaternion.Euler(0, 135, 90);
            GetComponent<Collider>().enabled = false;
            GetComponent<LaserScript>().on = true;
        }
    }
}
