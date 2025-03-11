
using System;
using System.ComponentModel;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WindowModeController : FunctionControllerBase
{
    private Toggle windowModeToggle;
    private Animator switchAnimator;
    private Image switchImage;
    private Color offColor, onColor;
    
    private bool isPlayingToggleAnimation = false;
    
    public void Initialize(
        ResolutionSettingsData previewData, 
        ResolutionSettingsDTO backupData,
        ref Color offColor,
        ref Color onColor,
        float speed)
    {
        base.Initialize(previewData, backupData);
        
        this.windowModeToggle = GetComponent<Toggle>();
        this.switchAnimator = GetComponent<Animator>();
        this.switchImage = transform.Find("Background").Find("Toggle Switch").GetComponent<Image>();
        this.offColor = offColor;
        this.onColor = onColor;
        
        ChangeToggleSpeed(speed);
    }

    private void OnEnable()
    {
        Assert.IsNotNull(windowModeToggle, $"{path}Window Mode Toggle is null");
        
        windowModeToggle.onValueChanged.AddListener(OnWindowModeToggleChanged);
        windowModeToggle.isOn = backupData.IsWindowed;
        switchAnimator.SetBool("IsOn", backupData.IsWindowed);
    }

    private void OnDisable()
    {
        windowModeToggle.onValueChanged.RemoveListener(OnWindowModeToggleChanged);
    }
    
    private void OnWindowModeToggleChanged(bool arg0)
    {
        if (isPlayingToggleAnimation) return;
        previewData.IsWindowed = arg0;
    }

    private void ChangeToggleSpeed(float secondTime)
    {
        var clips = AnimationUtility.GetAnimationClips(gameObject);
        var offClip = new AnimationClip();
        var onClip = new AnimationClip();
        
        foreach (var clip in clips)
        {
            switch (clip.name)
            {
                case "Pressed":
                    offClip = clip;
                    break;
                case "BackNormal":
                    onClip = clip;
                    break;
            }
        }
        
        var offEvent = offClip.events;
        var onEvent = onClip.events;
        
        offEvent[1].time = secondTime;
        onEvent[1].time = secondTime;
        
        offClip.events = offEvent;
        onClip.events = onEvent;
        
        
        var offBinding = AnimationUtility.GetCurveBindings(offClip)[0];
        var offCurve = AnimationUtility.GetEditorCurve(offClip, offBinding);
        offCurve.MoveKey(1, new Keyframe(secondTime, offCurve.keys[1].value));
        AnimationUtility.SetEditorCurve(offClip, offBinding, offCurve);
        
        var onBinding = AnimationUtility.GetCurveBindings(onClip)[0];
        var onCurve = AnimationUtility.GetEditorCurve(onClip, onBinding);
        onCurve.MoveKey(1, new Keyframe(secondTime, onCurve.keys[1].value));
        AnimationUtility.SetEditorCurve(onClip, onBinding, onCurve);
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        {
            switchAnimator.SetBool("IsOn", previewData.IsWindowed);
            switchImage.DOColor(previewData.IsWindowed ? onColor : offColor, 0.08f);
        }
    }

    public void AnimationEvent_OnCheckPlayingAnimation()
    {
        isPlayingToggleAnimation = !isPlayingToggleAnimation;
    }
}
