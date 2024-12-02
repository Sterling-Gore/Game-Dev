using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip[] soundEffects; // Add sound effects here
    public AudioClip[] musicTracks; // Add music tracks here

    [Header("Audio Settings")]
    public int audioSourcePoolSize = 5; // Number of reusable AudioSources
    private List<AudioSource> audioSources = new List<AudioSource>();
    private AudioSource musicSource; // Dedicated music source

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize Audio Sources
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSources.Add(source);
        }

        // Dedicated Music AudioSource
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true; // Music should loop by default
    }

    // Play a sound effect by name
    public void PlaySound(string clipName)
    {
        AudioClip clip = GetClipByName(soundEffects, clipName);
        if (clip != null)
        {
            AudioSource source = GetAvailableAudioSource();
            source.clip = clip;
            source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound effect '{clipName}' not found!");
        }
    }

    // Play music by name
    public void PlayMusic(string clipName)
    {
        AudioClip clip = GetClipByName(musicTracks, clipName);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music track '{clipName}' not found!");
        }
    }

    // Stop music playback
    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Get an available (non-playing) AudioSource
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSources)
        {
            if (!source.isPlaying)
                return source;
        }
        // If no sources are available, use the first one (optional behavior)
        return audioSources[0];
    }

    // Helper to find an AudioClip by name
    private AudioClip GetClipByName(AudioClip[] clips, string clipName)
    {
        foreach (AudioClip clip in clips)
        {
            if (clip.name == clipName)
                return clip;
        }
        return null;
    }
}
