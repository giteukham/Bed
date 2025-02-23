
using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MouseDeadZoneUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Slider deadZoneSlider;
    
    [SerializeField] 
    private RectTransform backgroundTransform, valueBarTransform;

    [SerializeField]
    private RectTransform fill;
    
    [SerializeField]
    private RectTransform handle;

    [Header("Settings")]
    [SerializeField]
    private Color idleColor;
    
    [SerializeField]
    private Color activeColor;

    private MouseSettings mouseSettings;
    private MouseSettingsPreviewPlayer previewPlayer;
    
    private MouseDeadZoneHandle mouseDeadZoneHandle;
    private MouseDeadZoneFill   mouseDeadZoneFill;

    private void Awake()
    {
        mouseDeadZoneHandle = handle.GetComponent<MouseDeadZoneHandle>();
        mouseDeadZoneFill = fill.GetComponent<MouseDeadZoneFill>();
    }

    private void Update()
    {
        ChangeValueBarPosition();
    }

    private void OnEnable()
    {
        mouseSettings = MouseSettings.Instance;
        
        mouseDeadZoneHandle.Init(idleColor);
        mouseDeadZoneFill.ChangeFillColor(idleColor);
        
        previewPlayer = (MouseSettingsPreviewPlayer) MouseSettings.Instance.PreviewPlayer;
        previewPlayer.OnDirectionStateChanged += ChangeFillColorDependingOnDirection;
        deadZoneSlider.value = mouseSettings.DeadZoneCurrentValue;
        deadZoneSlider.onValueChanged.AddListener(OnValueChanged);
    }
    
    private void OnDisable()
    {
        deadZoneSlider.onValueChanged.RemoveAllListeners();
        previewPlayer.OnDirectionStateChanged -= ChangeFillColorDependingOnDirection;
    }

    /// <summary>
    /// DeadZone 이미지의 크기를 조절
    /// </summary>
    /// <param name="value">슬라이더 값 0 ~ DeadZoneLimit 까지</param>
    private void OnValueChanged(float value)
    {
        ChangeDeadZoneCurrentValue(value);
        mouseSettings.ChangeTurnAxisSpeed(value);
    }

    private void ChangeFillColorDependingOnDirection(PlayerDirectionStateTypes types)
    {
        if (types == PlayerDirectionStateTypes.Switching)
        {
            mouseDeadZoneFill.ChangeFillColor(activeColor);
            mouseDeadZoneHandle.ChangeHandleColor(activeColor);
        }
        else if (mouseDeadZoneFill.FillImage.color != idleColor)
        {
            mouseDeadZoneFill.ChangeFillColor(idleColor);
            mouseDeadZoneHandle.ChangeHandleColor(idleColor);
        }
    }

    /// <summary>
    /// 슬라이더의 값을 변경
    /// </summary>
    /// <param name="value"></param>
    private void ChangeDeadZoneCurrentValue(float value)
    {
        mouseSettings.ChangeDeadZoneCurrentValue(value);
        deadZoneSlider.value = Mathf.Clamp(value, 0f, mouseSettings.DeadZoneLimit);
    }
    
    private void ChangeValueBarPosition()
    {
        var barPos = valueBarTransform.anchoredPosition;
        var targetValue = Mathf.Abs(mouseSettings.MouseHorizontalSpeed) * ((backgroundTransform.rect.width - 5f) / mouseSettings.MouseAxisLimit);

        // 현재 슬라이더 값을 목표 값으로 부드럽게 보간합니다.
        barPos.x = Mathf.Lerp(valueBarTransform.anchoredPosition.x, targetValue, Time.deltaTime * 100f);
        barPos.x = Mathf.Floor(barPos.x * 1000f) * 0.001f;
        valueBarTransform.anchoredPosition = barPos;
    }
    
}
