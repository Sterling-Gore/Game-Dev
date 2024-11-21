using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : Interactable
{
    Item item;
    public GameObject player;
    PlayerController pc;
    public GameObject JournalPlayer;
    public GameObject ItemGlow;



    override protected void Awake()
    {
        base.Awake();
        item = GetComponent<Item>();
        pc = player.GetComponent<PlayerController>();

        //Physics.IgnoreCollision(transform.Find("Collider").GetComponent<Collider>(), player.transform.Find("Player Model").GetComponent<Collider>(), true);
    }

    void Update()
    {
        if (gameObject.activeSelf && ItemGlow.activeSelf)
            ItemGlow.transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
    }

    public override string GetDescription()
    {

        return (item.itemName);
    }

    public override void Interact()
    {
        //inventory.AddItem(item);

        ItemGlow.SetActive(false);
        switch (item.itemName)
        {
            case "Sticky Note":
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle1(2);
                break;
            case "Locker Key":
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(2);
                break;
            default:
                break;
        }

        if (!item.isItem)
        {
            JournalPlayer.GetComponent<PlayJournal>().PlayAudioOnPickUp(item);
        }

        gameObject.SetActive(false);
        if (item.isItem)
        {
            pc.inventory.AddItem(item);
        }
        else
        {
            pc.inventory.AddJournal(item);
        }

        //pc.uiInvetory.RefreshInventoryItems();

    }
}
