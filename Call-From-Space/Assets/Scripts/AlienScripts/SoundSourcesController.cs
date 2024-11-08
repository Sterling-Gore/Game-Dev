using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SoundSource = PathNode;

public class SoundSourcesController : MonoBehaviour
{
  float yLevel;
  List<AlienController> aliens = new();
  List<SoundSource> soundSources = new();

  void Start()
  {
    yLevel = transform.position.y;
    aliens = GameObject
      .FindGameObjectsWithTag("Alien")
      .Select(obj => obj.GetComponent<AlienController>())
      .ToList();
  }

  void Update()
  {
    // Debug.Log(soundSources.Count + " | " + aliens.Count);
    soundSources.ForEach(source =>
    {
      Debug.DrawRay(source.pos, Vector3.up * source.radius * 100, Color.white, 4);
      aliens.ForEach(alien =>
      {
        if (
          Vector3.Distance(alien.transform.position, source.pos) < source.radius &&
          !alien.blackListedSoundSources.Contains(source)
        )
          alien.nextTarget = source;
      });
    });
    soundSources.Clear();
  }

  public void SubscribeToSoundSources(AlienController alien) =>
    aliens.Append(alien);

  public void CreateNewSoundSource(Vector3 position, float radius)
  {
    position.y = yLevel;
    soundSources.Add(new()
    {
      pos = position,
      radius = radius
    });
  }

  public static SoundSourcesController GetInstance() =>
    GameObject.Find("SoundSourcesController").GetComponent<SoundSourcesController>();
}

/*
  a source will only exist to the alien if alien was there to hear it at first
  if a new source appears go towards it
*/