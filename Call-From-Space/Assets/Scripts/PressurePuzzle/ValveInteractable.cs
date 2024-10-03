using UnityEngine;

public class ValveInteractable : Interactable
{
    //public ValveController valveController;
    public GameObject valveManager;
    ValvePuzzleSetup controller;
    bool isOn;
    int GaugeVal1;
    int GaugeVal2;

    private Animator animation;
    private Renderer lightRenderer;

    public Material offMaterial;
    public Material onMaterial;

    void Start()
    {
        controller = valveManager.GetComponent<ValvePuzzleSetup>();
        animation = GetComponent<Animator>();
        lightRenderer = transform.Find("Light").GetComponent<Renderer>();
        isOn = false;
        UpdateMaterial();
    }

    public override string GetDescription()
    {
        return "Press [E] to turn the valve";
    }

    public override void Interact()
    {
        //gameobject.animate
        isOn = !isOn;
        UpdateMaterial();
        animation.SetTrigger(isOn ? "On" : "Off");
        StartCoroutine(AdjustPressureAfterDelay());
        //if (isOn)
        //{
        //    controller.AdjustPressure(-1* GaugeVal1, -1* GaugeVal2);
        //    animation.SetTrigger("Off");
        //}
        //else
        //{
        //    controller.AdjustPressure(GaugeVal1, GaugeVal2);
        //    animation.SetTrigger("On");
        //}
        //valveController.ToggleValve();
    }

    private System.Collections.IEnumerator AdjustPressureAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);
        if (isOn)
        {
            controller.AdjustPressure(GaugeVal1, GaugeVal2);
        }
        else
        {
            controller.AdjustPressure(-GaugeVal1, -GaugeVal2);
        }
    }

    public void setGaugeVals(int val1, int val2)
    {
        GaugeVal1 = val1;
        GaugeVal2 =  val2;
    }

    public void UpdateMaterial()
    {
        if (lightRenderer != null)
        {
            lightRenderer.material = isOn ? onMaterial : offMaterial;
        }
        else
        {
            Debug.LogError("Renderer component not found on the valve object!");
        }
    }
}