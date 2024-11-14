using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioTriggerImage : MonoBehaviour {
    public BoxCollider scareCollider;
    public GameObject scareImage; 
    public RawImage[] scareImages; 
    public AudioSource scareAudio;

    private bool isFirstTime = true;

    void Start() {
        if (scareImage != null) {
            scareImage.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (isFirstTime) {
                isFirstTime = false; 
                int randomIndex = Random.Range(0, scareImages.Length);
                StartCoroutine(ShowScareImage(randomIndex));
            } else {
                if (Random.value <= 0.05f) { 
                    int randomIndex = Random.Range(0, scareImages.Length);
                    StartCoroutine(ShowScareImage(randomIndex));
                }
            }
        }
    }

    private IEnumerator ShowScareImage(int randomIndex) {
        if (scareImage != null && scareImages.Length > 0) {
            Texture selectedTexture = scareImages[randomIndex].texture;
            scareImage.GetComponent<RawImage>().texture = selectedTexture;
            scareImage.SetActive(true);
            scareAudio.Play();
        }

        // Wait for 0.15 seconds
        yield return new WaitForSeconds(0.15f);

        // Hide the scare image
        if (scareImage != null) {
            scareImage.SetActive(false);
        }
    }
}
