using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class ZoomIcon : MonoBehaviour, IPointerClickHandler
{
    enum ZoomState
    {
        ZoomIn,
        ZoomOut
    }
    
    private ResolutionManagement resolutionManagement;
    
    private ZoomState zoomState = ZoomState.ZoomIn;

    private Vector2 savedOffsetMin, savedOffsetMax;

    private void OnEnable()
    {
        resolutionManagement = ResolutionManagement.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (zoomState == ZoomState.ZoomIn)
        {
            savedOffsetMin = resolutionManagement.InsideOffsetMin;
            savedOffsetMax = resolutionManagement.InsideOffsetMax;
            
            Vector2[] maxOffsets = resolutionManagement.GetOffsetsByResolution(Display.main.systemWidth, Display.main.systemHeight);
            
            zoomState = ZoomState.ZoomOut;
            DoZoom(maxOffsets[0], maxOffsets[1]);
        }
        else
        {
            zoomState = ZoomState.ZoomIn;
            DoZoom(savedOffsetMin, savedOffsetMax);
        }
    }
    
    private void DoZoom(Vector2 offsetMin, Vector2 offsetMax)
    {
        resolutionManagement.InsideOffsetMin = offsetMin;
        resolutionManagement.InsideOffsetMax = offsetMax;
    }
}