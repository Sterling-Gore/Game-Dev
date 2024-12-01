using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMash : Interactable
{
    public float timeLimit = 5f; // Time in seconds for the player to respond
    //private KeyCode randomKey;
    private bool waitingForKey = false; 


    bool PuzzleCompleted = false;
    public GameObject PuzzleUI;
    public GameObject player;
    public MeshRenderer VineMeshRenderer;
    public Collider VineCollider;
    bool breakTheRoutine = false;

    public Sprite[] keySprites;
    public string[] Keycodes;
    //Key order
    //0 : v
    //1 : k
    //2 : l
    //3 : g
    //4 : t
    //5 : u
    //6 : m

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
            return ("Press [E] to Override Electrical Pannel");
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

            int randnum = Random.Range(0,keySprites.Length);
            ButtonMashImage.SetActive(true);
            ButtonMashImage.transform.GetComponent<Image>().sprite = keySprites[randnum]; 
            ButtonMashImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-450,450), Random.Range(-200,150));

            // Wait for the correct key or timeout
            while (timer < timeLimit)
            {
                if (Input.GetKeyDown(Keycodes[randnum]))
                {
                    Debug.Log("Success! You pressed the correct key: ");
                    count += 1;
                    waitingForKey = false;
                    break;
                }
                else if(Input.anyKeyDown)
                {
                    Debug.Log("Fail! You pressed the wrong key: ");
                    waitingForKey = false;
                    count = 0;
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
            }

            ButtonMashImage.SetActive(false);
            if (count >= 4)
            {
                flag = false;
                PuzzleCompleted = true;
                StartCoroutine(FadeOut());
                player.GetComponent<PlayerController>().ESCAPE();
            }
        }
    }


    IEnumerator FadeOut()
    {
        //flame.Play();
        //audioSource.Play();
        while (VineMeshRenderer.materials[0].color.a > 0f)
        {
            Color currentColor = VineMeshRenderer.materials[0].color;
            currentColor.a -= .5f * Time.deltaTime;
            VineMeshRenderer.materials[0].color = currentColor;
            yield return null;
        }
        //flame.Stop();
        VineCollider.enabled = false;
        VineMeshRenderer.enabled = false;
    }
}
