using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopedDoor : Interactable
{
    // Start is called before the first frame update
    float fadeSpeed = .5f;
    public GameObject player;
    public GameObject Lighter;
    public GameObject Sparkle;
    ParticleSystem flame;
    //GameObject mesh;
    void Start()
    {
        flame = transform.Find("Flame").GetComponent<ParticleSystem>();
        //mesh = transform.Find("Goop").gameObject;
        flame.Stop();
    }

    // Update is called once per frame
    public override string GetDescription()
    {
        //adds the task to find a way to destroy the goop
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(1);

        if(player.GetComponent<Interactor>().holdingName == "Lighter")
        {
            if(Lighter.GetComponent<LighterScript>().isOpen)
            {
                return "<color=red>Burn</color=read> the foreign material";
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
        if(player.GetComponent<Interactor>().holdingName == "Lighter" && Lighter.GetComponent<LighterScript>().isOpen)
        {
            Sparkle.SetActive(false);
            StartCoroutine(FadeOut());
            player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(4);
            Lighter.GetComponent<LighterScript>().StopGlowEffect();
        }
    }


    IEnumerator FadeOut()
    {
        flame.Play();
        while (GetComponent<MeshRenderer>().materials[0].color.a > 0f)
        {
            Color currentColor = GetComponent<MeshRenderer>().materials[0].color;
            currentColor.a -= fadeSpeed * Time.deltaTime;
            GetComponent<MeshRenderer>().materials[0].color = currentColor;
            yield return null; 
        }
        flame.Stop();
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

    }
}
