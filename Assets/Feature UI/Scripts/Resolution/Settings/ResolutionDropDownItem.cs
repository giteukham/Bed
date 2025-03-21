using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResolutionDropDownItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ResolutionDropDowncontroller resolutionSelectController;
    public void OnPointerClick(PointerEventData eventData)
    {
        resolutionSelectController.OnSelection();
    }
}
