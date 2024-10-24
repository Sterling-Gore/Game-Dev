using UnityEngine;
using System.Collections.Generic;

public class PowerLevel : MonoBehaviour
{
    [System.Serializable]
    public class PowerZone
    {
        public string zoneName;
        public GameObject[] lights;
        public GameObject[] cameras;
        public int requiredPowerLevel;
    }

    [Header("Power Settings")]
    public int currentPowerLevel = 0;
    public int maxPowerLevel = 1; 

    [Header("Power Zones")]
    public PowerZone[] powerZones;

    [Header("Camera System")]
    public CameraSwapper cameraSwapper;

    private int activeGenerators = 0;
    private Dictionary<GameObject, bool> originalCameraStates = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> originalLightStates = new Dictionary<GameObject, bool>();

    void Start()
    {
        foreach (PowerZone zone in powerZones)
        {
            foreach (GameObject camera in zone.cameras)
            {
                if (camera != null)
                {
                    originalCameraStates[camera] = camera.activeSelf;
                    camera.SetActive(false);
                }
            }

            foreach (GameObject light in zone.lights)
            {
                if (light != null)
                {
                    originalLightStates[light] = light.activeSelf;
                    light.SetActive(false);
                }
            }
        }

        // Initial power level setup
        UpdatePowerSystems();
    }

    public void GeneratorActivated()
    {
        activeGenerators = Mathf.Min(activeGenerators + 1, maxPowerLevel);
        currentPowerLevel = activeGenerators;
        UpdatePowerSystems();
        Debug.Log($"Generator activated. Current power level: {currentPowerLevel}");
    }

    public void GeneratorDeactivated()
    {
        activeGenerators = Mathf.Max(activeGenerators - 1, 0);
        currentPowerLevel = activeGenerators;
        UpdatePowerSystems();
        Debug.Log($"Generator deactivated. Current power level: {currentPowerLevel}");
    }

    private void UpdatePowerSystems()
    {
        Debug.Log($"Updating power systems. Current level: {currentPowerLevel}");

        foreach (PowerZone zone in powerZones)
        {
            bool shouldBeActive = currentPowerLevel >= zone.requiredPowerLevel;

            foreach (GameObject light in zone.lights)
            {
                if (light != null)
                {
                    bool originalState = originalLightStates[light];
                    light.SetActive(shouldBeActive && originalState);
                }
            }

            foreach (GameObject camera in zone.cameras)
            {
                if (camera != null)
                {
                    bool originalState = originalCameraStates[camera];
                    camera.SetActive(shouldBeActive && originalState);
                }
            }
        }

        if (cameraSwapper != null)
        {
            cameraSwapper.enabled = false;
            cameraSwapper.enabled = true;
        }
    }

    public int GetCurrentPowerLevel()
    {
        return currentPowerLevel;
    }

    public bool IsZonePowered(string zoneName)
    {
        foreach (var zone in powerZones)
        {
            if (zone.zoneName == zoneName)
            {
                return currentPowerLevel >= zone.requiredPowerLevel;
            }
        }
        return false;
    }
}