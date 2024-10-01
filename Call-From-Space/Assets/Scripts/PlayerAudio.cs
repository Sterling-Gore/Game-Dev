using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource[]  MovementAudios;
    void Start()
    {
      MovementAudios = gameObject.GetComponents<AudioSource>();
      MovementAudios[3].enabled = true;  
    }

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
       {
        if (Input.GetKey("left shift"))
            {
                MovementAudios[0].enabled = false;
                MovementAudios[1].enabled = true;
                MovementAudios[2].enabled = false;
            }
            else if (Input.GetKey("left ctrl"))
            {
                MovementAudios[0].enabled = false;
                MovementAudios[1].enabled = false;
                MovementAudios[2].enabled = true;
            }
            else{
                MovementAudios[0].enabled = true;
                MovementAudios[1].enabled = false;
                MovementAudios[2].enabled = false;
            }
       } 
       else
        {
            MovementAudios[0].enabled = false;
            MovementAudios[1].enabled = false;
            MovementAudios[2].enabled = false;
        }
    }
}
