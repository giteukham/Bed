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
    public static Dictionary<PlayerEyeStateTypes, IState> EyeStates => eyeStates;
    
    private int prevEyeLittleBlinkCount = 0;
    private int eyeLittleBlinkCount = 0;    // TODO: �� ���� ���� Ƚ��? �̸� �� ������ ���� �� ��.
    
    public const int LITTLE_BLINK_COUNT_MIN = 0, LITTLE_BLINK_COUNT_MAX = 6;   // �� ���� ���� Ƚ��
    public const int MOUSE_SCROLL_VALUE = 120;    // ���콺 �� ��
    public const float EYE_POSITION_MAX_Y = 1080f;
    
    private StateMachine playerEyeStateMachine;
    private PlayerEyeStates playerEyeStates;
    
    public PlayerEyeControl(StateMachine playerEyeStateMachine, RectTransform topEyelid, RectTransform bottomEyelid)
    {
        this.playerEyeStateMachine = playerEyeStateMachine;
        playerEyeStates = new PlayerEyeStates(this, topEyelid, bottomEyelid);
        
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Open]);
    }
    
    public void SubscribeToEvents()
    {
        InputSystem.OnMouseScrollEvent += OnLittleBlink;
        InputSystem.OnMouseWheelClickEvent += OnBlink;
    }

    private void OnLittleBlink(int mouseScrollValue)
    {
        UpdateBlinkCount(mouseScrollValue);
        UpdateEyeState();
    }
    
    private void OnBlink()
    {
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Blink]);
    }

    /// <summary>
    /// ���콺 ���� �Ʒ��� -> eyeLittleBlinkCount ����
    /// ���콺 ���� ���� -> eyeLittleBlinkCount ����
    /// </summary>
    /// <param name="mouseScrollValue"></param>
    private void UpdateBlinkCount(int mouseScrollValue)
    {
        if (mouseScrollValue == -MOUSE_SCROLL_VALUE && eyeLittleBlinkCount < LITTLE_BLINK_COUNT_MAX)    // ���콺 ���� �Ʒ��� ������ ��
        {
            eyeLittleBlinkCount += Mathf.Abs(mouseScrollValue / MOUSE_SCROLL_VALUE);                    // eyeLittleBlinkCount 1�� ����
        }
        else if (mouseScrollValue == MOUSE_SCROLL_VALUE && eyeLittleBlinkCount > LITTLE_BLINK_COUNT_MIN)    // ���콺 ���� ���� �÷��� ��
        {
            eyeLittleBlinkCount -= (mouseScrollValue / MOUSE_SCROLL_VALUE);                                // eyeLittleBlinkCount 1�� ����
        }
    }

    /// <summary>
    /// eyeLittleBlinkCount�� LITTLE_BLINK_COUNT_MIN�̸� ������ �� �� ���·� ����
    /// eyeLittleBlinkCount�� LITTLE_BLINK_COUNT_MAX�̸� ������ �� ���� ���·� ����
    /// eyeLittleBlinkCount�� ���� -> ���� ���� ���ܰ�
    /// eyeLittleBlinkCount�� ���� -> ���� ���� ����
    /// </summary>
    private void UpdateEyeState()
    {
        if (eyeLittleBlinkCount == LITTLE_BLINK_COUNT_MIN)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Open]);
        }
        else if (eyeLittleBlinkCount == LITTLE_BLINK_COUNT_MAX)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Close]);
        }
        else if (prevEyeLittleBlinkCount < eyeLittleBlinkCount)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Closing], true);
        }
        else if (prevEyeLittleBlinkCount > eyeLittleBlinkCount)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Opening], true);
        }
        prevEyeLittleBlinkCount = eyeLittleBlinkCount;
    }

    public float GetChangedEyePosition()
    {
        return EYE_POSITION_MAX_Y - (EYE_POSITION_MAX_Y / LITTLE_BLINK_COUNT_MAX * eyeLittleBlinkCount);
    }
    
    public void UnsubscribeToEvents()
    {
        InputSystem.OnMouseScrollEvent -= OnLittleBlink;
    }
}