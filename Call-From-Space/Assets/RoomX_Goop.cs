using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomX_Goop : Interactable
{
    public GameObject Sparkle;
    Collider collider;
    public AudioSource audioSource;
    MeshRenderer meshRenderer;
    ParticleSystem flame;
    public BurningStem stemScript;
    // Start is called before the first frame update
    void Start()
    {
       flame = transform.Find("Flame").GetComponent<ParticleSystem>();
        //mesh = transform.Find("Goop").gameObject;
        flame.Stop();
        collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>(); 
    }

    public override string GetDescription()
    {
        if(!stemScript.doneBurning)
            return "<color=red>Burn</color=read> Down the Central Specimen First";
        return "<color=red>Burn</color=read> With FlameThrower";
    }
    public override void Interact()
    {
    }

    public void BurnGoop()
    {
        Sparkle.SetActive(false);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        flame.Play();
        audioSource.Play();
        while (meshRenderer.materials[0].color.a > 0f)
        {
            Color currentColor = meshRenderer.materials[0].color;
            currentColor.a -= .5f * Time.deltaTime;
            meshRenderer.materials[0].color = currentColor;
            yield return null;
        }
        flame.Stop();
        collider.enabled = false;
        meshRenderer.enabled = false;
    }
}
