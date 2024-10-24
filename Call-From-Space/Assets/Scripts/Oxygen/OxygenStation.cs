using UnityEngine;
using UnityEngine.UI;

namespace GameDev.Scripts.Oxygen
{
    public class OxygenStation : Interactable
    {
        //public Text interactionText; // Reference to the UI Text component
        public OxygenSystem oxygenSystem;
        public GameObject oxygenRadial;

        void Update()
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                oxygenRadial.SetActive(false);
            }
        }
        public override string GetDescription()
        {
            return ("<color=red>Hold [E]</color=red> to Refill Oxygen");
        }

        public override void Interact()
        {
            Debug.Log("Starting to refill oxygen...");
            // OxygenSystem oxygenSystem = gameObject.GetComponent<OxygenSystem>(); // Get the OxygenSystem component
            if (oxygenSystem != null)
            {
                oxygenRadial.SetActive(true);
                oxygenSystem.IncreaseOxygen(); // Call the RefillOxygen method
            }
            else
            {
                Debug.Log("No OxygenSystem component found.");
            }
        }

    }
}