using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningStem : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    public GameObject flameObject;
    public bool doneBurning;
    ParticleSystem flame;

    void Start()
    {
        doneBurning = false;
        flame = flameObject.GetComponent<ParticleSystem>();
        flame.Stop();
    }

    public void burnPlant()
    {
        if(!doneBurning)
            StartCoroutine(burn());
    }

    IEnumerator burn()
    {
        flame.Play();
        float time = 2f;
        while(time > 0f && !doneBurning)
        {
            time -= Time.deltaTime;
            foreach(MeshRenderer meshRenderer in meshRenderers)
            {
                Color currentColor = meshRenderer.materials[0].color;
                currentColor.r = Mathf.Clamp(currentColor.r - (0.001f * Time.deltaTime), 0, 1);
                currentColor.g = Mathf.Clamp(currentColor.g - (0.001f * Time.deltaTime), 0, 1);
                currentColor.b = Mathf.Clamp(currentColor.b - (0.001f * Time.deltaTime), 0, 1);
                meshRenderer.materials[0].color = currentColor;
                if(currentColor.r == 0)
                    doneBurning = true;
            }
            yield return null;

        }
        flame.Stop();
    }
}
