public enum States
{
    LookingAround, RoamingRoom, Rotating, MovingToNextRoom
}
public abstract class State
{
    public States state;
    public RoamController roamer;
    public AlienController alien;
    /// <summary>
    /// don't call roamer.GoToNextState in constructor
    /// </summary>
    /// <param name="roamer"></param>
    /// <param name="alien"></param>
    public State(RoamController roamer, AlienController alien)
    {
        this.roamer = roamer;
        this.alien = alien;
    }
    abstract public void Update();

    abstract public void OnStuck();
}