using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SimonSays : MonoBehaviour
{
    public GameObject[] buttonsOff;
    
    public GameObject[] buttonsOn;
    public GameObject[] HeaderButtons; 
    public Sprite greenButton;
    public GameObject SimonSaysUI;


    int level;
    int[] ButtonsPerLevel = { 2, 3, 4 };

    int[] lightArray;
    int buttonsClicked = 0;

    public GameObject gen;

    bool won = false;
    bool passed = true;

    public AudioClip enableSound;

    public AudioClip pressSound;
    public AudioClip upSound;
    public AudioClip downSound;
    public AudioClip leftSound;
    public AudioClip rightSound;

    public AudioClip valid;

    public AudioClip inValid;
    
    public GameObject player;

    public AudioSource audioSource;

    void Start()
    {
        
    }

    void Awake() {
        level = 0;
    }

    void OnEnable() {
        enableButtons(false);
        if(!won)
        {
            if (enableSound != null)
            {
                audioSource.PlayOneShot(enableSound);
            }
            makeNewLevel();
            for(int i = 0; i < lightArray.Length; i++)
            {
                Debug.Log("i: " + i + " " + lightArray[i]);
            }

            StartCoroutine(ColorOrder());
        }
    }

    void makeNewLevel(){
        buttonsClicked = 0;
        Debug.Log("LEVEL! " + level);
        lightArray = new int[ButtonsPerLevel[level]];
        for(int i = 0; i < lightArray.Length; i++){
            lightArray[i] = (Random.Range(0,4));
      }
    }

    IEnumerator ColorOrder(){
        yield return new WaitForSeconds(1F);

        for(int i = 0; i < lightArray.Length; i++){
            if (lightArray[i] == 0)
    {
        audioSource.PlayOneShot(upSound);
    }
    else if (lightArray[i] == 1)
    {
        audioSource.PlayOneShot(rightSound);
    }
    else if (lightArray[i] == 2)
    {
        audioSource.PlayOneShot(downSound);
    }
    else if (lightArray[i] == 3)
    {
        audioSource.PlayOneShot(leftSound);
    }
            buttonsOn[lightArray[i]].SetActive(true);
            yield return new WaitForSeconds(0.5F);
            buttonsOn[lightArray[i]].SetActive(false);
            yield return new WaitForSeconds(0.25F);
        }

        enableButtons(true);
        TurnInteractableButtons(true);
    }

    void TurnInteractableButtons(bool enable){
        for(int i = 0; i < buttonsOn.Length; i++){
            Debug.Log("enable " + i);
            buttonsOn[i].GetComponent<Button>().interactable = enable;
        }
    }

    public void enableButtons(bool enable){
        for(int i = 0; i < buttonsOn.Length; i++){
        buttonsOn[i].SetActive(enable);
        }
    }

    public void ButtonClickOrder(int button)
    {
        audioSource.PlayOneShot(pressSound);
        buttonsClicked++;
        if(button == lightArray[buttonsClicked-1]){
            Debug.Log("RIGHT COLOR");
            passed = true;
        } else {
            audioSource.PlayOneShot(inValid);
            Debug.Log("FAILED");
            won = false;
            passed = false;
            makeNewLevel();
            enableButtons(false);
            StartCoroutine(ColorOrder());
            // Logic here if they failed
        }

        if(buttonsClicked == ButtonsPerLevel[level] && passed == true){
            //HeaderButtons[level].SetActive(true); 
            HeaderButtons[level].transform.GetComponent<Image>().sprite = greenButton; 
            level += 1;
            audioSource.PlayOneShot(valid);
            if(level == 3)
            {
                won = true;
                enableButtons(false);
                audioSource.PlayOneShot(valid);
                gen.transform.Find("genDoor").GetComponent<Animator>().SetTrigger("Open");
                gen.transform.Find("Fuel-Deposit").GetComponent<Collider>().enabled = true;
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(6);
            }
            else
            {
                makeNewLevel();
                passed = false;
                enableButtons(false);
                StartCoroutine(ColorOrder());
            }
        }
    }
}
