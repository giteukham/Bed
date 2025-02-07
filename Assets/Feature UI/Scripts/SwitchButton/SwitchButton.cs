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
    [SerializeField] private float switchSpeed;
    [SerializeField] private RectTransform rotationIcon;

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
        if (isReverse == true)
        {
            switchDot.transform.DOLocalMoveX(50, switchSpeed);
            rotationIcon.transform.DORotate(new Vector3(0, 0, -180), switchSpeed, RotateMode.FastBeyond360).SetRelative()
                .OnKill(() => rotationIcon.transform.eulerAngles = new Vector3(0, 0, -180));

            backGround.DOColor(switchColor_On, switchSpeed);
        }
        else
        {
            switchDot.transform.DOLocalMoveX(-50, switchSpeed);
            rotationIcon.transform.DORotate(new Vector3(0, 0, 180), switchSpeed, RotateMode.Fast).SetRelative()
                .OnKill(() => rotationIcon.transform.eulerAngles = new Vector3(0, 0, 180));

            backGround.DOColor(switchColor_Off, switchSpeed);
        }
    }

    /*public void OnSwitchButtonClicked(bool isReverse)
    {
        int targetX = isReverse ? 50 : -50; // 이동할 목표 위치
        //애니메이션 실행 메소드(현재 값, 변환 적용식, 목표값, 바뀌는 시간)
        DOTween.To(
            () => switchDot.transform.localPosition.x,
            changeValue => switchDot.transform.localPosition = new Vector3(changeValue, switchDot.transform.localPosition.y, switchDot.transform.localPosition.z),
            targetX,
            switchSpeed);

        backGround.DOColor(isReverse ? switchColor_On : switchColor_Off, switchSpeed);
    }*/

    public void SwitchLoadDataApply(bool isReverse)
    {
        if (isReverse == true)
        {
            //switchDot.transform.DOLocalMoveX(100f, 0.01f).SetRelative();
            switchDot.transform.DOLocalMoveX(50, 0.01f);
            backGround.DOColor(switchColor_On, 0.01f);
        }
        else
        {
            backGround.DOColor(switchColor_Off, 0.01f);
        }
    }
}
