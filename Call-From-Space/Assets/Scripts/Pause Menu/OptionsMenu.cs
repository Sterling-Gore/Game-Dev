using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour {
    public AudioMixer audioMixer;
    public CameraController cameraController;
    public void setVolume (float volume) {
        audioMixer.SetFloat("volume", volume);
    }
    public void setSensitivity (float sens ) {
        if (sens == 1) {
            cameraController.mouseSensitivity = 25;
        } else if (sens == 2) {
            cameraController.mouseSensitivity = 50;
        } else if (sens == 3) {
            cameraController.mouseSensitivity = 100;
        } else if (sens == 4) {
            cameraController.mouseSensitivity = 150;
        } else if (sens == 5) {
            cameraController.mouseSensitivity = 200;
        } else if (sens == 6) {
            cameraController.mouseSensitivity = 250;
        } else if (sens == 7) {
            cameraController.mouseSensitivity = 300;
        } else if (sens == 8) {
            cameraController.mouseSensitivity = 350;
        } else if (sens == 9) {
            cameraController.mouseSensitivity = 400;
        } else if (sens == 10) {
            cameraController.mouseSensitivity = 450;
        } else if (sens == 11) {
            cameraController.mouseSensitivity = 500;
        } else if (sens == 11) {
            cameraController.mouseSensitivity = 550;
        }
    }
}
