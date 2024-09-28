using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item_interaction : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    
    public TMPro.TextMeshProUGUI ItemText;
    public Item item;
    public GameObject inspectorObj;
    bool mouse_over = false;

    void Update()
    {
        if(mouse_over)
        {
            ItemText.text = item.itemName;
        }
        else
        {
            ItemText.text = "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }

    /*
    public void loadInspector()
    {
        inspectorObj.SetActive(true);
        item.obj.SetActive(true);
        item.obj.GetComponent<Rigidbody>().useGravity = false;
        item.obj.transform.position = new Vector3(1000,1000,1000);
        //gameObject.SetActive(true);
        
    }*/
    
    


}
