using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MouseDeadZoneFill : MonoBehaviour
{
    private Image fillImage;
    public Image FillImage => fillImage;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    public void ChangeFillColor(Color color)
    {
        fillImage.DOColor(color, 0.2f);
    }
}
