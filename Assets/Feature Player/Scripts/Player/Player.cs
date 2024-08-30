using Bed.PostProcessing;
using Cinemachine;
using Cinemachine.PostFX;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.PostProcessing;
using Bed.Collider;

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
    [Header("Player Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    
    private PlayerAnimation playerAnimation;
    
    [Header("Input System")]
    [SerializeField] private InputSystem inputSystem;
    
    [Header("Post Processing")]
    [SerializeField] private PostProcessProfile postProcessingProfile;
    private CustomVignette customVignette;
    [Header("Cone Colider")]
    [SerializeField] private ConeCollider coneCollider;
    
    #endregion
    
    #region Player Control Classes
    private PlayerDirectionControl playerDirectionControl;
    private PlayerEyeControl playerEyeControl;
    #endregion

    #region Main Camera
    [Header("Main Camera")]
    [SerializeField] private Camera mainCamera;
    #endregion

    #region Player Stats Updtae Variables
    private float updateInterval = 0.1f; // ?��?��?��?�� 주기
    private float timeSinceLastUpdate = 0f;

    private float currentHorizontalHeadMovement;
    private float currentVerticalHeadMovement;
    private float recentHorizontalHeadMovement;
    private float recentVerticalHeadMovement;
    private float headMovementDelta;
    #endregion


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

    void Update() 
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateStats();
            timeSinceLastUpdate = 0f;
        }

        coneCollider.SetColider(customVignette.blink.value);
    }

    private void UpdateStats()
    {
        // ----------------- Eye State -----------------
        float cameraDeltaX = playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        float cameraDeltaY = playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
        recentHorizontalHeadMovement = currentHorizontalHeadMovement;
        recentVerticalHeadMovement = currentVerticalHeadMovement;
        currentHorizontalHeadMovement = cameraDeltaX;
        currentVerticalHeadMovement = cameraDeltaY;
        if(currentVerticalHeadMovement == recentVerticalHeadMovement && currentHorizontalHeadMovement == recentHorizontalHeadMovement) headMovementDelta = 0;
        else headMovementDelta = Mathf.Abs(currentHorizontalHeadMovement - recentHorizontalHeadMovement) + Mathf.Abs(currentVerticalHeadMovement - recentVerticalHeadMovement);

        float mouseDeltaX = Mathf.Abs(InputSystem.MouseDeltaX);
        float mouseDeltaY = Mathf.Abs(InputSystem.MouseDeltaY);
        
        PlayerConstant.HeadMovementCAT += (mouseDeltaX + mouseDeltaY) * headMovementDelta;
        PlayerConstant.HeadMovementLAT += (mouseDeltaX + mouseDeltaY) * headMovementDelta;
        // ----------------- Head Movement -----------------

        // ----------------- Look Value -----------------
        float eulerY = mainCamera.transform.eulerAngles.y;
        float eulerX = mainCamera.transform.eulerAngles.x;

        if (eulerY < 105f) 
        {
            PlayerConstant.LeftLookCAT += timeSinceLastUpdate;
            PlayerConstant.LeftLookLAT += timeSinceLastUpdate;
        }
        else if (eulerY < 175f)
        {
            PlayerConstant.LeftFrontLookCAT += timeSinceLastUpdate;
            PlayerConstant.LeftFrontLookLAT += timeSinceLastUpdate;
        }
        else if (eulerY <= 185f)
        {
            PlayerConstant.FrontLookCAT += timeSinceLastUpdate;
            PlayerConstant.FrontLookLAT += timeSinceLastUpdate;
        }
        else if (eulerY <= 250f)
        {
            PlayerConstant.RightFrontLookCAT += timeSinceLastUpdate;
            PlayerConstant.RightFrontLookLAT += timeSinceLastUpdate;
        }
        else
        {
            PlayerConstant.RightLookCAT += timeSinceLastUpdate;
            PlayerConstant.RightLookLAT += timeSinceLastUpdate;
        }

        if (eulerX > 330f)
        {
            PlayerConstant.UpLookCAT += timeSinceLastUpdate;
            PlayerConstant.UpLookLAT += timeSinceLastUpdate;
        }
        else
        {
            PlayerConstant.DownLookCAT += timeSinceLastUpdate;
            PlayerConstant.DownLookLAT += timeSinceLastUpdate;
        }
        // ----------------- Look Value -----------------
    }

    private void OnApplicationQuit()
    {
        customVignette.blink.value = 0.001f;
    }
}