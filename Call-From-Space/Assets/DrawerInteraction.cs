using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerInteraction : Interactable
{
    // Start is called before the first frame update
    Animator animation;
    public GameObject Player;
    public GameObject Key;
    bool unlocked = false;
    
    void Start()
    {
        animation = GetComponent<Animator>();
    }

    public override string GetDescription()
    {
        if(!unlocked)
        {
            if (Player.GetComponent<PlayerController>().inventory.IsItemInList(Key.GetComponent<Item>()) )
            {
                return "Unlock Drawer";
            }
            else
            {
                return "Needs a key";
            }
        }
        else{
            return "";
        }
        
    }

    public override void Interact()
    {
        unlocked = true;
        animation.SetTrigger("Open");
        Player.GetComponent<PlayerController>().inventory.DeleteItem(Key.GetComponent<Item>());
    }
}
