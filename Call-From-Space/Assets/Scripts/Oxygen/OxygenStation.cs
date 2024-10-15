using UnityEngine;
using UnityEngine.UI;

namespace GameDev.Scripts.Oxygen
{
    public class OxygenStation : MonoBehaviour
    {
        public Text interactionText; // Reference to the UI Text component

        private void Start()
        {
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(false); // Hide the text initially
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(true); // Show the text
                }

                if (Input.GetKey(KeyCode.E))
                {
                    OxygenSystem oxygenSystem = other.GetComponent<OxygenSystem>();
                    if (oxygenSystem != null)
                    {
                        oxygenSystem.RefillOxygen();
                    }
                    else
                    {
                        Debug.LogWarning("OxygenSystem component not found on Player.");
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (interactionText != null)
                {
                    interactionText.gameObject.SetActive(false); // Hide the text when the player leaves
                }
            }
        }
    }
}