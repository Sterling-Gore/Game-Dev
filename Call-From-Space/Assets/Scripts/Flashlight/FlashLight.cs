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

    public float flashlightTimer;
    bool isOn;

    void Start()
    { 
        light = GetComponent<Light>();
        light.enabled = false;
        flashlightTimer = 30f;
        isOn =  false;
    }

    
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            toggleLight();
        }

        if(isOn)
            flashlightTimer = Mathf.Clamp(flashlightTimer - Time.deltaTime, 0, 30);
        else
            flashlightTimer = Mathf.Clamp(flashlightTimer+ (3* Time.deltaTime), 0, 30);

        if(flashlightTimer == 0)
            toggleLight();

    } 

    void toggleLight()
    {
        if (playSound)
        {
            audioSource.clip = light.enabled ? flashlightSoundOff : flashlightSoundOn;
            audioSource.PlayOneShot(clip: audioSource.clip);
        }
        if(isOn)
            light.enabled = false;
        else
            light.enabled = true;
        isOn = !isOn;
    }
      
    

}