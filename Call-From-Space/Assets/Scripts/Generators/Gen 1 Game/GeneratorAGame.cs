using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gen_1_Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;  
using UnityEngine.UI;            
public class GeneratorAGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler { 
    public Sprite hoverSprite;   
    public Image normalImage;  
    public Sprite normalSprite;
    public Sprite clickedSprite;
    public TextMeshProUGUI counterText;
    public GameObject parentObject;
    public Image defaultMessage;
    public Sprite defaultMessageSprite;
    public Sprite incorrectMessage;
    public Sprite correctMessage; 
    private int secretCode = 31574;
    public GameObject generatorUI;
    public GameObject Player_for_interactor;
    public Interactor interactor;
    public GameObject gen;
    public GeneratorAudio generatorAudio;


    void Awake() {
      interactor = Player_for_interactor.GetComponent<Interactor>();
      if (normalImage == null) {
          normalImage = GetComponent<Image>();  
      }
      if (string.IsNullOrEmpty(counterText.text)) {
          counterText.text = "0";
      }
      interactor.inUI = true;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (normalImage != null && hoverSprite != null) {
            normalImage.sprite = hoverSprite;  
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (normalImage != null && normalSprite != null) {
            normalImage.sprite = normalSprite;  
        }
    }

    public void ButtonUpPressed(){
        if (normalImage != null && normalSprite != null) {
            normalImage.sprite = clickedSprite;  
        }
    }
    public void IncreaseNumber() {
        int counter = GetCurrentNumber();
        if (counter < 9) {
            counter++;
            updateCounter(counter);
        } else if (counter == 9) {
            counter = 0;
            updateCounter(counter);
        }
    }

    public void DecreaseNumber() {
        int counter = GetCurrentNumber();
        if (counter > 0) {
            counter--;
            updateCounter(counter);
        } else if (counter == 0) {
            counter = 9;
            updateCounter(counter);
        }
    }

    private int GetCurrentNumber() {
        if (int.TryParse(counterText.text, out int currentNumber))
        {
            return currentNumber;
        }
        return 0;
    }

    private void updateCounter(int newNumber) {
        counterText.text = newNumber.ToString();
    }

    public void SubmitSecretCode(){

        string secretCodeStr = secretCode.ToString();

        TextMeshProUGUI[] textMeshChildren = parentObject.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < textMeshChildren.Length; i++)
        {
            string textMeshValue = textMeshChildren[i].text.Trim();
            char secretCodeDigit = secretCodeStr[i];
            if (textMeshValue != secretCodeDigit.ToString()) {
                Debug.Log("called");
                updateImage(false);
                return;
            };
        }
        updateImage(true);
        gen.transform.Find("genDoor").GetComponent<Animator>().SetTrigger("Open");
        gen.transform.Find("Fuel-Deposit").GetComponent<Collider>().enabled = true;

        //generatorUI.SetActive(false);
        //interactor.inUI = false;

        if (generatorAudio != null)
        {
            generatorAudio.TurnOnGenerator();
        }
    }

    public void updateImage(bool correctCode){
      if (defaultMessage != null) {
        if (correctCode) {
            defaultMessage.sprite = correctMessage;
        } else {
            defaultMessage.sprite = incorrectMessage;
        }
      }
    }


}
