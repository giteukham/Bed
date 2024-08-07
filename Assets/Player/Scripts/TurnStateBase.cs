using Cinemachine;
using System.Collections.Generic;

abstract class TurnStateBase : IState
{
    protected CinemachinePOV povCamera;
    protected StateMachine playerTurnStateMachine;
    protected Dictionary<TurnStateTypes, IState> turnStates;
    
    protected const int TURN_THRESHOLD = 10;
    
    protected TurnStateBase(CinemachinePOV povCamera, StateMachine playerTurnStateMachine, Dictionary<TurnStateTypes, IState> turnStates)
    {
        this.povCamera = povCamera;
        this.playerTurnStateMachine = playerTurnStateMachine;
        this.turnStates = turnStates;
    }
    
    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
}