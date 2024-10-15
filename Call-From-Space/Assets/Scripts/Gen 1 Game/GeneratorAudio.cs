using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Gen_1_Game
{
    public class GeneratorAudio : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip humSound;
        public float maxAudioDistance = 10f;
        public GameObject player;

        private bool isGeneratorOn = false;
        void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.clip = humSound;
            }

            if (audioSource != null)
            {
                audioSource.loop = true;
                audioSource.spatialBlend = 1f; 
                audioSource.minDistance = 1f;
                audioSource.maxDistance = maxAudioDistance;
                audioSource.playOnAwake = false;
            }

        }

        void Update()
        {

            if (isGeneratorOn && audioSource != null && player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                float volume = 1f - Mathf.Clamp01(distance / maxAudioDistance);
                audioSource.volume = volume;
            }

        }

        public void TurnOnGenerator()
        {
            isGeneratorOn = true;
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

    }
}