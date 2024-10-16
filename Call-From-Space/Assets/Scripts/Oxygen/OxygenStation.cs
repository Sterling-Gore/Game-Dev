using UnityEngine;
using UnityEngine.UI;

namespace GameDev.Scripts.Oxygen
{
    public class OxygenStation : Interactable
    {
        //public Text interactionText; // Reference to the UI Text component
        public OxygenSystem oxygenSystem;
        private void Start()
        {
            //if (interactionText != null)
            //{
            //    interactionText.gameObject.SetActive(false); // Hide the text initially
            //}
        }
        
        public override string GetDescription()
        {
            return ("Oxygen Station");
        }

        public override void Interact()
        {
            Debug.Log("Starting to refill oxygen...");
            // OxygenSystem oxygenSystem = gameObject.GetComponent<OxygenSystem>(); // Get the OxygenSystem component
            if (oxygenSystem != null)
            {
                oxygenSystem.RefillOxygen(); // Call the RefillOxygen method
            }
            else
            {
                Debug.Log("No OxygenSystem component found.");
            }
        }

        /*

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
        */
    }
}