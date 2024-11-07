using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayJournal : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource Audio;

    public List<AudioClip> JorunalAudios;

    public void PlayAudio(GameObject temp)
    {
        int clip = temp.GetComponent<Item_interaction>().item.AudioLog;
        
        if(Audio.clip == JorunalAudios[clip] && Audio.isPlaying)
        {
            Audio.Stop();
        }
        else
        {
            Audio.clip = JorunalAudios[clip];
            Audio.Play();
        }
    } 

    public void PlayAudioOnPickUp(Item item)
    {
        int clip = item.AudioLog;
        Audio.clip = JorunalAudios[clip];
        Audio.Play();
        
    }


}
