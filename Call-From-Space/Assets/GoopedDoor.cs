using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoopedDoor : Interactable
{
    // Start is called before the first frame update
    float fadeSpeed = .5f;
    public GameObject player;
    public GameObject Lighter;
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
        if(player.GetComponent<Interactor>().holdingName == "Lighter")
        {
            if(Lighter.GetComponent<LighterScript>().isOpen)
            {
                return "Burn the foreign material";
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
            StartCoroutine(FadeOut());
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
