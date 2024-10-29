using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoxesPuzzle : MonoBehaviour
{
    public GameObject[] buttonsOff;
    
    public GameObject[] buttonsOn;
    public GameObject BoxesPuzzleUI;

    int[] answerArray; 

    int[] currentAnswerArray;

    bool won = false;

    public GameObject gen;

    public Sprite greenButton;

    public Sprite redButton;
    public Sprite NOTHING;

     public AudioClip enableSound;
    public AudioClip disableSound;

    public AudioClip selectBox;
    public AudioClip unSelectBox;

    public AudioClip valid;

    public AudioClip inValid;

    
    public AudioSource audioSource;
    void Start()
    {
        answerArray = new int[] 
        {
            0, 1, 1, 1, 
            1, 0, 1, 1, 
            1, 1, 0, 1, 
            1, 1, 1, 0
        };
        if (currentAnswerArray == null || currentAnswerArray.Length == 0)
        {
            currentAnswerArray = new int[answerArray.Length];
        }
    }

    // Update is called once per frame

    void Awake() {

    }

    void OnEnable() {
        if (enableSound != null)
        {
            audioSource.PlayOneShot(enableSound);
        }
        if(!won)
        {
            TurnInteractableButtons(true);
            

        }
    }

    void OnDisable() {
        // Play the disable sound if assigned
        Debug.Log("OnDisable called");
        if (disableSound != null)
        {
            Debug.Log("PLAY DISABLE SOUND");
            audioSource.PlayOneShot(disableSound);
        }
    }


    void TurnInteractableButtons(bool enable){
        for(int i = 0; i < buttonsOn.Length; i++){
            Debug.Log("enable " + i);
            buttonsOn[i].GetComponent<Button>().interactable = enable;
            buttonsOff[i].GetComponent<Button>().interactable = enable;
            buttonsOff[i].transform.GetComponent<UnityEngine.UI.Image>().sprite = NOTHING;
        }
    }

    public void ButtonClick(int button){
        Debug.Log(button);
        currentAnswerArray[button] = 1;
        buttonsOn[button].GetComponent<UnityEngine.UI.Image>().enabled = true;
        audioSource.PlayOneShot(selectBox);
    }
    public void ButtonClickOff(int button){
        Debug.Log(button);
        currentAnswerArray[button] = 0;
        buttonsOn[button].GetComponent<UnityEngine.UI.Image>().enabled = false;
        audioSource.PlayOneShot(unSelectBox);
        
    }

    IEnumerator RedOrder(){
        TurnInteractableButtons(false);
        for(int i = 0; i < buttonsOn.Length; i++){
            buttonsOn[i].GetComponent<UnityEngine.UI.Image>().enabled = false;
        }

        yield return new WaitForSeconds(0.25F);
        for(int j = 0; j < 3; j++){
            audioSource.PlayOneShot(inValid);

        for(int i = 0; i < buttonsOff.Length; i++){
            buttonsOff[i].transform.GetComponent<UnityEngine.UI.Image>().sprite = redButton;
        }
        yield return new WaitForSeconds(0.5F);
        for(int i = 0; i < buttonsOff.Length; i++){
        buttonsOff[i].transform.GetComponent<UnityEngine.UI.Image>().sprite = NOTHING;
        }
         yield return new WaitForSeconds(0.25F);
        }

        for(int i = 0; i < buttonsOn.Length; i++){
            if(currentAnswerArray[i] == 1){
            buttonsOn[i].GetComponent<UnityEngine.UI.Image>().enabled = true;
            }
        }
       TurnInteractableButtons(true);
    }


    IEnumerator GreenOrder(){
        for(int i = 0; i < buttonsOn.Length; i++){
            buttonsOn[i].GetComponent<UnityEngine.UI.Image>().enabled = false;
        }

        yield return new WaitForSeconds(0.25F);
        
        for(int j = 0; j < 5; j++){
            audioSource.PlayOneShot(valid);

        for(int i = 0; i < buttonsOff.Length; i++){
            buttonsOff[i].transform.GetComponent<UnityEngine.UI.Image>().sprite = greenButton;
        }
        yield return new WaitForSeconds(0.5F);
        for(int i = 0; i < buttonsOff.Length; i++){
        buttonsOff[i].transform.GetComponent<UnityEngine.UI.Image>().sprite = NOTHING;
        }
         yield return new WaitForSeconds(0.25F);
        }

        for(int i = 0; i < buttonsOn.Length; i++){
            buttonsOn[i].GetComponent<UnityEngine.UI.Image>().enabled = true;
        }
       
    }

    public void SubmitButton()
{
    if(!won){
    Debug.Log("Current Answer Array:");
    for (int i = 0; i < currentAnswerArray.Length; i++)
    {
        Debug.Log($"Index {i}: {currentAnswerArray[i]}");
    }
    bool isMatch = true;  // Flag to track if all elements match
    
    // Iterate over the arrays and compare each element
    for (int i = 0; i < answerArray.Length; i++)
    {
        if (currentAnswerArray[i] != answerArray[i])
        {
            Debug.Log($"Mismatch at index {i}: expected {answerArray[i]}, but got {currentAnswerArray[i]}.");
            isMatch = false;  // Set the flag to false if any element doesn't match
            StartCoroutine(RedOrder());
        }
    }
    
    // If the flag remains true, all elements match
    if (isMatch)
    {
        Debug.Log("All elements match.");
        won = true;
        gen.transform.Find("genDoor").GetComponent<Animator>().SetTrigger("Open");
        gen.transform.Find("Fuel-Deposit").GetComponent<Collider>().enabled = true;
        TurnInteractableButtons(false);
        StartCoroutine(GreenOrder());
    }
    }
}
   
}