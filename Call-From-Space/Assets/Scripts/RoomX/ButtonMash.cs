using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMash : Interactable
{
    public float timeLimit = 5f; // Time in seconds for the player to respond
    //private KeyCode randomKey;
    private bool waitingForKey = false; 

    public GameObject ButtonMashImage;

    public override string GetDescription()
    {
        return ("Start Button Mash");
    }

    public override void Interact()
    {
        Debug.Log("START");
        StartCoroutine(StartButtonMash());
    }

    IEnumerator StartButtonMash()
    {
        while (true)
        {
            // Generate a random key
            //randomKey = GetRandomKey();
            //Debug.Log("Press the key: " + randomKey);

            waitingForKey = true;
            float timer = 0f;
            ButtonMashImage.SetActive(true);

            // Wait for the correct key or timeout
            while (timer < timeLimit)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Debug.Log("Success! You pressed the correct key: ");
                    waitingForKey = false;
                    //yield return new WaitForSeconds(1f); // Short delay before next round
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            if (waitingForKey)
            {
                Debug.Log("Time's up! You failed to press the correct key.");
                waitingForKey = false;
            }

            ButtonMashImage.SetActive(false);
            yield return new WaitForSeconds(1f); // Short delay before next round
        }
    }
}
