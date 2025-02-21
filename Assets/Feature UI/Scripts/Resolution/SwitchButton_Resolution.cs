using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton_Resolution : MonoBehaviour
{
    //private bool switchState = false;
    [SerializeField] private RawImage backGround;
    [SerializeField] private GameObject switchDot;
    [SerializeField] private Color switchColor_On;
    [SerializeField] private Color switchColor_Off;
    [SerializeField] private float switchSpeed = 0.2f;
    [SerializeField] private GameObject windowedIcon;

    private void OnEnable()
    {
        transform.GetComponent<Button>().enabled = true;
    }

    //버튼 클릭시 호출
    public void OnSwitchButtonClicked(bool isReverse)
    {
        SwitchButtonComponent();
        if (isReverse == true)
        {
            print("실행1");
            switchDot.transform.DOLocalMoveX(50, switchSpeed);
            windowedIcon.GetComponent<Image>().DOColor(switchColor_On, switchSpeed);
            backGround.DOColor(switchColor_On, switchSpeed).OnComplete(() => SwitchButtonComponent());
        }
        else
        {
            print("실행2");
            switchDot.transform.DOLocalMoveX(-50, switchSpeed);
            windowedIcon.GetComponent<Image>().DOColor(switchColor_Off, switchSpeed);
            backGround.DOColor(switchColor_Off, switchSpeed).OnComplete(() => SwitchButtonComponent());
        }
    }

    //저장된 데이터대로 스위치 On, Off에 반영
    public void SwitchLoadDataApply(bool isReverse)
    {
        if (isReverse == true)
        {
            switchDot.transform.DOLocalMoveX(50, 0);
            backGround.DOColor(switchColor_On, 0);
            windowedIcon.GetComponent<Image>().DOColor(switchColor_On, 0);
        }
        else
        {
            switchDot.transform.DOLocalMoveX(-50, 0);
            backGround.DOColor(switchColor_Off, 0);
            windowedIcon.GetComponent<Image>().DOColor(switchColor_Off, 0);
        }
    }

    //연속클릭 방지 코드
    private void SwitchButtonComponent()
    {
        if (transform.GetComponent<Button>().enabled == true)
        {
            transform.GetComponent<Button>().enabled = false;
            windowedIcon.GetComponent<Button>().enabled = false;
        }
        else
        {
            transform.GetComponent<Button>().enabled = true;
            windowedIcon.GetComponent<Button>().enabled = true;
        }
    }
}
