
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public class ResolutionPreviewPanel : MonoBehaviour
{
    /// <summary>
    /// Aspect Ratio를 먼저 넣어줄 것
    /// </summary>
    public class UIPreviewData
    {
        // Inside는 Outside - 30
        private Vector2 outsideMaxSize, insideMaxSize;
        private Vector2 outsideMinSize, insideMinSize;
        private float aspectRatio, reverseAspectRatio;

        public float AspectRatio
        {
            get => aspectRatio;
            set
            {
                aspectRatio = value;
                reverseAspectRatio = 1 / aspectRatio;
            }
        }
        public float ReverseAspectRatio => reverseAspectRatio;
        public Vector2 OutSideMaxSize
        {
            get
            {
                if (Mathf.Approximately(AspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
                outsideMaxSize.y = outsideMaxSize.x * reverseAspectRatio;
                return outsideMaxSize;
            }
            set => outsideMaxSize = value;
        }
        public Vector2 InsideMaxSize
        {
            get
            {
                if (Mathf.Approximately(AspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
                insideMaxSize.y = insideMaxSize.x * reverseAspectRatio;
                return insideMaxSize;
            }
            set => insideMaxSize = value;
        }
        public Vector2 OutSideMinSize
        {
            get
            {
                if (Mathf.Approximately(AspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
                outsideMinSize.y = outsideMinSize.x * reverseAspectRatio;
                return outsideMinSize;
            }
            set => outsideMinSize = value;
        }
        public Vector2 InsideMinSize
        {
            get
            {
                if (Mathf.Approximately(AspectRatio, 0f)) Debug.LogWarning("AspectRatio is 0");
                insideMinSize.y = insideMinSize.x * reverseAspectRatio;
                return insideMinSize;
            }
            set => insideMinSize = value;
        }
        
    }
    
    [SerializeField]
    private ResolutionInside resolutionInside;
    
    [SerializeField]
    private ResolutionOutside resolutionOutside;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    private UIPreviewData uiData;
    
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
        
        uiData = new UIPreviewData
        {
            //aspectRatio = (float) previewData.ResolutionWidth / previewData.ResolutionHeight,
            AspectRatio = (float) Display.main.systemWidth / Display.main.systemHeight, // TODO: 임시 값. 나중에 위 코드로 변경,
            OutSideMaxSize = new Vector2(640, 360),
            InsideMaxSize = new Vector2(610, 343),
            OutSideMinSize = new Vector2(320, 180),         // TODO: 임시 값
            InsideMinSize = new Vector2(290, 163)           // TODO: 임시 값    
        };
        
        resolutionOutside?.Initialize(uiData);
        resolutionInside?.Initialize(previewData, backupData, uiData);
    }
}
