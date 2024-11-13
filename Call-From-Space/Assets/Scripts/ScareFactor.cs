using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioTriggerImage : MonoBehaviour {
    public BoxCollider scareCollider;
    public GameObject scareImage;
    public AudioSource scareAudio;

    private bool isFirstTime = true;

    void Start () {
        if (scareImage != null) {
            scareImage.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (isFirstTime && other.CompareTag("Player"))
        {
            isFirstTime = true;
            StartCoroutine(ShowScareImage());
        }
    }   
    private IEnumerator ShowScareImage()
    {
        // Display the scare image
        if (scareImage != null)
        {
            scareImage.SetActive(true);
            scareAudio.Play();
        }

        // Wait for 1 second
        yield return new WaitForSeconds(0.15f);

        // Hide the scare image
        if (scareImage != null)
        {
            scareImage.SetActive(false);
        }
    }

    // public AudioSource audioSource;  
    // public AudioClip yesClip;        
    // public RawImage displayImage;       

    // private bool isImageDisplayed = false;

    // void Start()
    // {
    //     // Ensure the image is initially hidden
    //     if (displayImage != null)
    //     {
    //         displayImage.enabled = false;
    //     }
    // }

    // void Update()
    // {
    //     // Check if the specified audio is playing
    //     if (audioSource.isPlaying && audioSource.clip == yesClip && !isImageDisplayed)
    //     {
    //         StartCoroutine(ShowImageForOneSecond());
    //     }
    // }

    // IEnumerator ShowImageForOneSecond()
    // {
    //     // Display the image
    //     if (displayImage != null)
    //     {
    //         displayImage.enabled = true;
    //     }

    //     isImageDisplayed = true;

    //     // Wait for 1 second
    //     yield return new WaitForSeconds(1f);

    //     // Hide the image
    //     if (displayImage != null)
    //     {
    //         displayImage.enabled = false;
    //     }

    //     isImageDisplayed = false;
    // }
}
