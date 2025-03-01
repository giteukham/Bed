
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InsideNavigationBar : MonoBehaviour, IPointerClickHandler
{
    //private ResolutionManagement resolutionManagement;

    private void Awake()
    {
        //resolutionManagement = ResolutionManagement.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //resolutionManagement.DoZoom(eventData);
    }

    public void SetNavigationBarActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
