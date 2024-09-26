using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public GaugeController[] gauges;
    public GameObject airlock;

    private void Update()
    {
        if (CheckPuzzleCompletion())
        {
            OpenAirlock();
        }
    }

    private bool CheckPuzzleCompletion()
    {
        foreach (var gauge in gauges)
        {
            if (!gauge.IsAtTargetPressure())
            {
                return false;
            }
        }
        return true;
    }

    private void OpenAirlock()
    {
        // Implement airlock opening logic here
        airlock.SetActive(false); // Simple example: just deactivate the airlock object
    }
}