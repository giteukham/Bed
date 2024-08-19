using Bed.PostProcessing;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeControl : IPlayerControl
{
    private static Dictionary<PlayerEyeStateTypes, IState> eyeStates = new Dictionary<PlayerEyeStateTypes, IState>()
    {
        { PlayerEyeStateTypes.Open, new PlayerEyeStates.OpenEyeState() },
        { PlayerEyeStateTypes.Opening, new PlayerEyeStates.OpeningEyeState() },
        { PlayerEyeStateTypes.Close, new PlayerEyeStates.CloseEyeState() },
        { PlayerEyeStateTypes.Closing, new PlayerEyeStates.ClosingEyeState() },
        { PlayerEyeStateTypes.Blink, new PlayerEyeStates.BlinkEyeState() }
    };
    
    public const float BLINK_VALUE_MIN = 0.001f, BLINK_VALUE_MAX = 1f;   // Vignette�� Blink �� �ּڰ�, �ִ�.
                                                                         // �ּڰ��� 0���� �ϸ� ������ �Ұ���.
    public const int MOUSE_SCROLL_VALUE = 120;    // ���콺 �� ��
    
    private StateMachine playerEyeStateMachine;
    private PlayerEyeStates playerEyeStates;
    private CustomVignette customVignette;

    private float prevBlinkValue = 0f;
    
    public PlayerEyeControl(StateMachine playerEyeStateMachine, CustomVignette customVignette)
    {
        this.playerEyeStateMachine = playerEyeStateMachine;
        this.customVignette = customVignette;
        playerEyeStates = new PlayerEyeStates(this, customVignette);
        
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Open]);
    }
    
    public void SubscribeToEvents()
    {
        InputSystem.OnMouseScrollEvent += OnLittleBlink;
        InputSystem.OnMouseWheelClickEvent += OnBlink;
    }

    private void OnLittleBlink(int mouseScrollValue)
    {
        UpdateBlinkValue(mouseScrollValue);
        UpdateEyeState();
    }
    
    private void OnBlink()
    {
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Blink]);
    }
    
    private void UpdateBlinkValue(int mouseScrollValue)
    {
        if (mouseScrollValue == -MOUSE_SCROLL_VALUE && customVignette.blink.value < BLINK_VALUE_MAX)    // ���콺 ���� �Ʒ��� ������ ��
        {
            customVignette.blink.value += PlayerConstant.eyeOpenCloseInterval;
        }
        else if (mouseScrollValue == MOUSE_SCROLL_VALUE && customVignette.blink.value > BLINK_VALUE_MIN)    // ���콺 ���� ���� �÷��� ��
        {
            customVignette.blink.value -= PlayerConstant.eyeOpenCloseInterval;
        }
    }
    
    private void UpdateEyeState()
    {
        if (customVignette.blink.value <= BLINK_VALUE_MIN)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Open]);
        }
        else if (customVignette.blink.value >= BLINK_VALUE_MAX)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Close]);
        }
        else if (prevBlinkValue < customVignette.blink.value)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Closing], true);
        }
        else if (prevBlinkValue > customVignette.blink.value)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Opening], true);
        }
        prevBlinkValue = customVignette.blink.value;
    }
    
    public void ChangeEyeState(PlayerEyeStateTypes stateType) => playerEyeStateMachine.ChangeState(eyeStates[stateType]);
    
    public void UnsubscribeToEvents() => InputSystem.OnMouseScrollEvent -= OnLittleBlink;
    
}