using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InsideWindowZoom : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    
    private void Awake()
    {
        resolutionManagement = ResolutionManagement.Instance;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        resolutionManagement.DoZoom();
    }
}