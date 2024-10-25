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
        }
    }
}
