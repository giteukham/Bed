using Cinemachine;

public class PlayerDirectionStates
{
    public class LeftDirectionState : IState
    {
        public void Enter()
        {

        }
        
        public void Execute()
        {
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

