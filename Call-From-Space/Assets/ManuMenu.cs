using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartingMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public TextMeshProUGUI buttonText;
    public Color glowColor = Color.red;
    public float glowIntensity = 1.5f;
    private Color originalColor;

    void Start()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }
        originalColor = buttonText.color;
    }

    public void OnPointerEnter(PointerEventData eventData){
        buttonText.color = glowColor * glowIntensity;
    }

    public void OnPointerExit(PointerEventData eventData){
        buttonText.color = originalColor;
    }

    public void PlayGame() {
        SceneManager.LoadSceneAsync("Ship");
    }

    public void ExitGame() {
        Application.Quit();
    }
}
