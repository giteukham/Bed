
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
    /// OnEnable���� Resolution Data�� �ʱ�ȭ
    /// </summary>
    /// <param name="previewData"></param>
    public void Initialize(ResolutionSettingsData previewData, ResolutionSettingsDTO backupData)
    {
        this.previewData = previewData;
        this.backupData = backupData;
        //resolutionInside.Initialize(previewData, backupData);
    }
    
}
