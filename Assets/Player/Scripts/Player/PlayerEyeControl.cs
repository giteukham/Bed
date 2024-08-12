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
    private int eyeLittleBlinkCount = 0;    // TODO: 눈 조금 감긴 횟수? 이름 좀 생각해 봐야 할 듯.
    
    public const int LITTLE_BLINK_COUNT_MIN = 0, LITTLE_BLINK_COUNT_MAX = 6;   // 눈 조금 감는 횟수
    public const int MOUSE_SCROLL_VALUE = 120;    // 마우스 휠 값
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
    /// 마우스 휠을 아래로 -> eyeLittleBlinkCount 증가
    /// 마우스 휠을 위로 -> eyeLittleBlinkCount 감소
    /// </summary>
    /// <param name="mouseScrollValue"></param>
    private void UpdateBlinkCount(int mouseScrollValue)
    {
        if (mouseScrollValue == -MOUSE_SCROLL_VALUE && eyeLittleBlinkCount < LITTLE_BLINK_COUNT_MAX)    // 마우스 휠을 아래로 내렸을 때
        {
            eyeLittleBlinkCount += Mathf.Abs(mouseScrollValue / MOUSE_SCROLL_VALUE);                    // eyeLittleBlinkCount 1씩 증가
        }
        else if (mouseScrollValue == MOUSE_SCROLL_VALUE && eyeLittleBlinkCount > LITTLE_BLINK_COUNT_MIN)    // 마우스 휠을 위로 올렸을 때
        {
            eyeLittleBlinkCount -= (mouseScrollValue / MOUSE_SCROLL_VALUE);                                // eyeLittleBlinkCount 1씩 감소
        }
    }

    /// <summary>
    /// eyeLittleBlinkCount가 LITTLE_BLINK_COUNT_MIN이면 완전히 눈 뜬 상태로 변경
    /// eyeLittleBlinkCount가 LITTLE_BLINK_COUNT_MAX이면 완전히 눈 감은 상태로 변경
    /// eyeLittleBlinkCount가 증가 -> 눈이 점점 감겨감
    /// eyeLittleBlinkCount가 감소 -> 눈이 점점 떠짐
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