using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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

    PlayerController playerController;
    string deposited;

    protected override void Awake()
    {
        base.Awake();
        playerController = Player.GetComponent<PlayerController>();
    }

    public override string GetDescription()
    {
        playerController.TaskList_UI_Object.GetComponent<TaskList>().LaserPuzzle(2);
        if (playerController.inventory.IsItemInList(Reflector1.GetComponent<Item>()) || playerController.inventory.IsItemInList(Reflector2.GetComponent<Item>()))
            return "Plug In Reflector";
        else
            return "Find The Reflector";
    }

    public override void Interact()
    {
        if (playerController.inventory.IsItemInList(Reflector1.GetComponent<Item>()))
            Deposit(Reflector1);
        else if (playerController.inventory.IsItemInList(Reflector2.GetComponent<Item>()))
            Deposit(Reflector2);

        if (!playerController.inventory.IsItemInList(Reflector1.GetComponent<Item>()) && !playerController.inventory.IsItemInList(Reflector2.GetComponent<Item>()) && !Reflector1.activeSelf && !Reflector2.activeSelf)
            playerController.TaskList_UI_Object.GetComponent<TaskList>().LaserPuzzle(3);
    }

    void Deposit(GameObject reflector)
    {
        Sparkle.SetActive(false);
        playerController.inventory.DeleteItem(reflector.GetComponent<Item>());
        reflector.SetActive(false);
        PlugedInReflector.SetActive(true);
        gameObject.SetActive(false);
        audioSource.PlayOneShot(InsertSound);
        deposited = reflector.name;
    }

    public override void Load(JObject state)
    {
        base.Load(state);
        if ((bool)state[fullName]["isDeposited"])
        {
            if ((string)state[fullName]["deposited"] == Reflector1.name)
                Deposit(Reflector1);
            else
                Deposit(Reflector2);
        }
    }

    public override void Save(ref JObject state)
    {
        base.Save(ref state);
        state[fullName]["isDeposited"] = !gameObject.activeSelf;
        state[fullName]["deposited"] = deposited;
    }
}
