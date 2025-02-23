
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
    /// DeadZone �̹����� ũ�⸦ ����
    /// </summary>
    /// <param name="value">�����̴� �� 0 ~ DeadZoneLimit ����</param>
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
    /// �����̴��� ���� ����
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

        // ���� �����̴� ���� ��ǥ ������ �ε巴�� �����մϴ�.
        barPos.x = Mathf.Lerp(valueBarTransform.anchoredPosition.x, targetValue, Time.deltaTime * 100f);
        barPos.x = Mathf.Floor(barPos.x * 1000f) * 0.001f;
        valueBarTransform.anchoredPosition = barPos;
    }
    
}
