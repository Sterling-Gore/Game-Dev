using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoxesPuzzle : MonoBehaviour
{
    public GameObject[] buttonsOff;
    
    public GameObject[] buttonsOn;
    public GameObject BoxesPuzzleUI;

    public Interactor interactor;

    int[] answerArray = new int[] 
{
    0, 1, 1, 1, 
    1, 0, 1, 1, 
    1, 1, 0, 1, 
    1, 1, 1, 0
};

    int[] currentAnswerArray = new int[]
    {
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0,
        0, 0, 0, 0
    };

    bool won = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Escape)) {
        BoxesPuzzleUI.SetActive(false);
        interactor.inUI = false;
      }

    }

    void Awake() {
       

    }

    void OnEnable() {
        // makeNewLevel();
int[] answerArray = new int[] 
{
    0, 1, 1, 1, 
    1, 0, 1, 1, 
    1, 1, 0, 1, 
    1, 1, 1, 0
};
int[] currentAnswerArray = new int[]
{
    0, 0, 0, 0,
    0, 0, 0, 0,
    0, 0, 0, 0,
    0, 0, 0, 0
    
};
Debug.Log("hey" + currentAnswerArray[0]);


TurnInteractableButtons(true);


        // StartCoroutine(ColorOrder());
    }

    // void makeNewLevel(){
    //     buttonsClicked = 0;
    //     Debug.Log("LEVEL! " + level);
    //     lightArray = new int[level * 3];
    //     for(int i = 0; i < lightArray.Length; i++){
    //         lightArray[i] = (Random.Range(0,4));
    //   }
    // }

    // IEnumerator ColorOrder(){
    //     yield return new WaitForSeconds(1F);

    //     for(int i = 0; i < lightArray.Length; i++){
    //         buttonsOn[lightArray[i]].SetActive(true);
    //         yield return new WaitForSeconds(0.5F);
    //         buttonsOn[lightArray[i]].SetActive(false);
    //         yield return new WaitForSeconds(0.25F);
    //     }

    //     enableButtons(true);
    //     TurnInteractableButtons(true);
    // }

    void TurnInteractableButtons(bool enable){
        for(int i = 0; i < buttonsOn.Length; i++){
            Debug.Log("enable " + i);
            buttonsOn[i].GetComponent<Button>().interactable = enable;
        }
    }

    // public void turnOnButtons(bool enable){
    //     for(int i = 0; i < buttonsOn.Length; i++){
    //     buttonsOn[i].SetActive(enable);
    //     }
    // }

    public void ButtonClick(int button){
        Debug.Log(button);
        currentAnswerArray[button] = 1;
        buttonsOn[button].GetComponent<UnityEngine.UI.Image>().enabled = true;
    }
    public void ButtonClickOff(int button){
        Debug.Log(button);
        currentAnswerArray[button] = 0;
        buttonsOn[button].GetComponent<UnityEngine.UI.Image>().enabled = false;
        
    }

    public void SubmitButton()
{
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
        }
    }
    
    // If the flag remains true, all elements match
    if (isMatch)
    {
        Debug.Log("All elements match.");
    }
}
   
}
