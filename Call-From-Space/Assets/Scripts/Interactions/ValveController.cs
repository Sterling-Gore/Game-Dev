using UnityEngine;

public class ValveController : MonoBehaviour
{
    public int[] gaugeEffects;
    public GaugeController[] gauges;
    public bool isOn = false;

    public Material offMaterial;
    public Material onMaterial;

    private Renderer valveRenderer;

    private void Start()
    {
        valveRenderer = GetComponent<Renderer>();
        UpdateMaterial();
    }

    public void ToggleValve()
    {
        isOn = !isOn;
        ApplyEffects();
        UpdateMaterial();
    }

    private void ApplyEffects()
    {
        int multiplier = isOn ? 1 : -1;
        for (int i = 0; i < gauges.Length; i++)
        {
            int effect = gaugeEffects[i] * multiplier;
            gauges[i].AdjustPressure(effect);
        }
    }


    public void UpdateMaterial()
    {
        if (valveRenderer != null)
        {
            valveRenderer.material = isOn ? onMaterial : offMaterial;
        }
        else
        {
            Debug.LogError("Renderer component not found on the valve object!");
        }
    }

}