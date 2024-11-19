using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMash : Interactable
{
    public GameObject player;
    public AudioSource audioSource;
    public AudioClip start;
    public AudioClip correct;
    public AudioClip wrong;
    
    // Start is called before the first frame update
    public override string GetDescription()
    {
        
        return ("Puzzle");
    }

    public override void Interact()
    {
        
        player.GetComponent<Interactor>().inUI = true;
        player.GetComponent<PlayerController>().Set_UI_Value(1);
        StartCoroutine(buttonMash());
        float timer = 2f;
        
    }


    IEnumerator buttonMash(){
        yield return new WaitForSeconds(1F);

        for(int i = 0; i < 5; i++){
            float timer = 2f;
            bool answered = false;
            audioSource.PlayOneShot(start);
            yield return new WaitForSeconds(1F);
            while( timer > 0 || answered)
            {
                Debug.Log(timer);
                if (Input.GetKey(KeyCode.V))
                {
                    audioSource.PlayOneShot(correct);
                    answered = true;
                }
                timer -= Time.deltaTime;
            }
            yield return new WaitForSeconds(1F);
            if(!answered)
            {
                audioSource.PlayOneShot(wrong);
            }
            
        }

    }
}
