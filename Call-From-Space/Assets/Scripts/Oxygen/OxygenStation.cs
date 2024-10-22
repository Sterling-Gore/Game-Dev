using UnityEngine;
using UnityEngine.UI;

namespace GameDev.Scripts.Oxygen
{
    public class OxygenStation : Interactable
    {
        //public Text interactionText; // Reference to the UI Text component
        public OxygenSystem oxygenSystem;

        
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
                oxygenSystem.IncreaseOxygen(); // Call the RefillOxygen method
            }
            else
            {
                Debug.Log("No OxygenSystem component found.");
            }
        }

    }
}