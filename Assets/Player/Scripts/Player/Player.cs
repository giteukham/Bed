using Bed.PostProcessing;
using Cinemachine;
using Cinemachine.PostFX;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public enum PlayerDirectionStateTypes
{
    Left,
    Middle,
    Right,
    Switching
}

public enum PlayerEyeStateTypes
{
    Open,
    Opening,
    Close,
    Closing,
    Blink
}

[RequireComponent(typeof(PlayerAnimation))]
public class Player : MonoBehaviour
{
    [Header("State Machine")]
    [SerializeField] private StateMachine playerDirectionStateMachine;
    [SerializeField] private StateMachine playerEyeStateMachine;
    
    #region Player Components
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    
    private PlayerAnimation playerAnimation;
    
    [Header("Input System")]
    [SerializeField] private InputSystem inputSystem;
    
    [Header("Post Processing")]
    [SerializeField] private PostProcessProfile postProcessingProfile;
    private CustomVignette customVignette;
    #endregion
    
    #region Player Control Classes
    private PlayerDirectionControl playerDirectionControl;
    private PlayerEyeControl playerEyeControl;
    #endregion
    
    //TODO: 나중에 Game Manager로 옮겨야 함
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        TryGetComponent(out playerAnimation);
        
        customVignette = postProcessingProfile.GetSetting<CustomVignette>();
        playerDirectionControl = new PlayerDirectionControl(playerDirectionStateMachine);
        playerEyeControl = new PlayerEyeControl(playerEyeStateMachine, customVignette);
        playerEyeControl.SubscribeToEvents();
        
    }

    public void AnimationEvent_ChangeDirectionState(string toState)
    {
        switch (toState)
        {
            case "Left":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Left]);
                break;
            case "Middle":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Middle]);
                break;
            case "Right":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Right]);
                break;
            case "Switching":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Switching]);
                break;
        }
    }
    
    private void OnApplicationQuit()
    {
        customVignette.blink.value = 0.001f;
    }
}