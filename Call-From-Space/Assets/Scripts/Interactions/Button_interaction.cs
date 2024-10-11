using UnityEngine;

public class Button_interaction : Interactable
{
    bool _doorOpened;
    Animator _doorAnimator;
    
    //used to change the color of the shuttle_light
    public Material myMaterial;
    public Texture redTexture;
    public Texture greenTexture;

    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip doorOpenSound;
    

    void Start()
    {   
        myMaterial.mainTexture = redTexture;
        _doorOpened = false;
        _doorAnimator = GetComponent<Animator>();
        
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        audioSource.playOnAwake = false;
    }

    public override string GetDescription()
    {
        if (!_doorOpened){
            return ("Press [E] to <color=red>open<color=red> the door.");
        }
        return ("");
    }

    public override void Interact()
    {
        if (!_doorOpened)
        {
            _doorAnimator.SetTrigger("open");
            myMaterial.mainTexture = greenTexture;
            
            PlaySound(buttonClickSound);
            
            StartCoroutine(PlayDelayedSound(doorOpenSound, 0.5f));
            
            _doorOpened = true;
        }
    }
    
    private void PlaySound(AudioClip clip)
    {
        if (audioSource  && clip )
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is missing!");
        }
    }
    
    private System.Collections.IEnumerator PlayDelayedSound(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySound(clip);
    }
}