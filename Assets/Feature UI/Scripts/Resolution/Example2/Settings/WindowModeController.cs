
using System;
using System.ComponentModel;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowModeController : FunctionControllerBase
{
    private Button switchButton;
    private RawImage switchBgImage;
    private RawImage switchDot;
    private Color offColor, onColor;
    [SerializeField]private Image switchIcon;
    private float speed;

    public void Initialize(
        ResolutionSettingsData previewData,
        ResolutionSettingsDTO backupData,
        ref Color offColor,
        ref Color onColor,
        float speed)
    {
        base.Initialize(previewData, backupData);

        this.switchButton = GetComponent<Button>();
        this.switchBgImage = transform.Find("BackGround").GetComponent<RawImage>();
        this.switchDot = transform.Find("SwitchDot").GetComponent<RawImage>();
        this.offColor = offColor;
        this.onColor = onColor;
        this.switchIcon = transform.Find("Windowed Mode Icon").GetComponent<Image>();
        this.speed = speed;
    }

    private void OnEnable()
    {
        var posX = backupData.IsWindowed ? 50 : -50;
        var color = backupData.IsWindowed ? onColor : offColor;
        ChangeWindowModeButtonUI(posX, color, 0f);
    }

    public void OnWindowModeButtonClick()
    {
        previewData.IsWindowed = !previewData.IsWindowed;
        
        var posX = previewData.IsWindowed ? 50 : -50;
        var color = previewData.IsWindowed ? onColor : offColor;
        ChangeWindowModeButtonUI(posX, color, speed);
    }

    private void ChangeWindowModeButtonUI(int posX, Color bgColor, float speed)
    {
        switchDot.transform.DOLocalMoveX(posX, speed);
        switchBgImage.DOColor(bgColor, speed);
        switchIcon.DOColor(bgColor, speed);
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        
    }
}
