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
        public GameObject[] doors;
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
    private Dictionary<GameObject, bool> originalColliderStates = new Dictionary<GameObject, bool>();

    public GameObject alienSystem;

    private LightmapData[] level0, level1, level2, level3;

    public Texture2D[] level0color, level1color, level2color, level3color;

    void Start()
    {
        level0 = CreateLightmap(level0color);
        level1 = CreateLightmap(level1color);
        level2 = CreateLightmap(level2color);
        level3 = CreateLightmap(level3color);

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

            foreach (GameObject door in zone.doors)
            {
                if (door != null)
                {
                    BoxCollider doorCollider = door.GetComponent<BoxCollider>();
                    if (doorCollider != null)
                    {
                        originalColliderStates[door] = doorCollider.enabled;
                        doorCollider.enabled = false;
                    }
                }
            }
        }

        // Initial power level setup
        UpdatePowerSystems();
    }


    private LightmapData[] CreateLightmap(Texture2D[] colorTextures)
    {
        List<LightmapData> lightmapList = new List<LightmapData>();
        for (int i = 0; i < colorTextures.Length; i++)
        {
            LightmapData lightmapData = new LightmapData();
            lightmapData.lightmapColor = colorTextures[i];
            lightmapList.Add(lightmapData);
        }
        return lightmapList.ToArray();
    }

    public void GeneratorActivated()
    {
        if(activeGenerators == 0)
        {
            alienSystem.SetActive(true);
        }
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

        switch (currentPowerLevel)
        {
            case 0:
                LightmapSettings.lightmaps = level0;
                break;
            case 1:
                LightmapSettings.lightmaps = level1;
                break;
            case 2:
                LightmapSettings.lightmaps = level2;
                break;
            case 3:
                LightmapSettings.lightmaps = level3;
                break;
            default:
                LightmapSettings.lightmaps = level0;
                break;
        }

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

            foreach (GameObject door in zone.doors)
            {
                if (door != null)
                {
                    BoxCollider doorCollider = door.GetComponent<BoxCollider>();
                    if (doorCollider != null)
                    {
                        bool originalState = originalColliderStates[door];
                        doorCollider.enabled = shouldBeActive && originalState;
                    }
                }
            }
        }

        if (cameraSwapper != null)
        {
            cameraSwapper.RefreshCameraList();
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