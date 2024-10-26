using UnityEngine;
using UnityEngine.UI;

public class OxygenSystem : MonoBehaviour
{
    public float oxygenLevel = 100f;
    public Text oxygenLevelText;
    public Image oxygenBar;
    public Image oxygenRadial;
    public float refillSpeed = 10f;
    public ParticleSystem breathEffect; // Reference to the particle system

    private void Update()
    {
        // Update the UI text with the current oxygen level
        if (oxygenLevelText != null)
        {
            oxygenLevelText.text = "Oxygen Level: " + Mathf.RoundToInt(oxygenLevel).ToString();
        }
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;

        // Adjust the particle effect based on the oxygen level
        if (breathEffect != null)
        {
            var emission = breathEffect.emission;
            emission.rateOverTime = Mathf.Lerp(0, 50, 1 - (oxygenLevel / 100)); // Increase emission rate as oxygen level decreases
        }
    }

    public void DecreaseOxygen(float amount)
    {
        oxygenLevel = Mathf.Clamp(oxygenLevel - (amount * Time.deltaTime), 0, 100);
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;
    }

    public void IncreaseOxygen()
    {
        oxygenLevel = Mathf.Clamp(oxygenLevel + (refillSpeed * Time.deltaTime), 0, 100);
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;
    }
}