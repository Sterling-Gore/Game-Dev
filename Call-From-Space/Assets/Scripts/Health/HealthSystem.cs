using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float healthLevel = 100f;
    public Text healthLevelText;
    public Image healthBar;
    public Image healthRadial;
    public float healSpeed = 5f; // Speed at which health regenerates
    private Coroutine healCoroutine;
    GameObject endingScreen;
    public RectTransform healthBarRectTransform; // Assign this in the Inspector

    private void OnEnable()
    {
        healCoroutine = StartCoroutine(HealOverTime());
        endingScreen = GameObject.Find("EndingScreen");
    }

    private void OnDisable()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
        }
    }

    private IEnumerator HealOverTime()
    {
        while (true)
        {
            if (healthLevel > 0 && healthLevel < 100f)
            {
                healthLevel += healSpeed * Time.deltaTime;
                healthLevel = Mathf.Clamp(healthLevel, 0, 100f); // Ensure health level stays within bounds
            }
            yield return null;
        }
    }

    private void Update()
    {
        // Update the UI text with the current health level
        if (healthLevelText != null && healthLevel > 0)
        {
            healthLevelText.text = "Health Level: " + Mathf.RoundToInt(healthLevel).ToString();
            healthBar.fillAmount = healthLevel / 100;
            healthRadial.fillAmount = healthLevel / 100;
        }

        // Adjust the width of the health bar
        if (healthBarRectTransform != null)
        {
            healthBarRectTransform.sizeDelta = new Vector2(healthLevel, healthBarRectTransform.sizeDelta.y);
        }

        // Check if health level is zero
        if (healthLevel <= 0)
        {
            var gameOver = endingScreen.transform.GetChild(0).gameObject;
            gameOver.SetActive(true);
        }
    }
}
