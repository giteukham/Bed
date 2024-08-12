using System;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: 성혁이꺼랑 합쳐야 함.

public class InputSystem : MonoBehaviour
{
    private static float mouseDeltaX;
    public static float MouseDeltaX
    {
        get { return mouseDeltaX; }
        private set { mouseDeltaX = value; }
    }

    public static event Action OnMouseWheelClickEvent; 
    public static event Action<int> OnMouseScrollEvent; // 마우스 휠. 위로는 120, 아래로는 -120

    private void OnMouseDelta(InputValue value)
    {
        MouseDeltaX = value.Get<Vector2>().x;
    }

    private void OnMouseScroll(InputValue value)
    {
        OnMouseScrollEvent?.Invoke(Convert.ToInt32(value.Get<Vector2>().y));
    }
    
    private void OnMouseWheelClick(InputValue value)
    {
        if (value.isPressed)
        {
            OnMouseWheelClickEvent?.Invoke();
        }
    }
}