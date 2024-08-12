using Cinemachine;
using UnityEngine;

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

public class Player : MonoBehaviour
{
    // 갱신 단위 3초
    
    [Header("State Machine")]
    [SerializeField] private StateMachine playerDirectionStateMachine;
    [SerializeField] private StateMachine playerEyeStateMachine;
    
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    private CinemachinePOV povCamera;
    
    [Header("Player Animation")]
    [SerializeField] private PlayerAnimation playerAnimation;
    
    [Header("Input System")]
    [SerializeField] private InputSystem inputSystem;

    [Header("Player Eyelid UI")]
    [SerializeField] private RectTransform topEyelid; 
    [SerializeField] private RectTransform bottomEyelid;
    
    private PlayerDirectionControl playerDirectionControl;
    private PlayerEyeControl playerEyeControl;
    
    private float stressGauge = 0f, fearGauge = 0f;
    private float stressGaugeMax = 100f, fearGaugeMax = 100f;
    
    // TODO: 나중에 Cursor 조정을 GameManager로 옮겨야 함.
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        povCamera = playerCamera.GetCinemachineComponent<CinemachinePOV>();
        
        playerDirectionControl = new PlayerDirectionControl(playerDirectionStateMachine, ref povCamera);
        playerEyeControl = new PlayerEyeControl(playerEyeStateMachine, topEyelid, bottomEyelid);
        playerEyeControl.SubscribeToEvents();
    }

    /// <summary>
    /// 애니메이션 이벤트에서 쓰는 State 변경 함수.
    /// </summary>
    /// <param name="toState"></param>
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
    
}