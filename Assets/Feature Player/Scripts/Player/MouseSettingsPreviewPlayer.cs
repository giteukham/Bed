using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MouseSettingsPreviewPlayer : MonoBehaviour
{
    private Animator playerAnimator, blanketAnimator;
    
    [SerializeField] private Transform blanket;
    private Vector3 blanketPosition;
    
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
    
    private Vector3[] playerOriginTransform = new []
    {
        new Vector3(1.8f, 2.5f, 5.1f),                      // origin position
        new Vector3(2.25f, -180, 0),                        // origin rotation
        new Vector3(0.5f, 0.5f, 0.5f)                       // origin scale
    };

    private void OnEnable()
    {
        TryGetComponent(out playerAnimator);
        blanketAnimator = blanket.GetComponent<Animator>();
        blanketPosition = blanket.transform.position;
        previewPlayerDirectionStates = new PreviewPlayerDirectionStates(playerAnimator, blanketAnimator);
        playerDirectionControl = new PlayerDirectionControl(playerDirectionStateMachine, directionStates);
        playerDirectionStateMachine.ChangeState(directionStates[PlayerDirectionStateTypes.Middle]);
    }

    private void OnDisable()
    {
        gameObject.transform.position = playerOriginTransform[0];
        gameObject.transform.eulerAngles = playerOriginTransform[1];
        gameObject.transform.localScale = playerOriginTransform[2];
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
    private static Animator playerAnimator, blanketAnimator;
    
    public PreviewPlayerDirectionStates(Animator playerAnimator, Animator blanketAnimator)
    {
        PreviewPlayerDirectionStates.playerAnimator = playerAnimator;
        PreviewPlayerDirectionStates.blanketAnimator = blanketAnimator;
    }
    
    public class LeftDirectionState : IState
    {
        public void Enter()
        {

        }
        
        public void Execute()
        {
            if (InputSystem.MouseDeltaX >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                playerAnimator.SetTrigger("Middle From Left");
                blanketAnimator.SetTrigger("Middle From Left");
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
                playerAnimator.SetTrigger("Middle To Left");
                blanketAnimator.SetTrigger("Middle To Left");
            }
            else if (InputSystem.MouseDeltaX >= PlayerDirectionControl.TURN_RIGHT_DELTA_POWER)
            {
                playerAnimator.SetTrigger("Middle To Right");
                blanketAnimator.SetTrigger("Middle To Right");
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
                playerAnimator.SetTrigger("Middle From Right");
                blanketAnimator.SetTrigger("Middle From Right");
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

