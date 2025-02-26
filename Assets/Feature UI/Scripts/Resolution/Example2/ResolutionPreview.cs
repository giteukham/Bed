
using UnityEngine;

public class ResolutionPreview : MonoBehaviour
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
