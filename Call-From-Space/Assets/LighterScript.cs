using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighterScript : MonoBehaviour
{
    // Start is called before the first frame update
    bool isOpen;
    public Animator animation;
    public GameObject Fire;
    int debug;
    void Start()
    {
        isOpen = false;
        Fire.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.Mouse0) ) 
        {
            Debug.Log(debug);
            animation.SetTrigger(isOpen ? "Closed" : "Open");
            isOpen = !isOpen;
            Fire.SetActive(isOpen);
        }
    }
}
