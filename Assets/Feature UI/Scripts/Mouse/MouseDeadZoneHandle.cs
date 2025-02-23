
using System;
using System.Windows.Forms.VisualStyles;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MouseDeadZoneHandle : MonoBehaviour
{
    private Image handleImage;

    public void Init(Color initColor)
    {
        handleImage = GetComponent<Image>();
        handleImage.color = initColor;
    }
    
    public void ChangeHandleColor(Color color)
    {
        handleImage.DOColor(color, 0.2f);
    }
}
