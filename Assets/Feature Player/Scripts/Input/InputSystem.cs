using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// 수정 날짜 : 2024-11-18 최무령
/// </summary>
public class InputSystem : MonoBehaviour
{
    private static int verticalConstant = 1, horizontalConstant = 1;                    // 1은 정상, -1은 반전
    private static float mouseDeltaHorizontal;
    private static float mouseDeltaVertical;
    public static float MouseDeltaHorizontal
    {
        get { return mouseDeltaHorizontal; }
        private set { mouseDeltaHorizontal = value; }
    }

    public static float MouseDeltaVertical
    {
        get { return mouseDeltaVertical; }
        private set { mouseDeltaVertical = value; }
    }
    public static int xBodyReverse;
    
    private static bool isVerticalReverse = false, isHorizontalReverse = false;         // false는 정상, true는 반전
    public static bool IsVerticalReverse => isVerticalReverse;
    public static bool IsHorizontalReverse => isHorizontalReverse;

    #region Mouse Events
    public static event Action OnMouseWheelClickEvent; 
    public static event Action<int> OnMouseScrollEvent;
    public event Action OnMouseClickEvent;   
    #endregion

    private void Awake()
    {
        isVerticalReverse = SaveManager.Instance.LoadMouseVerticalReverse();
        verticalConstant = isVerticalReverse ? -1 : 1;
        isHorizontalReverse = SaveManager.Instance.LoadMouseHorizontalReverse();
        horizontalConstant = isHorizontalReverse ? -1 : 1;
    }

    public static void ToggleVerticalReverse()
    {
        isVerticalReverse = !isVerticalReverse;
        verticalConstant = isVerticalReverse ? -1 : 1;
        SaveManager.Instance.SaveMouseVerticalReverse(isVerticalReverse);
    }
    
    public static void ToggleHorizontalReverse()
    {
        isHorizontalReverse = !isHorizontalReverse;
        horizontalConstant = isHorizontalReverse ? -1 : 1;
        SaveManager.Instance.SaveMouseHorizontalReverse(isHorizontalReverse);
    }

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