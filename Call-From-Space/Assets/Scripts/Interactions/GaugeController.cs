using UnityEngine;
using TMPro;

public class GaugeController : MonoBehaviour
{
    public int currentPressure = 0;
    public TextMeshPro pressureText;
    
    [SerializeField]
    private Gradient textColorGradient;

    private void Start()
    {
        if (textColorGradient == null)
        {
            SetupDefaultGradient();
        }
        UpdateDisplay();
    }

    public void AdjustPressure(int amount)
    {
        currentPressure += amount;
        currentPressure = Mathf.Clamp(currentPressure, 0, 10);
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (pressureText != null)
        {
            pressureText.text = currentPressure.ToString();
            
            // Calculate how far the current pressure is from 5 (0 to 5)
            float distanceFromTarget = Mathf.Abs(currentPressure - 5) / 5f;
            
            // Use this to evaluate the gradient (0 is green, 1 is red)
            pressureText.color = textColorGradient.Evaluate(distanceFromTarget);
        }
        else
        {
            Debug.LogError("Pressure Text is not assigned in the GaugeController!");
        }
    }

    private void SetupDefaultGradient()
    {
        textColorGradient = new Gradient();
        var colorKey = new GradientColorKey[3];
        var alphaKey = new GradientAlphaKey[2];

        // Green at 0 (middle pressure)
        colorKey[0].color = Color.green;
        colorKey[0].time = 0f;
        // Yellow at 0.5 (25% away from middle)
        colorKey[1].color = Color.yellow;
        colorKey[1].time = 0.5f;
        // Red at 1 (furthest from middle)
        colorKey[2].color = Color.red;
        colorKey[2].time = 1f;

        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 1.0f;
        alphaKey[1].time = 1.0f;

        textColorGradient.SetKeys(colorKey, alphaKey);
    }

    public bool IsAtTargetPressure()
    {
        return currentPressure == 5;
    }
}