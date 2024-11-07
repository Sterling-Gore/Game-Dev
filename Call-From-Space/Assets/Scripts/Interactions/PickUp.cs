using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : Interactable
{
    Item item;
    public GameObject player;
    PlayerController pc;
    public GameObject JournalPlayer;


    
    void Awake()
    {
        item = GetComponent<Item>();
        pc = player.GetComponent<PlayerController>();

        //Physics.IgnoreCollision(transform.Find("Collider").GetComponent<Collider>(), player.transform.Find("Player Model").GetComponent<Collider>(), true);
    }

    public override string GetDescription()
    {
        
        return (item.itemName);
    }

    public override void Interact()
    {
        //inventory.AddItem(item);
     

        switch (item.itemName)
        {
            case "Sticky Note":
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle1(2);
                break;
            default:
                break;
        }

        if(!item.isItem)
        {
            JournalPlayer.GetComponent<PlayJournal>().PlayAudioOnPickUp(item);
        }

        gameObject.SetActive(false);
        if(item.isItem)
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
