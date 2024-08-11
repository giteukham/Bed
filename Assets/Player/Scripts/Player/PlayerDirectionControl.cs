using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDirectionControl
{
    private static Dictionary<PlayerDirectionStateTypes, IState> directionStates = new Dictionary<PlayerDirectionStateTypes, IState>();
    public static Dictionary<PlayerDirectionStateTypes, IState> DirectionStates => directionStates;
    
    public const float TURN_RIGHT_DELTA_POWER = 3f, TURN_LEFT_DELTA_POWER = -3f;
    public const float LEFT_LIMIT_ANGLE = 140f, RIGHT_LIMIT_ANGLE = 235f;
    
    public PlayerDirectionControl(ref StateMachine playerDirectionStateMachine, ref CinemachinePOV povCamera)
    {
        InitializeState();
        povCamera.m_HorizontalAxis.m_MinValue = LEFT_LIMIT_ANGLE;
        povCamera.m_HorizontalAxis.m_MaxValue = RIGHT_LIMIT_ANGLE;
        playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Middle]);
    }

    /// <summary>
    /// 플레이어가 바라보는 방향에 따라 상태 3가지.
    /// Switching는 몸 돌릴 때 사용하는 빈 상태.
    /// </summary>
    private void InitializeState()
    {
        try
        {
            directionStates[PlayerDirectionStateTypes.Left] = new PlayerDirectionStates.LeftDirectionState();
            directionStates[PlayerDirectionStateTypes.Middle] = new PlayerDirectionStates.MiddleDirectionState();
            directionStates[PlayerDirectionStateTypes.Right] = new PlayerDirectionStates.RightDirectionState();
            directionStates[PlayerDirectionStateTypes.Switching] = new PlayerDirectionStates.SwitchingState();
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"PlayerTurnStateMachine가 비어 있습니다. {e.Message}; {e.StackTrace}");
        }
        
    }
}