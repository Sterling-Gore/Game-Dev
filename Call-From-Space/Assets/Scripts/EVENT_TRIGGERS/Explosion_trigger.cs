using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Explosion_trigger : LoadableTrigger
{
    public GameObject StationDoor;
    public GameObject player;
    public GameObject ShuttleLights;
    public AudioSource Explosion;
    public AudioSource AI_warning;
    public CameraShakeGeneral cameraShake;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameStateManager.instance.SaveGame(GameStateManager.checkPointFilePath);
            cameraShake.StartShake(2f, 0.8f);
            Action();
        }
    }

    void Action()
    {
        Explosion.enabled = true;
        StationDoor.GetComponent<Animator>().SetTrigger("Closed");
        ShuttleLights.SetActive(false);
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().Explosion();
        StartCoroutine(AiDelay());
    }

    IEnumerator AiDelay()
    {
        yield return new WaitForSeconds(2f);
        AI_warning.enabled = true;
        gameObject.SetActive(false);
        yield return null;
    }

    void ReverseAction()
    {
        Explosion.enabled = false;
        StationDoor.GetComponent<Animator>().SetTrigger("Open");
        ShuttleLights.SetActive(true);
        player.GetComponent<PlayerController>().TaskList_UI_Object.GetComponent<TaskList>().UndoExplosion();
    }

    public override void Load(JObject state)
    {
        State start, end;
        start = getState(Explosion.enabled);

        base.Load(state);
        end = getState((bool)state[fullName]["wasTurnedOn"]);


        if (start == end)
        {
            if (start == State.after)
            {
                ReverseAction();
                Action();
            }
            else
                return;
        }
        if (end == State.after)
            Action();
        else if (start == State.after)
            ReverseAction();

        State getState(bool wasTurnedOn)
        {
            if (gameObject.activeSelf)
                return State.during;
            else if (wasTurnedOn)
                return State.after;
            else return State.before;
        }
    }

    public override void Save(ref JObject state)
    {
        base.Save(ref state);
        state[fullName]["wasTurnedOn"] = Explosion.enabled;
    }

    enum State
    {
        before, during, after
    }
    /*
    before during after
    before during-> only enable
    before after-> only actions
    during before->disable
    during after-> actions and disable
    after before->do inverse actions
    after during->inverse and enable
    */
}
