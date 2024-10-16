using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OxygenSystem : MonoBehaviour
{
    public float oxygenLevel = 100f;
    public Text oxygenLevelText;
    private bool isRefilling = false;


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
        if (!isRefilling)
        {
            StartCoroutine(RefillOxygenOverTime());
        }
    }

    private IEnumerator RefillOxygenOverTime()
    {
        isRefilling = true;
        while (oxygenLevel < 100f)
        {
            oxygenLevel += Time.deltaTime; // Increase oxygen level over time
            Debug.Log("Oxygen level: " + oxygenLevel);
            yield return null; // Wait for the next frame
        }
        Debug.Log("Oxygen refilled to max level.");
        isRefilling = false;
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