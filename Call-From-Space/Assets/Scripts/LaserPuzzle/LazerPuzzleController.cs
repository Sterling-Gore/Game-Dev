using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerPuzzleController : MonoBehaviour
{
    public List<GameObject> reflectors_and_endpoint; //9 slots
    public bool isCompleted;

    void Start()
    {
        isCompleted = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(!isCompleted) 
        {
            if(CheckCompletion())
            {
                isCompleted = true;
                //completion logic
                Debug.Log("Bruddha");
            }
        }
    }

    bool CheckCompletion()
    {
        return( 
            reflectors_and_endpoint[0].GetComponent<LaserScript>().on && 
            reflectors_and_endpoint[1].GetComponent<LaserScript>().on  && 
            reflectors_and_endpoint[2].GetComponent<LaserScript>().on  && 
            reflectors_and_endpoint[3].GetComponent<LaserScript>().on  && 
            reflectors_and_endpoint[4].GetComponent<LaserScript>().on  && 
            reflectors_and_endpoint[5].GetComponent<LaserScript>().on  && 
            reflectors_and_endpoint[6].GetComponent<LaserScript>().on  &&
            reflectors_and_endpoint[7].GetComponent<LaserScript>().on && 
            reflectors_and_endpoint[8].GetComponent<LaserScript>().on 
            );
    }
}
