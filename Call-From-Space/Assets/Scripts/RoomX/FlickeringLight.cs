using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public GameObject light1;
    public GameObject light2;
    public GameObject light3;
    public GameObject light4;
    public GameObject lightCone;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("FLICKER TEST");
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while(true)
        {
            yield return new WaitForSeconds(3f);
            if (Random.value > 0.6f)
            {
                //Debug.Log("FLICKER!!!!!!!S");
                //turn off
                light1.SetActive(false);
                light2.SetActive(false);
                light3.SetActive(false);
                light4.SetActive(false);
                lightCone.SetActive(false);
                yield return new WaitForSeconds(.25f);
                //turn on
                light1.SetActive(true);
                light2.SetActive(true);
                light3.SetActive(true);
                light4.SetActive(true);
                lightCone.SetActive(true);
            }
        }
        
    }
}
