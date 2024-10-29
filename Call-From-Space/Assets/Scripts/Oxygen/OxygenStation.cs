using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameDev.Scripts.Oxygen
{
    public class OxygenStation : Interactable
    {
        //public Text interactionText; // Reference to the UI Text component
        public OxygenSystem oxygenSystem;
        public GameObject oxygenRadial;
        public AudioSource refillAudio;
        public AudioSource refillComplete;
        float previousO2;


        void Update()
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                oxygenRadial.SetActive(false);
                refillAudio.enabled = false;
                refillComplete.enabled = true;
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
                refillAudio.enabled = true;
                oxygenSystem.IncreaseOxygen(); // Call the RefillOxygen method
                if(oxygenSystem.oxygenLevel == 100f && previousO2 < 100f)
                {
                    refillComplete.Play(0);

                }
                previousO2 = oxygenSystem.oxygenLevel;
            }
            else
            {
                Debug.Log("No OxygenSystem component found.");
            }
        }


    }
}