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
        bool RefillAudioIsReadyToPlay;


        public GameObject player;
        bool haveAccessedYet;

        public GameObject Sparkle;
 
        void Start()
        {
            RefillAudioIsReadyToPlay = true;
            haveAccessedYet = false;
        }

        void Update()
        {
            if(Input.GetKeyUp(KeyCode.E))
            {
                oxygenRadial.SetActive(false);
                refillAudio.enabled = false;
                RefillAudioIsReadyToPlay = true;
            }
        }
        public override string GetDescription()
        {
            return ("<color=red>Hold [E]</color=red> to Refill Oxygen");
        }

        public override void Interact()
        {

            if(!haveAccessedYet)
            {
                player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().DeleteTask("Find An Oxygen Station");
                Sparkle.SetActive(false);
            }
            Debug.Log("Starting to refill oxygen...");
            // OxygenSystem oxygenSystem = gameObject.GetComponent<OxygenSystem>(); // Get the OxygenSystem component
            if (oxygenSystem != null)
            {
                oxygenRadial.SetActive(true);
                if (RefillAudioIsReadyToPlay)
                {
                    refillAudio.enabled = true;
                }
                oxygenSystem.IncreaseOxygen(); // Call the RefillOxygen method
                if(oxygenSystem.oxygenLevel == 100f && previousO2 < 100f)
                {
                    refillComplete.Play(0);
                    refillAudio.enabled = false;
                    RefillAudioIsReadyToPlay = false;

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