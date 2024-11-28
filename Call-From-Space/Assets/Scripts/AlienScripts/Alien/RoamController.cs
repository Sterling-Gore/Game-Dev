using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;
public class RoamController : RoomVisitor
{
    AlienController alien;

    [Header("Roamer")]
    public float timeToLookAroundFor = 0;
    public Vector3 nextRoamSpot;
    public int nodeIdx = 0;

    public Animator animator;

    LinkedList<State> prevStates = new();
    const int prevStatesCapacity = 5;
    public State curState;
    public States state;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Init(AlienController alien)
    {
        this.alien = alien;
        curState = new RoamingRoom(this, alien);
        base.Init();
    }
    public void RoamAround()
    {
        animator.SetBool("isWalking", true);
        curState.Update();
    }
    public void GoToNextState(States state)
    {
        this.state = state;
        prevStates.AddLast(curState);
        while (prevStates.Count > prevStatesCapacity)
            prevStates.RemoveFirst();
        switch (state)
        {
            case States.LookingAround:
                curState = new LookingAround(this, alien);
                break;
            case States.RoamingRoom:
                curState = new RoamingRoom(this, alien);
                break;
            case States.Rotating:
                curState = new Rotating(this, alien);
                break;
            case States.MovingToNextRoom:
                curState = new MovingToNextRoom(this, alien);
                break;
        }
    }
    public void GoToPreviousState()
    {
        curState = prevStates.Last();
        prevStates.RemoveLast();
        state = curState.state;
    }

    public override void Load(JObject state)
    {
        base.Load(state);
        curState = new RoamingRoom(this, alien);
    }

    public override void Save(ref JObject state)
    {
        base.Save(ref state);
    }
}
