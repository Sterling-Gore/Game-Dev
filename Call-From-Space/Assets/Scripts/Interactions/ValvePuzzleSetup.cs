using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ValvePuzzleSetup : MonoBehaviour
{
    public GaugeController[] gauges;
    public ValveController[] valves;

    private const int target_pressure = 5;
    private const int max_pressure = 10;
    private const int min_valve_effect = -3;
    private const int max_valve_effect = 3;


    void Start()
    {
        SetupPuzzle();
    }

    void Update()
    {
        //int test = ValveRandomizer();
        //Debug.Log(test);
    }

    void SetupPuzzle()
    {
        bool[] valveUsage = ValveRandomizer();

        int[] initialGaugeValues = new int[2];
        //1st gaugeValue is below 5
        initialGaugeValues[0] = Random.Range(0,5);
        //2st gaugeValue is any number
        initialGaugeValues[0] = Random.Range(0,max_pressure + 1);

        


        /*
        // generate initial gauge values
        int[] initialGaugeValues = new int[gauges.Length];
        for (int i = 0; i < gauges.Length; i++)
        {
            initialGaugeValues[i] = Random.Range(0, max_pressure + 1);
        }

        bool[] valveUsage = new bool[valves.Length];
        for (int i = 0; i < valves.Length; i++)
        {
            valveUsage[i] = (Random.value > 0.5f);
        }

        // generate valve effects and adjust them to reach the target pressure
        int[][] valveEffects = new int[valves.Length][];
        for (int i = 0; i < valves.Length; i++)
        {
            valveEffects[i] = new int[gauges.Length];
        }

        for (int i = 0; i < gauges.Length; i++)
        {
            int delta = target_pressure - initialGaugeValues[i];
            List<int> valvesUsed = new List<int>();
            for (int j = 0; j < valves.Length; j++)
            {
                if (valveUsage[j])
                {
                    valvesUsed.Add(j);
                }
            }
            // Calculate the minimum and maximum total effect that can be achieved with the valves used
            int minTotalEffect = valvesUsed.Count * min_valve_effect;
            int maxTotalEffect = valvesUsed.Count * max_valve_effect;

            if (delta < minTotalEffect || delta > maxTotalEffect)
            {
                initialGaugeValues[i] = target_pressure - Mathf.Clamp(delta, minTotalEffect, maxTotalEffect);
                delta = target_pressure - initialGaugeValues[i];
            }

            int remainingDelta = delta;
            int valvesRemaining = valvesUsed.Count;
            foreach (int valveIndex in valvesUsed)
            {
                int maxEffect = Mathf.Min(max_valve_effect, remainingDelta - (valvesRemaining - 1) * min_valve_effect);
                int minEffect = Mathf.Max(min_valve_effect, remainingDelta - (valvesRemaining - 1) * max_valve_effect);
                int effect = Random.Range(minEffect, maxEffect + 1);

                valveEffects[valveIndex][i] = effect;
                remainingDelta -= effect;
                valvesRemaining--;
            }

            // Assign random effects to valves not used in the solution
            for (int j = 0; j < valves.Length; j++)
            {
                if (!valveUsage[j])
                {
                    valveEffects[j][i] = Random.Range(min_valve_effect, max_valve_effect + 1);
                }
            }
        }

        for (int i = 0; i < gauges.Length; i++)
        {
            gauges[i].currentPressure = initialGaugeValues[i];
            gauges[i].UpdateDisplay();
        }

        for (int i = 0; i < valves.Length; i++)
        {
            valves[i].gaugeEffects = valveEffects[i];
        }

        string solutionString = string.Join(", ", valveUsage.Select(b => b ? "ON" : "OFF"));
        Debug.Log($"solution: [{solutionString}]");
        */
    }

    bool[]  ValveRandomizer()
    {
        bool[] valveUsage = new bool[4]; //should be 4 valves
        int num_of_used_valves = 0;
        bool temp;

        //1st valve
        temp = (Random.value > 0.25f); //75%
        valveUsage[0] = temp;
        if (temp)
            num_of_used_valves += 1;

        
        //2nd valve
        if (num_of_used_valves == 1)
            temp = (Random.value > 0.5f); //50% if we have 1 used valve
        else
            temp = (Random.value > 0.25f); //75% if we have no used valves
        valveUsage[1] = temp;
        if (temp)
            num_of_used_valves += 1;
        
        //3rd valve
        if (num_of_used_valves == 1 || num_of_used_valves == 2)
            temp = (Random.value > 0.5f); //50% if we have 1 or 2 used valves
        else
            temp = true; //100% if we have no used valves
        valveUsage[2] = temp;
        if (temp)
            num_of_used_valves += 1;
        
        //4th value
        if (num_of_used_valves == 2)
            temp = (Random.value > 0.5f); //50% if we have 2 used valves
        else if (num_of_used_valves == 1)
            temp = true; //100% if we have 1 used valve
        else
            temp = false; // 0% if we have 3 used valves
        valveUsage[3] = temp;
        if (temp)
            num_of_used_valves += 1;
        
        return valveUsage;

    }

}