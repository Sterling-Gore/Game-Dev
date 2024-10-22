using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private List<Item> itemList;
    private List<Item> journalList;

    public Inventory() {
        itemList = new List<Item>();
        journalList = new List<Item>();

        //AddItem(new Item {itemTag = 0});
        //AddItem(new Item {itemTag = 1});
        
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
    }

    public void DeleteItem(Item item)
    {
        itemList.RemoveAll(x => x == item);
    }

    public bool IsItemInList(Item item)
    {
        return itemList.Contains(item);
    }

    public void AddJournal(Item journal)
    {
        journalList.Add(journal);
    }

    public List<Item> GetItemList() {
        return itemList;
    }

    public List<Item> GetJournalList() {
        return journalList;
    }
}
