using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;

    public Inventory() {
        itemList = new List<Item>();

        //AddItem(new Item {itemTag = 0});
        //AddItem(new Item {itemTag = 1});
        
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }

    public List<Item> GetItemList() {
        return itemList;
    }
}
