
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public static class StaticUIData
{
    public static readonly Vector2 OutsideMaxBaseSize = new(670, 390);      // Outside 최대 기준 사이즈.
    public static readonly float BaseAspectRatio = 16f / 9f;
    public static readonly float BaseReverseAspectRatio = 9f / 16f;
    public static readonly int BaseWidth = 1920;
    public static readonly int BaseHeight = 1080;
    public static readonly float OutsideAndInsideDistance = 30f;
}

/// <summary>
/// Aspect Ratio를 먼저 넣어줄 것
/// </summary>
public class DynamicUIData
{
    private ResolutionSettingsData previewData;
    
    public DynamicUIData(ResolutionSettingsData previewData)
    {
        this.previewData = previewData;
    }
    
    public float UserAspectRatio => Display.main.systemWidth / (float) Display.main.systemHeight;
    
    public float UserReverseAspectRatio => Display.main.systemHeight / (float) Display.main.systemWidth;

    public Vector2 BlankSize
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
            
            var size = ResolutionUtility.ConvertResolutionToSize(Display.main.systemWidth, Display.main.systemHeight, UserAspectRatio);
            
            if (UserAspectRatio >= StaticUIData.BaseAspectRatio)
            {
                size.x = size.y * UserAspectRatio;
            }
            else
            {
                size.y = size.x * UserReverseAspectRatio;
            }
            
            return size;
        }
    }
    
    public Vector2 OutSideSize
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
            if (BlankSize == Vector2.zero) Debug.LogWarning("InsideMaxSize is 0");

            var size = new Vector2(BlankSize.x + StaticUIData.OutsideAndInsideDistance, BlankSize.y + StaticUIData.OutsideAndInsideDistance);
            return size;
        }
    }
    
    public Vector2 InsideMaxSize
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");

            Vector2 size = previewData.IsWindowed 
                ? ResolutionUtility.ConvertResolutionToSize(previewData.windowedMaxWidth, previewData.windowedMaxHeight, UserAspectRatio) 
                : ResolutionUtility.ConvertResolutionToSize(previewData.fullScreenMaxWidth, previewData.fullScreenMaxHeight, UserAspectRatio);
            
            return size;
        }
    }
    
    public Vector2[] InsideMaxOffsets
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
            if (OutSideSize == Vector2.zero) Debug.LogWarning("OutSideMaxSize is 0");

            return ResolutionUtility.ConvertSizeToOffset(InsideMaxSize);
        }
    }
    
    public Vector2 InsideMinSize
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");

            Vector2 size = previewData.IsWindowed 
                ? ResolutionUtility.ConvertResolutionToSize(previewData.windowedMinWidth, previewData.windowedMinHeight, UserAspectRatio)
                : ResolutionUtility.ConvertResolutionToSize(previewData.fullScreenMinWidth, previewData.fullScreenMinHeight, UserAspectRatio);
            
            return size;
        }
    }
    
    public Vector2[] InsideMinOffsets
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
            if (InsideMinSize == Vector2.zero) Debug.LogWarning("InsideMinSize is 0");

            return ResolutionUtility.ConvertSizeToOffset(InsideMinSize);
        }
    }
    
    public Vector2 InsideCurrentSize
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
            
            return ResolutionUtility.ConvertResolutionToSize(previewData.ResolutionWidth, previewData.ResolutionHeight, UserAspectRatio);
        }
    }
    
    public Vector2[] InsideCurrentOffsets
    {
        get
        {
            if (Mathf.Approximately(UserAspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
            if (InsideCurrentSize == Vector2.zero) Debug.LogWarning("InsideCurrentSize is 0");

            return ResolutionUtility.ConvertSizeToOffset(InsideCurrentSize);
        }
    }
}

public class ResolutionPreviewPanel : MonoBehaviour
{
    [SerializeField]
    private ResolutionInside resolutionInside;
    
    [SerializeField]
    private ResolutionOutside resolutionOutside;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    private DynamicUIData dynamicUIData;
    
    private readonly string path = "Menu UI/Resolution Settings Screen/Preview Panel/";
    
    /// <summary>
    /// OnEnable에서 Resolution Data를 초기화
    /// </summary>
    /// <param name="previewData"></param>
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData)
    {
        Assert.IsNotNull(resolutionInside, $"{path}ResolutionInside is null");
        Assert.IsNotNull(resolutionOutside, $"{path}ResolutionOutside is null");
        
        this.previewData = previewData;
        this.backupData = backupData;

        dynamicUIData = new DynamicUIData(previewData);
        
        resolutionOutside?.Initialize(previewData, backupData, dynamicUIData);
        resolutionInside?.Initialize(previewData, backupData, dynamicUIData);
    }

    public void SetResolutionText(int width, int height, int frame, bool isModified)
    {
        resolutionInside?.SetResolutionText(width, height, frame, isModified);
    }
}
