using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class GoopedDoor : Interactable
{
    // Start is called before the first frame update
    float fadeSpeed = .5f;
    public GameObject player;
    public LighterScript Lighter;
    public GameObject Sparkle;

    public AudioSource audioSource;
    ParticleSystem flame;
    //GameObject mesh;
    Collider collider;
    MeshRenderer meshRenderer;
    void Start()
    {
        flame = transform.Find("Flame").GetComponent<ParticleSystem>();
        //mesh = transform.Find("Goop").gameObject;
        flame.Stop();
        collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    public override string GetDescription()
    {
        //adds the task to find a way to destroy the goop
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(1);

        if (player.GetComponent<Interactor>().holdingName == "Lighter")
        {
            if (Lighter.isOpen)
            {
                return "Press [E] to <color=red>Burn</color=read> the foreign material";
            }
            else
            {
                return "Turn on the lighter";
            }
        }
        return "Blocked by foreign material";

    }

    public override void Interact()
    {
        if (player.GetComponent<Interactor>().holdingName == "Lighter" && Lighter.isOpen)
        {
            Sparkle.SetActive(false);
            StartCoroutine(FadeOut());
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(4);
            Lighter.StopGlowEffect();
        }
    }


    IEnumerator FadeOut()
    {
        flame.Play();
        audioSource.Play();
        while (meshRenderer.materials[0].color.a > 0f)
        {
            Color currentColor = meshRenderer.materials[0].color;
            currentColor.a -= fadeSpeed * Time.deltaTime;
            meshRenderer.materials[0].color = currentColor;
            yield return null;
        }
        flame.Stop();
        collider.enabled = false;
        meshRenderer.enabled = false;
        AlienController.aliens.ForEach(alien => alien.ReloadPathGraph());
    }

    public override void Load(JObject state)
    {
        base.Load(state);
        var active = (bool)state[fullName]["isActive"];
        Sparkle.SetActive(active);

        Color currentColor = meshRenderer.materials[0].color;
        currentColor.a = active ? 1 : 0;
        meshRenderer.materials[0].color = currentColor;

        collider.enabled = active;
        meshRenderer.enabled = active;
    }

    public override void Save(ref JObject state)
    {
        base.Save(ref state);
        state[fullName]["isActive"] = Sparkle.activeSelf;
    }
}
