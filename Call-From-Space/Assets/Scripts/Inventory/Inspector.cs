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
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            unloadInspector();
            player.GetComponent<PlayerController>().toggleInventory();
        }
    }

    public void loadInspector(GameObject temp)
    {
        gameObject.SetActive(true);
        item = temp.GetComponent<Item_interaction>().item;
        item.obj.SetActive(true);
        item.obj.GetComponent<Rigidbody>().useGravity = false;
        item.obj.transform.position = new Vector3(1000,1000,1003);
        //gameObject.SetActive(true);

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
        item.obj.SetActive(false);
        gameObject.SetActive(false);
    }

    
}
