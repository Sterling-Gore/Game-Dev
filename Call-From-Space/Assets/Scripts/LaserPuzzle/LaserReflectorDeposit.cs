using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReflectorDeposit : Interactable
{

    public GameObject Reflector1;
    public GameObject Reflector2;
    public GameObject PlugedInReflector;
    public GameObject Player;
    public GameObject Sparkle;

    public AudioClip InsertSound;

    public AudioSource audioSource;



    public override string GetDescription()
    {
        Player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().LaserPuzzle(2);
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
            Sparkle.SetActive(false);
            Player.GetComponent<PlayerController>().inventory.DeleteItem(Reflector1.GetComponent<Item>());
            Reflector1.SetActive(false);
            PlugedInReflector.SetActive(true);
            gameObject.SetActive(false);
            audioSource.PlayOneShot(InsertSound);
        }
        else if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector2.GetComponent<Item>()))
        {
            Sparkle.SetActive(false);
            Player.GetComponent<PlayerController>().inventory.DeleteItem(Reflector2.GetComponent<Item>());
            Reflector2.SetActive(false);
            PlugedInReflector.SetActive(true);
            gameObject.SetActive(false);
            audioSource.PlayOneShot(InsertSound);
        }

        if(!Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector1.GetComponent<Item>()) && !Player.GetComponent<PlayerController>().inventory.IsItemInList(Reflector2.GetComponent<Item>()) && !Reflector1.activeSelf && !Reflector2.activeSelf)
            Player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().LaserPuzzle(3);
    }
}
