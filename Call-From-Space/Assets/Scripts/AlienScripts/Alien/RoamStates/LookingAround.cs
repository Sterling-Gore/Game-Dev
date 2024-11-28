using UnityEngine;

class LookingAround : State
{
  public const States state = States.LookingAround;
  public float timeLookingAround = 0;
  public LookingAround(RoamController roamer, AlienController alien) : base(roamer, alien)
  {
    Debug.Log("looking around");
  }

  override public void Update()
  {
    roamer.animator.SetBool("isWalking", false);
    roamer.animator.SetBool("isLookingAround", true);
    if (timeLookingAround > roamer.timeToLookAroundFor)
    {
      roamer.GoToPreviousState();
      roamer.animator.SetBool("isLookingAround", false);
      alien.PlayRandomIdleAudio();
    }
    else
    {
      int angle = 200;
      if (timeLookingAround < roamer.timeToLookAroundFor / 4 || timeLookingAround > roamer.timeToLookAroundFor * 3 / 4)
        angle = -200;

      alien.head.Rotate(Vector3.forward, angle * Time.deltaTime / roamer.timeToLookAroundFor);
      timeLookingAround += Time.deltaTime;
    }
  }

  public override void OnStuck() { }
}
