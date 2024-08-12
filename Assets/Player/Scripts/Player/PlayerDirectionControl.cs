using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerDirectionControl : IPlayerControl
{
    /// <summary>
    /// 플레이어가 바라보는 방향에 따라 상태 3가지.
    /// Switching는 몸 돌릴 때 사용하는 빈 상태.
    /// </summary>
    private static Dictionary<PlayerDirectionStateTypes, IState> directionStates = new Dictionary<PlayerDirectionStateTypes, IState>()
    {
        { PlayerDirectionStateTypes.Left, new PlayerDirectionStates.LeftDirectionState() },
        { PlayerDirectionStateTypes.Middle, new PlayerDirectionStates.MiddleDirectionState() },
        { PlayerDirectionStateTypes.Right, new PlayerDirectionStates.RightDirectionState() },
        { PlayerDirectionStateTypes.Switching, new PlayerDirectionStates.SwitchingState() }
    };
    public static Dictionary<PlayerDirectionStateTypes, IState> DirectionStates => directionStates;
    
    public const float TURN_RIGHT_DELTA_POWER = 50f, TURN_LEFT_DELTA_POWER = -50f;  // 마우스 Delta X값이 50이상이면 Right State로, -50이하이면 Left State로 변경
    public const float LEFT_LIMIT_ANGLE = 140f, RIGHT_LIMIT_ANGLE = 235f;           // 왼쪽으로 돌릴 때 최대 각도, 오른쪽으로 돌릴 때 최대 각도
    
    private StateMachine playerDirectionStateMachine;
    
    public PlayerDirectionControl(StateMachine playerDirectionStateMachine, ref CinemachinePOV povCamera)
    {
        povCamera.m_HorizontalAxis.m_MinValue = LEFT_LIMIT_ANGLE;
        povCamera.m_HorizontalAxis.m_MaxValue = RIGHT_LIMIT_ANGLE;
        playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Middle]);
    }

}