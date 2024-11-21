using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CameraSwapper : MonoBehaviour
{

    public RawImage displayImage;
    public RenderTexture sharedRenderTexture;

    private Camera[] securityCameras;
    private int currentCameraIndex = 0;

    void Start()
    {
        // Find all security cameras in the scene
        securityCameras = GameObject.FindGameObjectsWithTag("SecurityCamera")
                                    .Select(go => go.GetComponent<Camera>())
                                    .Where(cam => cam != null)
                                    .ToArray();

        if (securityCameras.Length == 0 || displayImage == null)
        {
            Debug.LogError("CameraSwapper: No security cameras found or display image not set!");
            return;
        }

        // Create a shared render texture if not provided
        if (sharedRenderTexture == null)
        {
            sharedRenderTexture = new RenderTexture(256, 256, 24);
            sharedRenderTexture.name = "SharedSecurityCameraRT";
        }

        // Set up the first camera
        SetActiveCamera(0);
    }

    public void RefreshCameraList()
    {
        securityCameras = GameObject.FindGameObjectsWithTag("SecurityCamera")
                                    .Select(go => go.GetComponent<Camera>())
                                    .Where(cam => cam != null)
                                    .ToArray();
        if (securityCameras.Length == 0)
        {
            Debug.LogError("CameraSwapper: No security cameras found!");
            return;
        }
        disableCameras();
    }

    public void NextCamera()
    {
        SetActiveCamera((currentCameraIndex + 1) % securityCameras.Length);
    }

    public void PreviousCamera()
    {
        SetActiveCamera((currentCameraIndex - 1 + securityCameras.Length) % securityCameras.Length);
    }

    private void SetActiveCamera(int index)
    {
        // Disable all cameras
        disableCameras();

        // Enable and set up the selected camera
        currentCameraIndex = index;
        Camera activeCamera = securityCameras[currentCameraIndex];
        activeCamera.enabled = true;
        activeCamera.targetTexture = sharedRenderTexture;

        // Update the display image
        displayImage.texture = sharedRenderTexture;
    }

    public void disableCameras()
    {
       foreach (Camera cam in securityCameras)
        {
            cam.enabled = false;
            cam.targetTexture = null;
        } 
    }
}