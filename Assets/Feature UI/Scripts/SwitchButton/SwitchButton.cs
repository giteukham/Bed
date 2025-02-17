using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    private bool switchState = false;
    [SerializeField] private RawImage backGround;
    [SerializeField] private GameObject switchDot;
    [SerializeField] private Color switchColor_On;
    [SerializeField] private Color switchColor_Off;
    [SerializeField] private float switchSpeed = 0.2f;
    [SerializeField] private GameObject rotationIcon;
    private RectTransform rotationIconRectTransform;

    private void Awake()
    {
        rotationIconRectTransform = rotationIcon.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        transform.GetComponent<Button>().enabled = true;
        rotationIcon.GetComponent<Button>().enabled = true;
    }

    public void OnSwitchButtonClicked()
    {
        if (switchState == true)
        {
            switchDot.transform.DOLocalMoveX(-100f, switchSpeed).SetRelative();
            backGround.DOColor(switchColor_Off, switchSpeed);
            switchState = false;
        }
        else
        {
            switchDot.transform.DOLocalMoveX(100f, switchSpeed).SetRelative();
            backGround.DOColor(switchColor_On, switchSpeed);
            switchState = true;
        }
    }

    public void OnSwitchButtonClicked(bool isReverse)
    {
        SwitchButtonComponent();
        if (isReverse == true)
        {
            switchDot.transform.DOLocalMoveX(50, switchSpeed);
            backGround.DOColor(switchColor_On, switchSpeed);
            rotationIconRectTransform.DORotate(rotationIconRectTransform.eulerAngles + new Vector3(0, 0, -180), switchSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(() => SwitchButtonComponent());
        }
        else
        {
            switchDot.transform.DOLocalMoveX(-50, switchSpeed);
            backGround.DOColor(switchColor_Off, switchSpeed);
            rotationIconRectTransform.DORotate(rotationIconRectTransform.eulerAngles + new Vector3(0, 0, 180), switchSpeed, RotateMode.FastBeyond360).SetEase(Ease.Linear).OnComplete(() => SwitchButtonComponent());
        }
    }

    public void SwitchLoadDataApply(bool isReverse)
    {
        if (isReverse == true)
        {
            switchDot.transform.DOLocalMoveX(50, 0.01f);
            backGround.DOColor(switchColor_On, 0.01f);
        }
        else
        {
            backGround.DOColor(switchColor_Off, 0.01f);
        }
    }

    private void SwitchButtonComponent()
    {
        if (transform.GetComponent<Button>().enabled == true)
        {
            transform.GetComponent<Button>().enabled = false;
            rotationIcon.GetComponent<Button>().enabled = false;
        }
        else
        {
            transform.GetComponent<Button>().enabled = true;
            rotationIcon.GetComponent<Button>().enabled = true;
        }
    }
}
