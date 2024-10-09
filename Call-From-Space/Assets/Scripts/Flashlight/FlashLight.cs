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
            audioSource.clip = light.enabled ? flashlightSoundOff : flashlightSoundOn;
            audioSource.PlayOneShot(clip: audioSource.clip);
            light.enabled = !light.enabled;
        }
    } 
      
    

}