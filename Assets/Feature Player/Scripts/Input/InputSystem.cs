using System;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: ¼ºÇõÀÌ²¨¶û ÇÕÃÄ¾ß ÇÔ.

public class InputSystem : MonoBehaviour
{
    private static float mouseDeltaX;
    private static float mouseDeltaY;
    public static float MouseDeltaX
    {
        get { return mouseDeltaX; }
        private set { mouseDeltaX = value; }
    }

    public static float MouseDeltaY
    {
        get { return mouseDeltaY; }
        private set { mouseDeltaY = value; }
    }

    public static event Action OnMouseWheelClickEvent; 
    public static event Action<int> OnMouseScrollEvent; // ¸¶¿ì½º ÈÙ. À§·Î´Â 120, ¾Æ·¡·Î´Â -120

    private void OnMouseDelta(InputValue value)
    {
        if (PlayerConstant.isPlayerStop) return;
        if ( PlayerConstant.isParalysis ) 
        {
            MouseDeltaX = value.Get<Vector2>().x * 0.02f;
            MouseDeltaY = value.Get<Vector2>().y * 0.02f;
        }
        else
        {
            MouseDeltaX = value.Get<Vector2>().x;
            MouseDeltaY = value.Get<Vector2>().y;
        }
        
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