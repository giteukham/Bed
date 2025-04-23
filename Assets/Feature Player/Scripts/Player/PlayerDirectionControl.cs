using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDirectionControl : IPlayerControl
{
    private StateMachine playerDirectionStateMachine;

    /// <summary>
    /// 플레이어가 바라보는 방향에 따라 상태 3가지.
    /// Switching는 몸 돌릴 때 사용하는 빈 상태.
    /// </summary>
    private Dictionary<PlayerDirectionStateTypes, IState> directionStates = new Dictionary<PlayerDirectionStateTypes, IState>()
    {
        { PlayerDirectionStateTypes.Left, new PlayerDirectionStates.LeftDirectionState() },
        { PlayerDirectionStateTypes.Middle, new PlayerDirectionStates.MiddleDirectionState() },
        { PlayerDirectionStateTypes.Right, new PlayerDirectionStates.RightDirectionState() },
        { PlayerDirectionStateTypes.Switching, new PlayerDirectionStates.SwitchingState() }
    };
    public Dictionary<PlayerDirectionStateTypes, IState> DirectionStates => directionStates;
    
    /// <summary>
    /// 11월 18일 최무령 수정
    /// </summary>
    /// <param name="playerDirectionStateMachine"></param>
    /// <param name="directionStates"></param>
    public PlayerDirectionControl(StateMachine playerDirectionStateMachine, Dictionary<PlayerDirectionStateTypes, IState> directionStates = null)
    {
        if (directionStates == null)
        {
            playerDirectionStateMachine.ChangeState(this.directionStates[PlayerDirectionStateTypes.Middle]);
        }
        else
        {
            playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Middle]);
        }
        this.playerDirectionStateMachine = playerDirectionStateMachine;
    }

    public void ChangeDirectionState(PlayerDirectionStateTypes stateType) 
    {
        if (playerDirectionStateMachine.IsCurrentState(directionStates[stateType])) return;
        
        switch (stateType)
        {
            case PlayerDirectionStateTypes.Left:
                if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Middle])) PlayerAnimation.PlayAnimation("Middle To Left");
                break;
            case PlayerDirectionStateTypes.Middle:
                if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Left])) PlayerAnimation.PlayAnimation("Middle From Left");
                else if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Right])) PlayerAnimation.PlayAnimation("Middle From Right");
                break;
            case PlayerDirectionStateTypes.Right:
                if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Middle])) PlayerAnimation.PlayAnimation("Middle To Right");
                break;
            case PlayerDirectionStateTypes.Switching:
                return;
        }
    }

    public void ChangeDirectionStateNoSound(PlayerDirectionStateTypes stateType) 
    {
        switch (stateType)
        {
            case PlayerDirectionStateTypes.Left:
                if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Middle])) PlayerAnimation.PlayAnimationNoSound("Middle To Left");
                break;
            case PlayerDirectionStateTypes.Middle:
                if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Left])) PlayerAnimation.PlayAnimationNoSound("Middle From Left");
                else if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Right])) PlayerAnimation.PlayAnimationNoSound("Middle From Right");
                break;
            case PlayerDirectionStateTypes.Right:
                if(playerDirectionStateMachine.IsCurrentState(directionStates[PlayerDirectionStateTypes.Middle])) PlayerAnimation.PlayAnimationNoSound("Middle To Right");
                break;
            case PlayerDirectionStateTypes.Switching:
                return;
        }
    }
}