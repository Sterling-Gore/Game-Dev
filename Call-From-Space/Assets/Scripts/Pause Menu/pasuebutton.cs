using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour {/*, IPointerEnterHandler, IPointerExitHandler {
    public TextMeshProUGUI buttonText;
    public Color glowColor = new Color32(8, 248, 255, 255);
    public float glowIntensity = 1.5f;
    private Color originalColor;
    public GameObject player;

    PlayerController playercontroller;
    Interactor interactor;

    void Start() {
        if (buttonText == null) {
            buttonText = GetComponentInChildren<TextMeshProUGUI>();
        }
        originalColor = buttonText.color;

        playercontroller = player.GetComponent<PlayerController>();
        interactor = player.GetComponent<Interactor>();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        buttonText.color = glowColor * glowIntensity;
    }

    public void OnPointerExit(PointerEventData eventData) {
        buttonText.color = originalColor;
    }
    public void Resume() {
        if (player != null) {
            interactor.inUI = false;
            playercontroller.isPaused = false;
            playercontroller.pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void LeaveGame() {
        SceneManager.LoadSceneAsync("Start");
    }
    */
}
