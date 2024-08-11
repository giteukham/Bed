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
    // ���� ���� 3��
    
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
    
    // TODO: ���߿� Cursor ������ GameManager�� �Űܾ� ��.
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
    /// �ִϸ��̼� �̺�Ʈ���� ���� State ���� �Լ�.
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