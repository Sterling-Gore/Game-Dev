using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inspector : MonoBehaviour, IDragHandler
{
    Item item;
    public GameObject player;
    float xRot = 0f;
    float yRot = 0f;
    


    public void loadInspector(GameObject temp)
    {
        item = temp.GetComponent<Item_interaction>().item;

        if(item.isItem)
        {
            player.GetComponent<PlayerController>().Set_UI_Value(2);
            
                Debug.Log("CHANGE SOUND HERE FOR INSPECTOR");
                AudioManager.Instance.PlaySound("Menu-Open");
            switch(item.itemName)
            {
                case "Sticky Note":

                    
                    player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle1(3);
                    break;
                default:
                    break;
            }
            gameObject.SetActive(true);
            item.obj.SetActive(true);
            item.obj.GetComponent<Rigidbody>().useGravity = false;
            item.obj.GetComponent<Rigidbody>().isKinematic = true;
            item.obj.transform.position = new Vector3(1000,1000,1001);
            item.obj.transform.rotation = Quaternion.Euler(0, 180, 0);
            //gameObject.SetActive(true);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        xRot -= eventData.delta.y;
        yRot -=eventData.delta.x;
        //item.obj.transform.eulerAngles += new Vector3(-eventData.delta.y, -eventData.delta.x);
        //item.obj.transform.rotation += new Vector3(-eventData.delta.y, -eventData.delta.x);
        item.obj.transform.rotation = Quaternion.Euler(xRot, yRot, 0);
    }

    public void unloadInspector()
    {
        item.obj.GetComponent<Rigidbody>().useGravity = true;
        item.obj.GetComponent<Rigidbody>().isKinematic = false;
        item.obj.SetActive(false);
        gameObject.SetActive(false);
    }

    
}
