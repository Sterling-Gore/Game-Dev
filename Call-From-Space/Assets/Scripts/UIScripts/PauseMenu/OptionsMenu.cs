using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public CameraController cameraController;
    public void setVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }
    public void setSensitivity(float sens)
    {
        if (sens == 1)
            cameraController.mouseSensitivity = 25;
        else
            cameraController.mouseSensitivity = 50 * (sens - 1);
    }
}
