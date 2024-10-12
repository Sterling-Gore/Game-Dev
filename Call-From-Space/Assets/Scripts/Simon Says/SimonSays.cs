using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class SimonSays : MonoBehaviour
{
    public GameObject[] buttonsOff;
    
    public GameObject[] buttonsOn;
    public GameObject SimonSaysUI;

    public Interactor interactor;

    int level;

    int[] lightArray;

    bool won = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if (Input.GetKeyDown(KeyCode.Escape)) {
        SimonSaysUI.SetActive(false);
        interactor.inUI = false;
      }

        
    }

    void Awake() {
        Debug.Log("WE HAVE AWAKEN ONLY ONCE");
        level = 0;
        lightArray = new int[4];
         for(int i = 0; i < lightArray.Length; i++){
            lightArray[i] = (Random.Range(0,4));
      }
    }

    void OnEnable() {
        Debug.Log("print level " + level);
        
        foreach (int value in lightArray)
        {
            print(value);
        }

        StartCoroutine(ColorOrder());
    }

    IEnumerator ColorOrder(){
        yield return new WaitForSeconds(1F);

        for(int i = 0; i < lightArray.Length; i++){
            buttonsOn[lightArray[i]].SetActive(true);
            yield return new WaitForSeconds(0.75F);
            buttonsOn[lightArray[i]].SetActive(false);
            yield return new WaitForSeconds(0.5F);
        }
    }
}
