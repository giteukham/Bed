
using UnityEngine;

public class ResolutionPreviewPanel : MonoBehaviour
{
    [SerializeField]
    private ResolutionInside resolutionInside;
    
    [SerializeField]
    private ResolutionOutside resolutionOutside;
    
    private ResolutionSettingsData resolutionData;
    
    public void Initialize(ResolutionSettingsData resolutionData)
    {
        this.resolutionData = resolutionData;
    }
}
