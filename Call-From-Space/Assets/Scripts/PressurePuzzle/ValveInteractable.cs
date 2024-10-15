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
        string soundPath = Path.Combine(Application.dataPath, "Sound", "Valves");
        DirectoryInfo dir = new DirectoryInfo(soundPath);
        FileInfo[] info = dir.GetFiles("*.mp3");
        
        if (info.Length == 0)
        {
            Debug.LogWarning("No .mp3 files found in the Sound/Valves folder!");
            return;
        }

        string randomFile = info[Random.Range(0, info.Length)].FullName;
        StartCoroutine(LoadAndPlayAudio(randomFile));
    }

    private System.Collections.IEnumerator LoadAndPlayAudio(string path)
    {
        using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            if (clip)
            {
                if (audioSource)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Audio source not found on the valve object!");
                }
            }
            else
            {
                Debug.LogError("Failed to create AudioClip from file: " + path);
            }
        }
        else
        {
            Debug.LogError("Error loading audio file: " + www.error);
        }
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