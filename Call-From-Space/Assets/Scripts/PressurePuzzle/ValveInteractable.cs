using UnityEngine;

public class ValveInteractable : Interactable
{
    //public ValveController valveController;
    public GameObject valveManager;
    ValvePuzzleSetup controller;
    bool isOn;
    int GaugeVal1;
    int GaugeVal2;

    void Start()
    {
        controller = valveManager.GetComponent<ValvePuzzleSetup>();
    }

    public override string GetDescription()
    {
        return "Press [E] to turn the valve";
    }

    public override void Interact()
    {
        //gameobject.animate
        if(isOn)
            controller.AdjustPressure(-1* GaugeVal1, -1* GaugeVal2);
        else
            controller.AdjustPressure(GaugeVal1, GaugeVal2);
        
        isOn = !isOn;

        //valveController.ToggleValve();
    }

    public void setGaugeVals(int val1, int val2)
    {
        GaugeVal1 = val1;
        GaugeVal2 =  val2;
    }
}