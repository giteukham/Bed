using System;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: �����̲��� ���ľ� ��.

public class InputSystem : MonoBehaviour
{
    private static float mouseDeltaX;
    public static float MouseDeltaX
    {
        get { return mouseDeltaX; }
        private set { mouseDeltaX = value; }
    }

    public static event Action OnMouseWheelClickEvent; 
    public static event Action<int> OnMouseScrollEvent; // ���콺 ��. ���δ� 120, �Ʒ��δ� -120

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