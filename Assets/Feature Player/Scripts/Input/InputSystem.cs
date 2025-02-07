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