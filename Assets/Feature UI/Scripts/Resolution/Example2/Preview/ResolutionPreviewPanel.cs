
using System.ComponentModel;
using UnityEngine;

public class ResolutionPreviewPanel : MonoBehaviour
{
    [SerializeField]
    private ResolutionInside resolutionInside;
    
    [SerializeField]
    private ResolutionOutside resolutionOutside;
    
    private ResolutionSettingsData previewData;
    private ResolutionSettingsDTO backupData;
    
    /// <summary>
    /// OnEnable에서 Resolution Data를 초기화
    /// </summary>
    /// <param name="previewData"></param>
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData)
    {
        this.previewData = previewData;
        this.backupData = backupData;
        //resolutionInside.Initialize(resolutionData);
    }
    
    private void OnEnable()
    {
        previewData.PropertyChanged += OnPropertyChanged;
    }
    
    private void OnDisable()
    {
        previewData.PropertyChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        
    }
}
