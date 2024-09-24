using Cinemachine;
using UnityEngine;

public class PlayerDirectionStates
{
    public class LeftDirectionState : IState
    {
        public void Enter()
        {

        }
        
        public void Execute()
        {
            PlayerConstant.LeftStateCAT += Time.deltaTime;
            PlayerConstant.LeftStateLAT += Time.deltaTime;

            if ( PlayerConstant.isParalysis ) return;
            if (InputSystem.MouseDeltaX >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle From Left");
            }
            
        }
        
        public void Exit()
        {
            
        }
    }

    public class MiddleDirectionState : IState
    {
        public void Enter()
        {
        }
        
        public void Execute()
        {
            PlayerConstant.MiddleStateCAT += Time.deltaTime;
            PlayerConstant.MiddleStateLAT += Time.deltaTime;

            if ( PlayerConstant.isParalysis ) return;
            if (InputSystem.MouseDeltaX <= PlayerDirectionControl.TURN_LEFT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle To Left");
            }
            else if (InputSystem.MouseDeltaX >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle To Right");
            }
            
        }
        
        public void Exit()
        {
            
        }
    }
        
    public class RightDirectionState : IState
    {
        public void Enter()
        {
        }
        public void Execute()
        {
            PlayerConstant.RightStateCAT += Time.deltaTime;
            PlayerConstant.RightStateLAT += Time.deltaTime;

            if ( PlayerConstant.isParalysis ) return;
            if (InputSystem.MouseDeltaX <= PlayerDirectionControl.TURN_LEFT_DELTA_POWER)
            {
                PlayerAnimation.PlayAnimation("Middle From Right");
            }
            
        }
        
        public void Exit()
        {
            
        }
    }

    public class SwitchingState : IState
    {
        public void Enter()
        {
            
        }

        public void Execute()
        {
            
        }

        public void Exit()
        {
            
        }
    }
}

