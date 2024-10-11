using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    Light light;
    public bool playSound;
    public AudioSource audioSource;
    public AudioClip flashlightSoundOn;
    public AudioClip flashlightSoundOff;

    void Start()
    { 
        light = GetComponent<Light>();
        light.enabled = false;
    }

    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (playSound)
            {
                audioSource.clip = light.enabled ? flashlightSoundOff : flashlightSoundOn;
                audioSource.PlayOneShot(clip: audioSource.clip);
            }
            light.enabled = !light.enabled;
        }
    } 
      
    

}