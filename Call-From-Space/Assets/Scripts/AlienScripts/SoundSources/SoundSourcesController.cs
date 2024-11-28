using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SoundSource = PathNode;

public class SoundSourcesController : MonoBehaviour
{
  float yLevel;
  List<SoundSource> soundSources = new();

  public static SoundSourcesController instance { get; private set; }

  void Start()
  {
    instance = this;
    yLevel = transform.position.y;
  }

  void Update()
  {
    // Debug.Log(soundSources.Count + " | " + AlienController.aliens.Count);
    soundSources.ForEach(source =>
    {
      Debug.DrawRay(source.pos, source.radius * 100 * Vector3.up, Color.white, 10);
      AlienController.aliens.ForEach(alien =>
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

  public void CreateNewSoundSource(Vector3 position, float radius)
  {
    position.y = yLevel;
    soundSources.Add(new()
    {
      pos = position,
      radius = radius
    });
  }
}

/*
  a source will only exist to the alien if alien was there to hear it at first
  if a new source appears go towards it
*/