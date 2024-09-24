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
    }

    public override string GetDescription()
    {
        
        return (item.itemName);
    }

    public override void Interact()
    {
        //inventory.AddItem(item);
        gameObject.SetActive(false);
        pc.inventory.AddItem(item);
        pc.uiInvetory.RefreshInventoryItems();
        
    }
}
