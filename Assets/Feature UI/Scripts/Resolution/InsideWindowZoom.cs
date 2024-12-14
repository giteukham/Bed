using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum ZoomState
{
    ZoomIn,
    ZoomOut
}

public class InsideWindowZoom : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    private Vector2 savedOffsetMin, savedOffsetMax;
    
    private ZoomState zoomState = ZoomState.ZoomIn;

    private void OnEnable()
    {
        resolutionManagement = ResolutionManagement.Instance;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (zoomState == ZoomState.ZoomIn)
        {
            SetZoomState(ZoomState.ZoomOut);
            
            savedOffsetMin = resolutionManagement.InsideOffsetMin;
            savedOffsetMax = resolutionManagement.InsideOffsetMax;
            
            Vector2[] maxOffsets = resolutionManagement.GetOffsetsByResolution(Display.main.systemWidth, Display.main.systemHeight);
            
            DoZoom(maxOffsets[0], maxOffsets[1]);
        }
        else if (zoomState == ZoomState.ZoomOut)
        {
            SetZoomState(ZoomState.ZoomIn);
            DoZoom(savedOffsetMin, savedOffsetMax);
        }
    }
    
    public void SetZoomState(ZoomState state)
    {
        zoomState = state;
    }
    
    private void DoZoom(Vector2 offsetMin, Vector2 offsetMax)
    {
        resolutionManagement.InsideOffsetMin = offsetMin;
        resolutionManagement.InsideOffsetMax = offsetMax;
    }
}