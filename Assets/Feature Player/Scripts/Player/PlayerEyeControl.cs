using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Bed;
using UnityEngine;
using UnityEngine.AI;
using Bed.Collider;

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
    
    public const float BLINK_VALUE_MIN = 0f, BLINK_VALUE_MAX = 1f;   // Vignette의 Blink 값 최솟값, 최댓값.
    public const int COLIDER_VALUE_MIN = 3, COLIDER_VALUE_MAX = 0;    // Cone Collider의 최대 값
    public const int MOUSE_SCROLL_VALUE = 120;    // 마우스 휠 값
    
    private StateMachine playerEyeStateMachine;
    private PlayerEyeStates playerEyeStates;

    private float prevBlinkValue = 0f;
    float elapsedTime = 0f;
    float durationTime = 0.12f;
    float lastScrollTime = 0;
    float moveValue = 0;
    float? currentValue = null;
    float? targetValue = BLINK_VALUE_MIN;

    public PlayerEyeControl(StateMachine playerEyeStateMachine)
    {
        this.playerEyeStateMachine = playerEyeStateMachine;
        playerEyeStates = new PlayerEyeStates(this);
        
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Open]);
    }
    
    public void SubscribeToEvents()
    {
        InputSystem.Instance.OnMouseScrollEvent += OnEyelidMove;
        InputSystem.Instance.OnMouseWheelClickEvent += OnBlink;
    }
    
    private void OnBlink()
    {
        if (playerEyeStateMachine.IsCurrentState(eyeStates[PlayerEyeStateTypes.Close]) 
         || playerEyeStateMachine.IsCurrentState(eyeStates[PlayerEyeStateTypes.Blink])) return;
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Blink]);
    }

    private async void OnEyelidMove(int mouseScrollValue)
    {
        if (playerEyeStateMachine.IsCurrentState(eyeStates[PlayerEyeStateTypes.Blink])) return;

        if (currentValue == null) currentValue = BlinkEffect.Blink;

        if (mouseScrollValue != 0) 
        {
            float currentScrollTime = Time.time;
            moveValue = 0.22f - (currentScrollTime - lastScrollTime);
            if ( moveValue > 0.2f) moveValue = 0.2f;
            if ( moveValue < 0.01f) moveValue = 0.01f;

            lastScrollTime = currentScrollTime;
        }

        if (mouseScrollValue < 0) // 휠 다운
        {
            targetValue += moveValue;
            if (targetValue > 1) targetValue = 1; 

            if (elapsedTime > 0.07f) elapsedTime = 0.07f;
            
            while (currentValue < targetValue && elapsedTime < durationTime)
            {
                elapsedTime += Time.deltaTime;
                BlinkEffect.Blink = Mathf.Lerp((float)currentValue, (float)targetValue, elapsedTime / durationTime);
                await UniTask.Yield();
            }
            UpdateEyeState();
            elapsedTime = 0f;
            currentValue = null;
        }

        if (mouseScrollValue > 0) // 휠 업
        {
            targetValue -= moveValue;
            if (targetValue < 0) targetValue = 0;
            if (elapsedTime > 0.07f) elapsedTime = 0.07f;

            while (currentValue > targetValue && elapsedTime < durationTime)
            {   
                elapsedTime += Time.deltaTime;
                BlinkEffect.Blink = Mathf.Lerp((float)currentValue, (float)targetValue, elapsedTime / durationTime);
                await UniTask.Yield();
            }
            UpdateEyeState();
            elapsedTime = 0f;
            currentValue = null;
        }
    }

    public void UpdateEyeState()
    {
        if (BlinkEffect.Blink <= BLINK_VALUE_MIN)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Open]);
        }
        else if (BlinkEffect.Blink >= BLINK_VALUE_MAX)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Close]);
        }
        else if (prevBlinkValue < BlinkEffect.Blink)
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Closing], true);
        }
        else if (prevBlinkValue > BlinkEffect.Blink || prevBlinkValue == BlinkEffect.Blink) // 같은 위치에서 계속 깜빡(휠 클릭)이면 Opening 상태가 되게
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Opening], true);
        }
        //Debug.Log("현재 상태 : " + playerEyeStateMachine.ToString());
        prevBlinkValue = BlinkEffect.Blink;
    }
    
    public void ChangeEyeState(PlayerEyeStateTypes stateType) => playerEyeStateMachine.ChangeState(eyeStates[stateType]);
    
    public void UnsubscribeToEvents() => InputSystem.Instance.OnMouseScrollEvent -= OnEyelidMove;
    
}