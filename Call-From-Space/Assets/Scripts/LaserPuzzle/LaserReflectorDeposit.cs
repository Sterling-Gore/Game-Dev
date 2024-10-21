using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReflectorDeposit : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject Reflector1;
    public GameObject Reflector2;
    public GameObject PlugedInReflector;
    public GameObject Player;
    public override string GetDescription()
    {
        if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector1.GetComponent<Item>()) || Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector2.GetComponent<Item>()))
        {
            return "Plug In Reflector";
        }
        else
        {
            return "Find The Reflector";
        }
    }

    public override void Interact()
    {
        if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector1.GetComponent<Item>()))
        {
            Player.GetComponent<PlayerController>().inventory.DeleteItem(Reflector1.GetComponent<Item>());
            Reflector1.SetActive(false);
            PlugedInReflector.SetActive(true);
            gameObject.SetActive(false);
        }
        else if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector2.GetComponent<Item>()))
        {
            Player.GetComponent<PlayerController>().inventory.DeleteItem(Reflector2.GetComponent<Item>());
            Reflector2.SetActive(false);
            PlugedInReflector.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
