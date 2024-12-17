
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InsideNavigationBar : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    private GameObject navigationBar;

    private void Awake()
    {
        resolutionManagement = ResolutionManagement.Instance;
        navigationBar = this.gameObject;
    }

    private void OnEnable()
    {
        SetNavigationBarActive(!resolutionManagement.IsWindowedScreenReady);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        resolutionManagement.DoZoom(eventData);
    }

    public void SetNavigationBarActive(bool active)
    {
        navigationBar.SetActive(active);
    }
}
