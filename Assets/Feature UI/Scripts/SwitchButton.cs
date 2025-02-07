using DG.Tweening;
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
}
