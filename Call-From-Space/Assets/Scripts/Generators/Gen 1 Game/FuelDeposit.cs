using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelDeposit : Interactable
{
    public GameObject FuelCell;
    public GameObject player;
    public PowerLevel powerLevel;
    public float soundRadius;
    public Vector3 rotation;
    public Vector3 position;
    public GameObject particles;
    public GenScreenInteraction gen;
    public AudioSource genPowerAudio;
    public AudioSource genRepeatingAudio;
    public GameObject Sparkle;





    void OnTriggerEnter(Collider other)
    {
        if (/*other.CompareTag("FuelCell")*/ other == FuelCell.GetComponent<Collider>() && !player.GetComponent<Interactor>().isHolding)
        {
            FuelCell.GetComponent<Rigidbody>().useGravity = false;
            FuelCell.GetComponent<Rigidbody>().isKinematic = true;
            FuelCell.GetComponent<Holdable>().enabled = false;
            FuelCell.transform.position = position;
            FuelCell.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
            FuelCell.GetComponent<FuelCellHoldable>().StopGlowEffect();
            genPowerAudio.enabled = true;
            //genRepeatingAudio.enabled = true;
            //lights.SetActive(true);
            //PowerLevel powerLevel = FindObjectOfType<PowerLevel>();
            if (powerLevel != null)
            {
                Debug.Log("Fuel cell deposited");
                powerLevel.GeneratorActivated();
                SoundSourcesController.instance.CreateNewSoundSource(transform.position, soundRadius);
            }
            switch (gen.generatorType)
            {
                case GenScreenInteraction.Generator.A:
                    player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle1(5);
                    break;
                case GenScreenInteraction.Generator.B:
                    player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle2(7);
                    break;
                case GenScreenInteraction.Generator.C:
                    player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().GenPuzzle3(3);
                    break;
                default:
                    break;
            }
            gameObject.GetComponent<Collider>().enabled = false;

            Sparkle.SetActive(false);
            StartCoroutine(waiter());
        }
    }

    IEnumerator waiter()
    {
        yield return new WaitForSeconds(6);
        genRepeatingAudio.enabled = true;
        particles.SetActive(true);
    }

    public override string GetDescription()
    {
        return "Deposit Fuel Cell";
    }

    public override void Interact()
    {

    }
}
