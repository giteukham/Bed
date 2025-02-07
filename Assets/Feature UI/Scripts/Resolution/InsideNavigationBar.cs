
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InsideNavigationBar : MonoBehaviour, IPointerClickHandler
{
    private ResolutionManagement resolutionManagement;
    private GameObject navigationBar;

    private void Awake()
    {
        resolutionManagement = ResolutionManagement.Instance;
        navigationBar = this.gameObject;
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
