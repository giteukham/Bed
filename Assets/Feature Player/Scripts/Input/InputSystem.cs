using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 수정 날짜 : 2024-11-18 최무령
/// </summary>
public class InputSystem : MonoBehaviour
{

    #region Mouse Events
    public static event Action OnMouseWheelClickEvent; 
    public static event Action<int> OnMouseScrollEvent;
    public event Action OnMouseClickEvent;   
    #endregion

    private void OnMouseDelta(InputValue value)
    {
        if (PlayerConstant.isPlayerStop) 
        {
            MouseDeltaHorizontal = 0;
            MouseDeltaVertical = 0;
        }
        if ( PlayerConstant.isParalysis ) 
        {
            MouseDeltaHorizontal = value.Get<Vector2>().x * 0.02f * horizontalConstant;
            MouseDeltaVertical = value.Get<Vector2>().y * 0.02f * verticalConstant;
        }
        else
        {
            MouseDeltaHorizontal = value.Get<Vector2>().x * horizontalConstant;
            MouseDeltaVertical = value.Get<Vector2>().y * verticalConstant;
        }
    }

    private void OnMouseScroll(InputValue value)
    {
        if (PlayerConstant.isPlayerStop) return;
        OnMouseScrollEvent?.Invoke(Convert.ToInt32(value.Get<Vector2>().y));
    }
    
    private void OnMouseWheelClick(InputValue value)
    {
        if (PlayerConstant.isPlayerStop) return;
        if (value.isPressed)
        {
            OnMouseWheelClickEvent?.Invoke();
        }
    }

    private void OnMouseClick(InputValue value)
    {
        if (value.isPressed)
        {
            OnMouseClickEvent?.Invoke();
        }
    }
}