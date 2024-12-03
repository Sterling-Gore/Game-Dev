using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class DeathMenu : MonoBehaviour
{
    Interactor interactor;
    public AudioSource ambiance;
    public AudioSource VHSstatic;
    //public AudioMixer audioMixer;
    float volume = 100f;

    void OnEnable()
    {
        Debug.Log("DeathMenu enabled");
        var player = GameObject.Find("player");
        interactor = player.GetComponent<Interactor>();
        interactor.inUI = true;
        ambiance.enabled = false;
        VHSstatic.enabled = true;
        Time.timeScale = 0f;
    }

    void OnDisable()
    {
        Time.timeScale = 1f;
        //audioMixer.SetFloat("volume", volume);
        ambiance.enabled = true;
        VHSstatic.enabled = false;
    }

    public void GoToCheckPoint()
    {
        GameStateManager.instance.LoadGame(GameStateManager.checkPointFilePath);
        gameObject.SetActive(false);
        interactor.inUI = false;
    }

    public void ExitGame()
    {
        gameObject.SetActive(false);
        SceneManager.LoadSceneAsync("Start");
    }
}
