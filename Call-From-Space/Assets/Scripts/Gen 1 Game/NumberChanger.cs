using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;  
using UnityEngine.UI;            
public class GeneratorGame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler { 
    public Sprite hoverSprite;   
    public Image normalImage;  
    public Sprite normalSprite;
    public Sprite clickedSprite;
    public TextMeshProUGUI counterText;
    public GameObject parentObject;
    private int secretCode = 31574;

    void Start() {
        if (normalImage == null) {
            normalImage = GetComponent<Image>();  
        }
        if (string.IsNullOrEmpty(counterText.text)) {
            counterText.text = "0";
        }
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

    public Boolean SubmitSecretCode(){

        string secretCodeStr = secretCode.ToString();

        TextMeshProUGUI[] textMeshChildren = parentObject.GetComponentsInChildren<TextMeshProUGUI>();

        for (int i = 0; i < textMeshChildren.Length; i++)
        {
            string textMeshValue = textMeshChildren[i].text;
            char secretCodeDigit = secretCodeStr[i];
            if (textMeshValue != secretCodeDigit.ToString()) {
                return false;
            }
        }
        return true;
    }
}
