using System;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: ¼ºÇõÀÌ²¨¶û ÇÕÃÄ¾ß ÇÔ.

public class InputSystem : MonoBehaviour
{
    private static float mouseDeltaX;
    public static float MouseDeltaX
    {
        get { return mouseDeltaX; }
        private set { mouseDeltaX = value; }
    }

    private void OnMouseDelta(InputValue value)
    {
        MouseDeltaX = value.Get<Vector2>().x * Time.deltaTime;
    }

    private void OnMouseScroll(InputValue value)
    {
        Debug.Log(value.Get<Vector2>().y);
    }
    
    private void OnMouseWheelClick(InputValue value)
    {
        Debug.Log("Wheel Click" + value.isPressed);
    }
}