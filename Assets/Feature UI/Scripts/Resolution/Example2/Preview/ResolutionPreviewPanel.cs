
using System.ComponentModel;
using UnityEngine;

public class ResolutionPreviewPanel : MonoBehaviour
{
    [SerializeField]
    private ResolutionInside resolutionInside;
    
    [SerializeField]
    private ResolutionOutside resolutionOutside;
    
    private ResolutionSettingsData previewData;
    
    /// <summary>
    /// OnEnable���� Resolution Data�� �ʱ�ȭ
    /// </summary>
    /// <param name="data"></param>
    public void Initialize(ResolutionSettingsData data)
    {
        previewData = data;
        previewData.PropertyChanged += OnPropertyChanged;
        
        //resolutionInside.Initialize(resolutionData);
    }
    
    private void OnDisable()
    {
        previewData.PropertyChanged -= OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        
    }
}
