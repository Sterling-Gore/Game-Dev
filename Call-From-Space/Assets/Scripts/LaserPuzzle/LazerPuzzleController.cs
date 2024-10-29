using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerPuzzleController : MonoBehaviour
{
    public GameObject endpoint; 
    public List<GameObject> reflectors; //list of the 6 reflectors we can move (not including the 2 you cant move)
    public bool isCompleted;
    float timer;


    public Material LightOn; // for when the light turns on
    public Material FuelCellOn; //for when the fuelcell is charged
    public GameObject FuelCell;
    public GameObject NeedsRechargingCollider;
    public Color lightOnColor;

    AudioSource completionSound;

    void Start()
    {
        isCompleted = false;
        timer = 0;
        completionSound = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!isCompleted) 
        {
            if(CheckCompletion())
            {
                isCompleted = true;
                //all the visuals
                FuelCell.GetComponent<Renderer>().material = FuelCellOn;
                transform.Find("Light").GetComponent<Renderer>().material = LightOn;
                transform.Find("Light").Find("Point Light").GetComponent<Light>().color = lightOnColor;
                FuelCell.transform.Find("Point Light").gameObject.SetActive(true);
                NeedsRechargingCollider.SetActive(false);
                completionSound.Play();

                //making sure the player cant turn the lasers anymore
                foreach(GameObject reflector in reflectors)
                    reflector.GetComponent<ReflectorRotater>().PuzzleIsCompleted = true;



                //completion logic
            }
        }
    }

    bool CheckCompletion()
    {
        if(endpoint.GetComponent<LaserScript>().on)
        {
            if(timer > 2f)
            {
                return true;
            }
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
        }
        return false;
    }
}
