using UnityEngine;
using UnityEngine.UI;

public class OxygenSystem : MonoBehaviour
{
    public float oxygenLevel = 100f;
    public Text oxygenLevelText;

    private void Update()
    {
        // Update the UI text with the current oxygen level
        if (oxygenLevelText != null)
        {
            oxygenLevelText.text = "Oxygen Level: " + Mathf.RoundToInt(oxygenLevel).ToString();
        }
    }

    public void RefillOxygen()
    {
        oxygenLevel = 100f;
        Debug.Log("Oxygen refilled to max level.");
    }

    public void DecreaseOxygen(float amount)
    {
        oxygenLevel -= amount;
    }

    private void IncreaseOxygen(float amount)
    {
        oxygenLevel += amount;
    }
}