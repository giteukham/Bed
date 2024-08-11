using Cinemachine;
using UnityEngine;

public enum PlayerDirectionStateTypes
{
    Left,
    Middle,
    Right,
    Switching
}

public class Player : MonoBehaviour
{
    // 갱신 단위 3초
    
    #region State Machine
    [Header("State Machine")]
    [SerializeField] private StateMachine playerDirectionStateMachine;
    #endregion
    
    #region Camera
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    private CinemachinePOV povCamera;
    #endregion
    
    [Header("Player Animation")]
    [SerializeField] private PlayerAnimation playerAnimation;

    private PlayerDirectionControl playerDirectionControl;
    
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
        playerDirectionControl = new PlayerDirectionControl(ref playerDirectionStateMachine, ref povCamera);
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