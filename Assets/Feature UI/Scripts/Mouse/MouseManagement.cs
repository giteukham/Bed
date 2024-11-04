using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManagement : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] SaveManager saveManager;

    /// <summary>
    /// 마우스 감도 전역 변수
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// 최종 적용되는 마우스 감도
    /// </summary>
    public static float mouseSpeed = 500f;
    /// <summary>
    /// 마우스 반전
    /// </summary>
    public static int mouseReverse = 1;

    private void Awake()
    {
        //슬라이더 조절바에 이전게임에 저장됐던 설정값 적용
        slider.value = saveManager.LoadMouseSensitivity();
    }

    //마우스 설정창 비활성화시 안돌아감
    private void Update()
    {
        //mouseSpeed = mouseSensitivity * 500f;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mouseSpeed -= 100f;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mouseSpeed += 100f;
        }
    }

    /// <summary>
    /// 슬라이더 조절바 사용시 자동 호출
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * 500f * mouseReverse;
        saveManager.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// 마우스 상하반전
    /// </summary>
    public void MouseReverse()
    {
        mouseReverse *= -1;
        saveManager.SaveMouseReverse(mouseReverse);
        print("마우스 리버스 : " + mouseReverse + ", 감도 : " + mouseSpeed);
    }


}
