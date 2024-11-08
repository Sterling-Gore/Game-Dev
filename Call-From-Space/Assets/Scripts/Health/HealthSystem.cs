using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float healthLevel = 100f;
    public Text healthLevelText;
    public Image healthBar;
    public Image healthRadial;
    public float healSpeed = 5f; // Speed at which health regenerates

    private void Update()
    {
        // Update the UI text with the current health level
        if (healthLevelText != null)
        {
            healthLevelText.text = "Health Level: " + Mathf.RoundToInt(healthLevel).ToString();
        }
        healthBar.fillAmount = healthLevel / 100;
        healthRadial.fillAmount = healthLevel / 100;

        // Heal over time if health is below the maximum level
        if (healthLevel < 100f)
        {
            healthLevel += healSpeed * Time.deltaTime;
            healthLevel = Mathf.Clamp(healthLevel, 0, 100f); // Ensure health level stays within bounds
        }
    }
}
