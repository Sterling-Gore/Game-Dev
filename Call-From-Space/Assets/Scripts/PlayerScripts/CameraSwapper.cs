using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//To add a security camera use the security camera prefab


public class CameraSwapper : MonoBehaviour
{
    Camera playerCamera;
    List<Camera> securityCameras;
    Camera activeSecurityCamera;
    RenderTexture activeCameraTexture;
    bool openManager=false;


    const string SECURITY_CAMERA = "SecurityCamera";
    const string MAIN_CAMERA = "MainCamera";

    void Start()
    {
        playerCamera = Camera.main;
        securityCameras = Camera.allCameras
                                .Where(camera => camera.CompareTag(SECURITY_CAMERA))
                                .ToList();
        securityCameras.ForEach(camera => camera.targetTexture = new RenderTexture(512, 512, 16));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ToggleManagerWindow();
    }

    private void OnGUI()
    {
        if (!openManager || securityCameras.Count==0)
            return;
        
        var cameraViewWidth = Screen.width / securityCameras.Count;
        var cameraViewHeight = Screen.height / securityCameras.Count;
        var cameraViewOptions = new []{ GUILayout.Width(cameraViewWidth), GUILayout.Height(cameraViewHeight) };

        // GUILayout.Box("choose a camera");
        GUILayout.BeginHorizontal();
        securityCameras.ForEach(camera =>
        {
            if (GUILayout.Button(camera.activeTexture, cameraViewOptions))
            {
                UseSecurityCamera(camera);
                ToggleManagerWindow();
            }
        });
        GUILayout.EndHorizontal();
    }

    void ToggleManagerWindow()
    {
        if (openManager) OpenManagerWindow(false);
        else if (activeSecurityCamera == null) OpenManagerWindow();
        else UsePlayerCamera();
    }

    void OpenManagerWindow(bool open=true)
    {
        Cursor.visible = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        openManager = open;
        securityCameras.ForEach(camera =>
        {
            if (camera != activeSecurityCamera) camera.enabled = open;
        });
    }

    void UsePlayerCamera()
    {
        activeSecurityCamera.tag = SECURITY_CAMERA;
        activeSecurityCamera.targetTexture = activeCameraTexture;
        playerCamera.enabled = true;
        activeSecurityCamera = null;
    }

    void UseSecurityCamera(Camera camera)
    {
        activeSecurityCamera = camera;
        camera.tag = MAIN_CAMERA;
        activeCameraTexture = camera.targetTexture;
        camera.targetTexture = null;
        playerCamera.enabled = false;
    }
}
