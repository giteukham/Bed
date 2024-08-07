using Cinemachine;
using System;
using System.Collections.Generic;

// Left State일 때 LeftLimit는 100, RightLimit는 140, RightTurn은 130 이상
// Middle State일 때 LeftLimit는 140, RightLimit는 235, LeftTurn은 150 이하, RightTurn은 225 이상
// Right State일 때 LeftLimit는 235, RightLimit는 260, LeftTurn은 245 이하

/// <summary>
///  화면을 좌우로 돌릴 때 돌릴 수 있는 한계 x-axis 값 및 돌리기 위한 각도
/// </summary>
struct HorizontalAxis
{
    public int leftLimitAngle;
    public int rightLimitAngle;
    public int leftTurnAngle;
    public int rightTurnAngle;
}

class LeftTurnState : TurnStateBase
{
    public LeftTurnState(CinemachinePOV povCamera, StateMachine playerTurnStateMachine, Dictionary<TurnStateTypes, IState> turnStates) : base(povCamera, playerTurnStateMachine, turnStates) { }
    private HorizontalAxis horizontalAxis = new HorizontalAxis
    {
        leftLimitAngle = 100,
        rightLimitAngle = 140
    };
    
    public override void Enter()
    {
        
    }
    
    /// <summary>
    /// HorizontalAxis > 130 이면 가운데로 돌림
    /// </summary>
    public override void Execute()
    {
        
    }
    
    public override void Exit()
    {
        
    }
}

class MiddleTurnState : TurnStateBase
{
    public MiddleTurnState(CinemachinePOV povCamera, StateMachine playerTurnStateMachine, Dictionary<TurnStateTypes, IState> turnStates) : base(povCamera, playerTurnStateMachine, turnStates) { }
    private HorizontalAxis horizontalAngle = new HorizontalAxis
    {
        leftLimitAngle = 140,
        rightLimitAngle = 235
    };
    
    public override void Enter()
    {
        povCamera.m_HorizontalAxis.m_MinValue = horizontalAngle.leftLimitAngle;
        povCamera.m_HorizontalAxis.m_MaxValue = horizontalAngle.rightLimitAngle;
        horizontalAngle.leftTurnAngle = horizontalAngle.leftLimitAngle + TURN_THRESHOLD;
        horizontalAngle.rightTurnAngle = horizontalAngle.rightLimitAngle - TURN_THRESHOLD;
    }
    
    /// <summary>
    /// 화면 좌우로 돌릴 때 상태를 변경하는 함수
    /// HorizontalAxis < 150 이면 왼쪽, HorizontalAxis > 225이면 오른쪽으로 돌림
    /// </summary>
    public override void Execute()
    {
        if (povCamera.m_HorizontalAxis.Value < horizontalAngle.leftTurnAngle)
        {
            playerTurnStateMachine.ChangeState(turnStates[TurnStateTypes.Left]);
        }
        else if (povCamera.m_HorizontalAxis.Value > horizontalAngle.rightTurnAngle)
        {
            playerTurnStateMachine.ChangeState(turnStates[TurnStateTypes.Right]);
        }
    }
    
    public override void Exit()
    {
        
    }
}
        
class RightTurnState : TurnStateBase
{
    public RightTurnState(CinemachinePOV povCamera, StateMachine playerTurnStateMachine, Dictionary<TurnStateTypes, IState> turnStates) : base(povCamera, playerTurnStateMachine, turnStates) { }

    public override void Enter()
    {
        
    }
    
    /// <summary>
    /// HorizontalAxis < 245 이면 가운데로 돌림
    /// </summary>
    public override void Execute()
    {
        
    }
    
    public override void Exit()
    {
        
    }
}