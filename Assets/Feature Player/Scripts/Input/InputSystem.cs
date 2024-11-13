using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoSingleton<InputSystem>
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
    public static int xBodyReverse;

    #region Mouse Events
    public static event Action OnMouseWheelClickEvent; 
    public static event Action<int> OnMouseScrollEvent;
    public event Action OnMouseClickEvent;   
    #endregion

    private void OnMouseDelta(InputValue value)
    {
        if (PlayerConstant.isPlayerStop) 
        {
            MouseDeltaX = 0;
            MouseDeltaY = 0;
        }
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