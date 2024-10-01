using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class ValvePuzzleSetup : MonoBehaviour
{
    public GameObject[] gauges;
    public GameObject[] valves;
    public GameObject door;

    int[][] ValveValues = new int[4][];
    int[] GaugeValues = new int[2];
    bool[] valveUsage;
    

    private const int target_pressure = 5;
    private const int max_pressure = 10;
    private const int min_valve_effect = -3;
    private const int max_valve_effect = 3;

    TextMeshPro[] Text = new TextMeshPro[2];

    public Material GaugeMat1;
    public Material GaugeMat2;
    public Material GaugeMat3;
    public Material GaugeMat4;
    public Material GaugeMat5;


    void Start()
    {
        for(int i = 0; i < 4; i++)
        {
            ValveValues[i] = new int[2];
        }

        SetupPuzzle();

        for(int i = 0; i < 4; i++)
        {
            valves[i].GetComponent<ValveInteractable>().setGaugeVals(ValveValues[i][0], ValveValues[i][1]);
        }

        Text[0] = gauges[0].transform.Find("Text").GetComponent<TextMeshPro>();
        Text[1] = gauges[1].transform.Find("Text").GetComponent<TextMeshPro>();
        Text[0].text = GaugeValues[0].ToString();
        Text[1].text = GaugeValues[1].ToString();
        updateMaterial();
        
        
       

    }

    void Update()
    {
        
        if (CheckPuzzleCompletion())
        {
            OpenAirlock();
        }
        
    }

    private bool CheckPuzzleCompletion()
    {
        if( GaugeValues[0] == 5 && GaugeValues[1] == 5)
        {
            return true;
        }
        return false;
    }

    private void OpenAirlock()
    {
        for( int i = 0; i < 4; i++)
        {
            valves[i].GetComponent<Collider>().enabled = false;
        }
        //door.GetComponent<Collider>().enabled = true;
        door.GetComponent<Animator>().SetTrigger("Open");
    }

    void updateMaterial()
    {
        for (int i = 0; i < 2; i++)
        {
            switch( GaugeValues[i] )
            {
                case int n when n < 3:
                    gauges[i].transform.Find("CircleGauge").GetComponent<Renderer>().material = GaugeMat1;
                    Text[i].color = new Color(255, 0, 0, 255);
                    break;
                case int n when n < 5:
                    gauges[i].transform.Find("CircleGauge").GetComponent<Renderer>().material = GaugeMat2;
                    Text[i].color = new Color(255, 255, 0, 255);
                    break;
                case int n when n > 8:
                    gauges[i].transform.Find("CircleGauge").GetComponent<Renderer>().material = GaugeMat5;
                    Text[i].color = new Color(255, 0, 0, 255);
                    break;
                case int n when n > 5:
                    gauges[i].transform.Find("CircleGauge").GetComponent<Renderer>().material = GaugeMat4;
                    Text[i].color = new Color(255, 255, 0, 255);
                    break;
                default:
                    gauges[i].transform.Find("CircleGauge").GetComponent<Renderer>().material = GaugeMat3;
                    Text[i].color = new Color(0, 255, 0, 255);
                    break;

            }
        }
    
    }

    public void AdjustPressure(int gaugeVal1, int gaugeVal2)
    {
        GaugeValues[0] += gaugeVal1;
        GaugeValues[1] += gaugeVal2;
        Text[0].text = GaugeValues[0].ToString();
        Text[1].text = GaugeValues[1].ToString();
        updateMaterial();
    }

    void SetupPuzzle()
    {
        int num_of_used_valves = 0;
        valveUsage = ValveRandomizer(ref num_of_used_valves);
        

        
        //1st gaugeValue is below 5
        GaugeValues[0] = Random.Range(0,5);
        //2st gaugeValue is any number
        GaugeValues[1] = Random.Range(0,max_pressure + 1);
        int delta1 = 5 - GaugeValues[0];
        int delta2 = 5 - GaugeValues[1];

        int[][] usedValve = new int[num_of_used_valves][];
        for(int i = 0; i < num_of_used_valves; i++)
        {
            usedValve[i] = new int[2];
        }
        int[][] NotUsedValve = new int[4 - num_of_used_valves][];
        for(int i = 0; i < 4 - num_of_used_valves; i++)
        {
            NotUsedValve[i] = new int[2];
        }
        if (num_of_used_valves == 2)
        {
            int[] usedValve1 = {Random.Range(-4,5), Random.Range(-4,5)};
            int[] usedValve2 = {delta1 - usedValve1[0], delta2 - usedValve1[1]};
            int[] NotUsedvalve1 = {Random.Range(-4,5), Random.Range(-4,5)};
            int[] NotUsedvalve2 = {Random.Range(-4,5), Random.Range(-4,5)};
            bool flag = true;
            while(flag)
            {
                flag = false;
                if(NotUsedvalve1 == usedValve1 || NotUsedvalve1 == usedValve2)
                {
                    NotUsedvalve1[0] = Random.Range(-4,5);
                    NotUsedvalve1[1] = Random.Range(-4,5);
                    flag = true;
                }

                if(NotUsedvalve1 == usedValve1 || NotUsedvalve1 == usedValve2)
                {
                    NotUsedvalve2[0] = Random.Range(-4,5);
                    NotUsedvalve2[1] = Random.Range(-4,5);
                    flag = true;
                }


                if(NotUsedvalve1[0] + NotUsedvalve2[0] == 5 && NotUsedvalve1[1] + NotUsedvalve2[1] == 5)
                {
                    NotUsedvalve1[0] = Random.Range(-4,5);
                    NotUsedvalve1[1] = Random.Range(-4,5);

                    flag = true;
                }   
            }

            int k = 0;
            int j = 0;

            for (int i = 0; i < 4; i++)
            {
                if (valveUsage[i])
                {
                    if( k == 0)
                    {
                        ValveValues[i] = usedValve1;
                        k++;
                    }
                    else
                    {
                        ValveValues[i] = usedValve2;
                    }   
                }
                else
                {
                    if( j == 0)
                    {
                        ValveValues[i] = NotUsedvalve1;
                        j++;
                    }
                    else
                    {
                        ValveValues[i] = NotUsedvalve2;
                    }
                }
            }
        }

        else
        {
            int[] usedValve1 = {Random.Range(-4,5), Random.Range(-4,5)};
            int[] usedValve2 = {Random.Range(-4,5), Random.Range(-4,5)};
            int[] usedValve3 = {5 - (usedValve1[0] + usedValve2[0]), 5 - (usedValve1[1] + usedValve2[1])};
            int[] NotUsedvalve1 = {Random.Range(-4,5), Random.Range(-4,5)};

            bool flag = true;
            while(flag)
            {
                flag = false;
                if(NotUsedvalve1 == usedValve1 || 
                NotUsedvalve1 == usedValve2 || 
                NotUsedvalve1 == usedValve3 ||
                (NotUsedvalve1[0] + usedValve1[0] == 5 && NotUsedvalve1[1] + usedValve1[1]== 5) || 
                (NotUsedvalve1[0] + usedValve2[0] == 5 && NotUsedvalve1[1] + usedValve2[1]== 5) || 
                (NotUsedvalve1[0] + usedValve3[0] == 5 && NotUsedvalve1[1] + usedValve3[1]== 5) 
                )
                {
                    NotUsedvalve1[0] = Random.Range(-4,5);
                    NotUsedvalve1[1] = Random.Range(-4,5);
                    flag = true;
                }
            }

            int k = 0;

            for (int i = 0; i < 4; i++)
            {
                if (valveUsage[i])
                {
                    if( k == 0)
                    {
                        ValveValues[i] = usedValve1;
                        k++;
                    }
                    else if(k == 1)
                    {
                        ValveValues[i] = usedValve2;
                        k++;
                    }
                    else
                    {
                        ValveValues[i] = usedValve3;
                    }   
                }
                else
                {                
                    ValveValues[i] = NotUsedvalve1;
                }
            }
        }








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

    bool[]  ValveRandomizer(ref int num_of_used_valves)
    {
        bool[] valveUsage = new bool[4]; //should be 4 valves
        
        bool temp;
        //for pre-duel
        valveUsage[0] = false;
        valveUsage[1] = true;
        valveUsage[2] = false;
        valveUsage[3] = true;

        num_of_used_valves = 2;




        //uses only 2 valves for solution
        /*
        //1st valve
        temp = (Random.value > 0.65f); //35%
        valveUsage[0] = temp;
        if (temp)
            num_of_used_valves += 1;

        
        //2nd valve
        if (num_of_used_valves == 1)
            temp = (Random.value > 0.5f); //50% if we have 1 used valve
        else
            temp = (Random.value > 0.3f); //70% if we have no used valves
        valveUsage[1] = temp;
        if (temp)
            num_of_used_valves += 1;
        
        //3rd valve
        if (num_of_used_valves == 2 )
             temp = false; //0% if there is 2 values
        else if (num_of_used_valves == 1 )
            temp = (Random.value > 0.4f); //60% if we have 1 or 2 used valves
        else
            temp = true; //100% if we have no used valves
        valveUsage[2] = temp;
        if (temp)
            num_of_used_valves += 1;
        
        //4th value
        if (num_of_used_valves == 1)
            temp = true; //100% if we have 1 used valve
        else
            temp = false; // 0% if we have 2 used valves
        valveUsage[3] = temp;
        if (temp)
            num_of_used_valves += 1;
        */


        //uses both 2 valves or 3 valves for solution
        /*
        //1st valve
        temp = (Random.value > 0.5f); //50%
        valveUsage[0] = temp;
        if (temp)
            num_of_used_valves += 1;

        
        //2nd valve
        if (num_of_used_valves == 1)
            temp = (Random.value > 0.5f); //50% if we have 1 used valve
        else
            temp = (Random.value > 0.3f); //70% if we have no used valves
        valveUsage[1] = temp;
        if (temp)
            num_of_used_valves += 1;
        
        //3rd valve
        if (num_of_used_valves == 1 || num_of_used_valves == 2)
            temp = (Random.value > 0.4f); //60% if we have 1 or 2 used valves
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
        */
        
        return valveUsage;

    }

}