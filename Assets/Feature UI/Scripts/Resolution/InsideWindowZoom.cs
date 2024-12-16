using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InsideWindowZoom : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    
    private Vector2[] maxOffsets;

    private void Awake()
    {
        resolutionManagement = ResolutionManagement.Instance;
        maxOffsets = resolutionManagement.ConvertResolutionToOffsets(new Vector2Int(Display.main.systemWidth, Display.main.systemHeight));
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (InsideWindow.ZoomState == ZoomState.Minimize)
        {
            InsideWindow.ZoomState = ZoomState.Maximize;
            InsideWindow.SaveOffsets(resolutionManagement.InsideOffsetMin, resolutionManagement.InsideOffsetMax);
            resolutionManagement.ResizeByOffsets(maxOffsets[0], maxOffsets[1]);
        }
        else if (InsideWindow.ZoomState == ZoomState.Maximize)
        {
            InsideWindow.ZoomState = ZoomState.Minimize;
            resolutionManagement.ResizeByOffsets(InsideWindow.SavedOffsetMin, InsideWindow.SavedOffsetMax);
        }
    }
}