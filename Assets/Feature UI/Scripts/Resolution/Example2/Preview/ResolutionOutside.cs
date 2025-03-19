
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Assertions;

public class ResolutionOutside : MonoBehaviour
{
    private RectTransform outsideRect;
    
    [Header("내부 UI")]
    [SerializeField]
    private RectTransform blankRect;

    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    private DynamicUIData dynamicUIData;
    
    private readonly string path = "Menu UI/Resolution Settings Screen/Preview Panel/Outside/";

    private readonly Color fullScreenBlankColor = Color.black;
    private readonly Color windowScreenBlankColor = Color.gray;
    
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData, DynamicUIData dynamicUIData)
    {
        Assert.IsNotNull(blankRect, $"{path}BlankRect is null");
        
        this.previewData = previewData;
        this.backupData = backupData;
        this.dynamicUIData = dynamicUIData;
        
        this.previewData.PropertyChanged += OnPropertyChanged;
        
        outsideRect = GetComponent<RectTransform>();
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ResolutionSettingsData.ResolutionWidth) ||
            e.PropertyName == nameof(ResolutionSettingsData.ResolutionHeight))
        {
            ResizeOutside();
        }
        
        // 전체모드일 땐 Blank가 검은색, 창모드일 땐 회색
        // if (e.PropertyName == nameof(ResolutionSettingsData.IsWindowed))
        // {
        //     ChangeBlankColor(previewData.IsWindowed ? windowScreenBlankColor : fullScreenBlankColor);
        // }
    }

    private void OnEnable()
    {
        ResizeOutside();
    }
    
    private void ResizeOutside()
    {
        outsideRect.sizeDelta = dynamicUIData.OutSideSize;
        blankRect.sizeDelta = dynamicUIData.BlankSize;
    }

    private void ChangeBlankColor(Color color)
    {
        blankRect.GetComponent<UnityEngine.UI.Image>().color = color;
    }
}
