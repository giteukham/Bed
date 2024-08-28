using Bed.PostProcessing;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
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
    
    public const float BLINK_VALUE_MIN = 0.001f, BLINK_VALUE_MAX = 1f;   // Vignette�� Blink �� �ּڰ�, �ִ�.
                                                                         // �ּڰ��� 0���� �ϸ� ������ �Ұ���.
    public const int COLIDER_VALUE_MIN = 3, COLIDER_VALUE_MAX = 0;    // Cone Collider�� �ִ� ��
    public const int MOUSE_SCROLL_VALUE = 120;    // ���콺 �� ��
    
    private StateMachine playerEyeStateMachine;
    private PlayerEyeStates playerEyeStates;
    private CustomVignette customVignette;

    private float prevBlinkValue = 0f;

    public int mouseCount = 0;
    public float[] mouseBlinkValues = new float[] { BLINK_VALUE_MIN, 0.04f, 0.08f, 0.18f,  BLINK_VALUE_MAX};
    private CancellationTokenSource blinkCancellationTokenSource;
    float elapsedTime = 0f;
    float durationTime = 0.18f;
    float currentValue;

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
    
    private void OnBlink()
    {
        if (playerEyeStateMachine.IsCurrentState(eyeStates[PlayerEyeStateTypes.Close]) 
         || playerEyeStateMachine.IsCurrentState(eyeStates[PlayerEyeStateTypes.Blink])) return;
        playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Blink]);
    }

    private async void OnLittleBlink(int mouseScrollValue)
    {
        if (playerEyeStateMachine.IsCurrentState(eyeStates[PlayerEyeStateTypes.Blink])) return;

        currentValue = customVignette.blink.value;
        
        if (mouseScrollValue == -MOUSE_SCROLL_VALUE && customVignette.blink.value < BLINK_VALUE_MAX) // ���콺 ���� �Ʒ��� ������ ��
        {
            mouseCount++;
            if (mouseCount >= mouseBlinkValues.Length) mouseCount = mouseBlinkValues.Length - 1;
           
            while (customVignette.blink.value < mouseBlinkValues[mouseCount] && elapsedTime < durationTime)
            {
                if(mouseCount == 4) durationTime = 0.15f;
                else durationTime = 0.18f;

                elapsedTime += Time.deltaTime;
                customVignette.blink.value = Mathf.Lerp(currentValue, mouseBlinkValues[mouseCount], elapsedTime / durationTime);
                await UniTask.Yield();
                UpdateEyeState();
            }
            elapsedTime = 0f;
        }
        if (mouseScrollValue == MOUSE_SCROLL_VALUE && customVignette.blink.value > BLINK_VALUE_MIN) // ���콺 ���� ���� �÷��� ��
        {
            mouseCount--;
            if (mouseCount < 0) mouseCount = 0;

            while (customVignette.blink.value > mouseBlinkValues[mouseCount] && elapsedTime < durationTime)
            {   
                if(mouseCount == 3) durationTime = 0.06f;
                else durationTime = 0.18f;

                elapsedTime += Time.deltaTime;
                customVignette.blink.value = Mathf.Lerp(currentValue, mouseBlinkValues[mouseCount], elapsedTime / durationTime);
                await UniTask.Yield();
                UpdateEyeState();
            }
            elapsedTime = 0f;
        }
    }

    public void UpdateEyeState()
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
        else if (prevBlinkValue > customVignette.blink.value || prevBlinkValue == customVignette.blink.value) // ���� ��ġ���� ��� ����(�� Ŭ��)�̸� Opening ���°� �ǰ�
        {
            playerEyeStateMachine.ChangeState(eyeStates[PlayerEyeStateTypes.Opening], true);
        }
        //Debug.Log("���� ���� : " + playerEyeStateMachine.ToString());
        prevBlinkValue = customVignette.blink.value;
    }
    
    public void ChangeEyeState(PlayerEyeStateTypes stateType) => playerEyeStateMachine.ChangeState(eyeStates[stateType]);
    
    public void UnsubscribeToEvents() => InputSystem.OnMouseScrollEvent -= OnLittleBlink;
    
}