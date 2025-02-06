using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    private bool switchState = false;
    [SerializeField] private RawImage backGround;
    public GameObject switchDot;

    public void OnSwitchButtonClicked()
    {
        if (switchState == true)
        {
            switchDot.transform.DOLocalMoveX(-100f, 0.2f).SetRelative();
            backGround.DOColor(Color.red, 0.2f);
            switchState = false;
        }
        else
        {
            switchDot.transform.DOLocalMoveX(100f, 0.2f).SetRelative();
            backGround.DOColor(Color.green, 0.2f);
            switchState = true;
        }
    }
}
