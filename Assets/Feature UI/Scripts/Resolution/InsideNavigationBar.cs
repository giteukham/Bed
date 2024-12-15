
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InsideNavigationBar : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    private GameObject navigationBar;
    
    private Vector2[] maxOffsets;

    private void OnEnable()
    {
        resolutionManagement = ResolutionManagement.Instance;
        navigationBar = this.gameObject;
        maxOffsets = resolutionManagement.ConvertResolutionToOffsets(Display.main.systemWidth, Display.main.systemHeight);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 && InsideWindow.ZoomState == ZoomState.Maximize)
        {
            InsideWindow.ZoomState = ZoomState.Minimize;
            resolutionManagement.ResizeByOffsets(InsideWindow.SavedOffsetMin, InsideWindow.SavedOffsetMax);
        }
        else if (eventData.clickCount == 2 && InsideWindow.ZoomState == ZoomState.Minimize)
        {
            InsideWindow.ZoomState = ZoomState.Maximize;
            InsideWindow.SaveOffset();
            resolutionManagement.ResizeByOffsets(maxOffsets[0], maxOffsets[1]);
        }
    }

    public void SetNavigationBarActive(bool active)
    {
        navigationBar?.SetActive(active);
    }
}
