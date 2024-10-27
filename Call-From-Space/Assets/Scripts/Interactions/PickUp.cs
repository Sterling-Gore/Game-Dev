using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : Interactable
{
    Item item;
    public GameObject player;
    PlayerController pc;
    
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
