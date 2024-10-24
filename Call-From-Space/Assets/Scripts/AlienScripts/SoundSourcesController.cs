using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
      Debug.DrawRay(source.position, Vector3.up * source.radius * 100, Color.white, 4);
      aliens.ForEach(alien =>
      {
        if (
          Vector3.Distance(alien.transform.position, source.position) < source.radius &&
          !alien.blackListedSoundSources.Contains(source)
        )
          alien.target = source;
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
      position = position,
      radius = radius
    });
  }

  public static SoundSourcesController GetInstance() =>
    GameObject.Find("SoundSourcesController").GetComponent<SoundSourcesController>();
}

public struct SoundSource
{
  public Vector3 position;
  public float radius;

  public static bool operator ==(SoundSource a, SoundSource b) =>
    a.position == b.position && a.radius == b.radius;
  public static bool operator !=(SoundSource a, SoundSource b) => !(a == b);
}
/*
  a source will only exist to the alien if alien was there to hear it at first
  if a new source appears go towards it
*/