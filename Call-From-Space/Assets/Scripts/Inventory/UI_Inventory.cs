using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private Transform itemSlotContainer;
    private Transform itemSlotTemplate;

    private List<RectTransform> ItemContainers;
    private List<RectTransform> JournalContainers;

    private void Awake()
    {
        itemSlotContainer = transform.Find("itemSlotContainer");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");

    }
    

    public void setInventory(Inventory inventory )
    {
        this.inventory = inventory;
        //RefreshInventoryItems();
        //RefreshInventoryJournals();
    }

    public void RefreshInventoryItems()
    {
        foreach( Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }
        int xpos = 0;
        int ypos = 0;
        float itemSlotCellSize = 150f;
        foreach (Item item in inventory.GetItemList())
        {
            
            
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            Vector2 additionalPosition = new Vector2(xpos*itemSlotCellSize, ypos*itemSlotCellSize);
            itemSlotRectTransform.anchoredPosition = additionalPosition;
            Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();
            image.sprite = item.itemImage;


            //Item_interaction itemInter = itemSlotRectTransform.GetComponent<Item_interaction>();
            //itemInter.item = item;

            xpos += 1;
            if(xpos > 3)
            {
                ypos -= 1;
                xpos = 0;
            }

        }
    }

    public void RefreshInventoryJournals()
    {
        foreach( Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }
        int xpos = 0;
        int ypos = 0;
        float itemSlotCellSize = 150f;

        foreach (Item item in inventory.GetJournalList())
        {
            

            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            itemSlotRectTransform.gameObject.SetActive(true);
            Vector2 additionalPosition = new Vector2(xpos*itemSlotCellSize, ypos*itemSlotCellSize);
            itemSlotRectTransform.anchoredPosition = additionalPosition;

            Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();
            image.sprite = item.itemImage;

            //Item_interaction itemInter = itemSlotRectTransform.GetComponent<Item_interaction>();
            //itemInter.item = item;
            //itemSlotRectTransform.GetComponent<Button>().OnPointerEnter = () => {
            //    Debug.Log("working");
            //};


            xpos += 1;
            if(xpos > 3)
            {
                ypos -= 1;
                xpos = 0;
            }

            
        }
    }

  
}
