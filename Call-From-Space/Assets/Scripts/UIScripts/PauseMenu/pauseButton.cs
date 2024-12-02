using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Color glowColor = new Color32(8, 248, 255, 255);
    public float glowIntensity = 1.5f;
    private Color originalColor;
    //08F8FF


    void Start()
    {
        if (buttonText == null)
        {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }
        originalColor = buttonText.color;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = glowColor * glowIntensity;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = originalColor;
    }


    public void LeaveGame()
    {
        GameStateManager.instance.SaveGame(GameStateManager.saveFilePath);
        SceneManager.LoadSceneAsync("Start");
    }

    public void GoToCheckPoint()
    {
        GameStateManager.instance.LoadGame(GameStateManager.checkPointFilePath);
    }
}
