
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ResolutionOutside : MonoBehaviour
{
    private RectTransform outsideRect;
    
    [Header("내부 UI")]
    [SerializeField]
    private RectTransform blankRect;

    private ResolutionPreviewPanel.UIPreviewData uiData;
    
    private readonly int blankSizeDecrease = 30;            // Blank 사이즈를 줄이기 위한 값   
    private readonly string path = "Menu UI/Resolution Settings Screen/Preview Panel/Outside/";
    
    public void Initialize(ResolutionPreviewPanel.UIPreviewData uiData)
    {
        Assert.IsNotNull(blankRect, $"{path}BlankRect is null");
        
        outsideRect = GetComponent<RectTransform>();
        this.uiData = uiData;
    }

    private void OnEnable()
    {
        //aspectRatio = (float) previewData.ResolutionWidth / previewData.ResolutionHeight;
        uiData.AspectRatio = (float)Display.main.systemWidth / Display.main.systemHeight; // TODO: 임시 값. 나중에 위 코드로 변경,
        outsideRect.sizeDelta = new Vector2(uiData.OutSideMaxSize.x, uiData.OutSideMaxSize.y + (blankSizeDecrease * 0.5f));
        blankRect.sizeDelta = new Vector2(uiData.InsideMaxSize.x, uiData.InsideMaxSize.y);
    }
}
