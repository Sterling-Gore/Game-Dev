using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    Light light;
    public AudioSource audioSource;
    public AudioClip flashlightSoundOn;
    public AudioClip flashlightSoundOff;

    void Start()
    {
        
        light = GetComponent<Light>();
    }

    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (light.enabled)
            {
                audioSource.clip = flashlightSoundOff;
            }
            else
            {
                audioSource.clip = flashlightSoundOn;
            }
            audioSource.Play();
            light.enabled = !light.enabled;
        }
    } 
      
    

}