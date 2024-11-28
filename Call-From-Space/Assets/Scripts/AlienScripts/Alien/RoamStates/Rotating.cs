using UnityEngine;

class Rotating : State
{
  public const States state = States.Rotating;
  public float rotatingTime = 0;
  public Rotating(RoamController roamer, AlienController alien) : base(roamer, alien)
  {
    Debug.Log("rotating around");
  }

  override public void Update()
  {
    if (rotatingTime > .9)
    {
      roamer.GoToPreviousState();
      rotatingTime = 0;
    }
    else
    {
      var target = roamer.nextRoamSpot;
      target.y = alien.transform.position.y;
      var targetRotation = Quaternion.LookRotation(target - alien.transform.position);
      alien.transform.rotation = Quaternion.Lerp(alien.transform.rotation, targetRotation, Time.deltaTime * alien.turnSpeed);
      rotatingTime += Time.deltaTime;
    }
  }
  public override void OnStuck() { }
}
