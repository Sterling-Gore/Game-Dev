using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMash : Interactable
{
    public float timeLimit = 5f; // Time in seconds for the player to respond
    //private KeyCode randomKey;
    private bool waitingForKey = false; 


    bool PuzzleCompleted = false;
    public GameObject PuzzleUI;
    public GameObject player;
    public bool breakTheRoutine = false;

    public GameObject ButtonMashImage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            {
                breakTheRoutine = true;
            }
    }

    public override string GetDescription()
    {
        if(!PuzzleCompleted)
            return ("Override Electrical Pannel");
        else
            return ("");
    }

    public override void Interact()
    {
        if(!PuzzleCompleted)
        {
            breakTheRoutine = false;
            PuzzleUI.SetActive(true);
            ButtonMashImage.SetActive(false);
            player.GetComponent<Interactor>().inUI = true;
            player.GetComponent<PlayerController>().Set_UI_Value(1);
            StartCoroutine(StartButtonMash());
        }
    }

    IEnumerator StartButtonMash()
    {
        bool flag = true;
        int count = 0;
        while (flag)
        {
            // Generate a random key
            //randomKey = GetRandomKey();
            //Debug.Log("Press the key: " + randomKey);
            if(breakTheRoutine)
                yield break;
            yield return new WaitForSeconds(1f); // Short delay before next round

            waitingForKey = true;
            float timer = 0f;
            ButtonMashImage.SetActive(true);
            ButtonMashImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-450,450), Random.Range(-200,150));

            // Wait for the correct key or timeout
            while (timer < timeLimit)
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Debug.Log("Success! You pressed the correct key: ");
                    count += 1;
                    waitingForKey = false;
                    break;
                }

                if(breakTheRoutine)
                    yield break;

                timer += Time.deltaTime;
                yield return null;
            }

            if (waitingForKey)
            {
                Debug.Log("Time's up! You failed to press the correct key.");
                count = 0;
                waitingForKey = false;
            }

            ButtonMashImage.SetActive(false);
            if (count >= 4)
            {
                flag = false;
                PuzzleCompleted = true;
                player.GetComponent<PlayerController>().ESCAPE();
            }
        }
    }
}
