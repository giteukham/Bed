using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public enum TurnStateTypes
{
    Left,
    Middle,
    Right
}

public class PlayerTurnControl : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    private CinemachinePOV povCamera;

    private Dictionary<TurnStateTypes, IState> turnStates = new Dictionary<TurnStateTypes, IState>();
    private StateMachine playerTurnStateMachine;

    // TODO: 나중에 Cursor 조정을 GameManager로 옮겨야 함.
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Start()
    {
        playerTurnStateMachine = PlayerManager.Instance.playerTurnStateMachine ?? null;
        povCamera = playerCamera.GetCinemachineComponent<CinemachinePOV>();
        InitializeState();
    }

    private void InitializeState()
    {
        try
        {
            turnStates[TurnStateTypes.Left] = new LeftTurnState(povCamera: povCamera, playerTurnStateMachine: playerTurnStateMachine, turnStates: turnStates);
            turnStates[TurnStateTypes.Middle] = new MiddleTurnState(povCamera: povCamera, playerTurnStateMachine: playerTurnStateMachine, turnStates: turnStates);
            turnStates[TurnStateTypes.Right] = new RightTurnState(povCamera: povCamera, playerTurnStateMachine: playerTurnStateMachine, turnStates: turnStates);
        
            playerTurnStateMachine.ChangeState(turnStates[TurnStateTypes.Middle]);
        }
        catch (NullReferenceException e)
        {
            Debug.LogError($"PlayerTurnStateMachine가 비어 있습니다. {e.Message}; {e.StackTrace}");
        }
    }

    private void Update()
    {
        Debug.Log(playerTurnStateMachine);
    }

}