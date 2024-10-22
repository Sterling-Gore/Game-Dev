using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class OxygenSystem : MonoBehaviour
{
    public float oxygenLevel = 100f;
    public Text oxygenLevelText;
    public Image oxygenBar;
    public Image oxygenRadial;
    private bool isRefilling = false;
    public float refillSpeed = 10f;


    private void Update()
    {
        // Update the UI text with the current oxygen level
        if (oxygenLevelText != null)
        {
            oxygenLevelText.text = "Oxygen Level: " + Mathf.RoundToInt(oxygenLevel).ToString();
        }
        oxygenBar.fillAmount = oxygenLevel / 100;
        oxygenRadial.fillAmount = oxygenLevel / 100;
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