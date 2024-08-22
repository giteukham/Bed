using Bed.PostProcessing;
using Cysharp.Threading.Tasks;
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
    
    private int mouseCount = 0;
    private double[] mouseBlinkValues = { 0.001, 0.04, 0.08, 0.18, 1};
    
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
    
    private async void UpdateBlinkValue(int mouseScrollValue)
    {
        float elapsedTime = 0f;
        float durationTime = 0.1f;
        float currentValue = customVignette.blink.value;
        
        if (mouseScrollValue == -MOUSE_SCROLL_VALUE && customVignette.blink.value < BLINK_VALUE_MAX && mouseCount < mouseBlinkValues.Length) // ���콺 ���� �Ʒ��� ������ ��
        {
            mouseCount++;
            while (customVignette.blink.value < mouseBlinkValues[mouseCount] && elapsedTime < durationTime)
            {
                elapsedTime += Time.deltaTime;
                customVignette.blink.value = Mathf.Lerp(currentValue, (float)mouseBlinkValues[mouseCount], elapsedTime / durationTime);
                await UniTask.Yield();
            }
            await UniTask.WaitUntil(() => customVignette.blink.value >= mouseBlinkValues[mouseCount]);
        }
        
        if (mouseScrollValue == MOUSE_SCROLL_VALUE && customVignette.blink.value > BLINK_VALUE_MIN && mouseCount >= 0) // ���콺 ���� ���� �÷��� ��
        {
            mouseCount--;
            while (customVignette.blink.value > mouseBlinkValues[mouseCount])
            {
                elapsedTime += Time.deltaTime;
                customVignette.blink.value = Mathf.Lerp(currentValue, (float)mouseBlinkValues[mouseCount], elapsedTime / durationTime);
                await UniTask.Delay(50);
            }
            await UniTask.WaitUntil(() => customVignette.blink.value < mouseBlinkValues[mouseCount]);
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