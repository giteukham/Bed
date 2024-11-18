using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MouseSettingsPreviewPlayer : MonoBehaviour
{
    private Animator playerAnimator;
    
    [SerializeField] protected StateMachine playerDirectionStateMachine;
    private PreviewPlayerDirectionStates previewPlayerDirectionStates;
    private PlayerDirectionControl playerDirectionControl;

    [SerializeField] private CinemachineVirtualCamera playerVirtualCamera;
    public CinemachinePOV POVCamera => playerVirtualCamera?.GetCinemachineComponent<CinemachinePOV>();
    
    private Dictionary<PlayerDirectionStateTypes, IState> directionStates = new Dictionary<PlayerDirectionStateTypes, IState>()
    {
        { PlayerDirectionStateTypes.Left, new PreviewPlayerDirectionStates.LeftDirectionState() },
        { PlayerDirectionStateTypes.Middle, new PreviewPlayerDirectionStates.MiddleDirectionState() },
        { PlayerDirectionStateTypes.Right, new PreviewPlayerDirectionStates.RightDirectionState() },
        { PlayerDirectionStateTypes.Switching, new PreviewPlayerDirectionStates.SwitchingState() }
    };

    private void OnEnable()
    {
        TryGetComponent(out playerAnimator);
        previewPlayerDirectionStates = new PreviewPlayerDirectionStates(playerAnimator);
        playerDirectionControl = new PlayerDirectionControl(playerDirectionStateMachine, directionStates);
    }

    private void OnDisable()
    {
        playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Middle]);
        playerAnimator.SetTrigger("To Middle");
    }
    
    public void EnablePlayerObject(bool isActivate)
    {
        try
        {
            gameObject?.SetActive(isActivate);
        }
        catch (Exception e)
        {
        }
    }
    
    public void AnimationEvent_ChangeDirectionState(string toState)
    {
        switch (toState)
        {   
            case "Left":
                playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Left]);
                break;
            case "Middle":
                playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Middle]);
                break;
            case "Right":
                playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Right]);
                break;
            case "Switching":
                playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Switching]);
                break;
        }
    }
}

public class PreviewPlayerDirectionStates
{
    private static Animator playerAnimator;
    
    public PreviewPlayerDirectionStates(Animator playerAnimator)
    {
        PreviewPlayerDirectionStates.playerAnimator = playerAnimator;
    }
    
    public class LeftDirectionState : IState
    {
        public void Enter()
        {

        }
        
        public void Execute()
        {
            if (InputSystem.MouseDeltaHorizontal >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                playerAnimator.SetTrigger("Middle From Left");
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
            if (InputSystem.MouseDeltaHorizontal <= PlayerDirectionControl.TURN_LEFT_DELTA_POWER)
            {
                playerAnimator.SetTrigger("Middle To Left");
            }
            else if (InputSystem.MouseDeltaHorizontal >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                playerAnimator.SetTrigger("Middle To Right");
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
            if (InputSystem.MouseDeltaHorizontal <= PlayerDirectionControl.TURN_LEFT_DELTA_POWER)
            {
                playerAnimator.SetTrigger("Middle From Right");
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

