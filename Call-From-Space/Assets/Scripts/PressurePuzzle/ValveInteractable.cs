using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

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
    
    public AudioSource audioSource;
    private Collider valveCollider;
    public AudioClip[] valveSounds;


    void Start()
    {
        
        controller = valveManager.GetComponent<ValvePuzzleSetup>();
        animation = GetComponent<Animator>();
        lightRenderer = transform.Find("Light").GetComponent<Renderer>();
        isOn = false;
        UpdateMaterial();
        valveCollider = GetComponent<Collider>();
    }

    public override string GetDescription()
    {
        return "Press [E] to turn the valve";
    }

    public override void Interact()
    {
        StartCoroutine(InteractSequence());
    }

    private IEnumerator InteractSequence()
    {
        valveCollider.enabled = false;
        isOn = !isOn;
        UpdateMaterial();
        animation.SetTrigger(isOn ? "On" : "Off");
        PlaySound();
        yield return StartCoroutine(AdjustPressureAfterDelay());
        valveCollider.enabled = true;
    }

    private void PlaySound()
    {
        if (audioSource && valveSounds != null && valveSounds.Length > 0)
        {
            AudioClip randomClip = valveSounds[Random.Range(0, valveSounds.Length)];
            audioSource.clip = randomClip;
            audioSource.Play();
        }
    }

    private IEnumerator AdjustPressureAfterDelay()
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
        if (lightRenderer)
        {
            lightRenderer.material = isOn ? onMaterial : offMaterial;
        }
        else
        {
            Debug.LogError("Renderer component not found on the valve object!");
        }
    }
}