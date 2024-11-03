using Bed;
using Cinemachine;
using UnityEngine;

public class PlayerDirectionStates
{
    public class LeftDirectionState : IState
    {
        public void Enter()
        {
            PlayerConstant.isLeftState = true;
            PlayerConstant.isMiddleState = false;
            PlayerConstant.isRightState = false;
            PlayerConstant.isMovingState = false;
        }
        
        public void Execute()
        {
            PlayerConstant.LeftStateCAT += Time.deltaTime;
            PlayerConstant.LeftStateLAT += Time.deltaTime;

            if ( PlayerConstant.isParalysis ) return;
            if (InputSystem.Instance.MouseDeltaX >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle From Left");
            }
            
        }
        
        public void Exit()
        {
            PlayerConstant.isLeftState = false;
        }
    }

    public class MiddleDirectionState : IState
    {
        public void Enter()
        {
            PlayerConstant.isLeftState = false;
            PlayerConstant.isMiddleState = true;
            PlayerConstant.isRightState = false;
            PlayerConstant.isMovingState = false;
        }
        
        public void Execute()
        {
            PlayerConstant.MiddleStateCAT += Time.deltaTime;
            PlayerConstant.MiddleStateLAT += Time.deltaTime;

            if ( PlayerConstant.isParalysis ) return;
            if (InputSystem.Instance.MouseDeltaX <= PlayerDirectionControl.TURN_LEFT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle To Left");
            }
            else if (InputSystem.Instance.MouseDeltaX >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle To Right");
            }
            
        }
        
        public void Exit()
        {
            PlayerConstant.isMiddleState = false;
        }
    }
        
    public class RightDirectionState : IState
    {
        public void Enter()
        {
            PlayerConstant.isLeftState = false;
            PlayerConstant.isMiddleState = false;
            PlayerConstant.isRightState = true;
            PlayerConstant.isMovingState = false;
        }
        public void Execute()
        {
            PlayerConstant.RightStateCAT += Time.deltaTime;
            PlayerConstant.RightStateLAT += Time.deltaTime;

            if ( PlayerConstant.isParalysis ) return;
            if (InputSystem.Instance.MouseDeltaX <= PlayerDirectionControl.TURN_LEFT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle From Right");
            }
            
        }
        
        public void Exit()
        {
            PlayerConstant.isRightState = false;
        }
    }

    public class SwitchingState : IState
    {
        public void Enter()
        {
            PlayerConstant.isLeftState = false;
            PlayerConstant.isMiddleState = false;
            PlayerConstant.isRightState = false;
            PlayerConstant.isMovingState = true;
        }

        public void Execute()
        {

        }

        public void Exit()
        {
            PlayerConstant.isMovingState = false;
        }
    }
}

