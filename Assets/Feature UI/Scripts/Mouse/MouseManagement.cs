using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseManagement : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image verticalSwitch;
    [SerializeField] private Image horizontalSwitch;
    [SerializeField] private SaveManager saveManager;

    /// <summary>
    /// 플레이어의 버츄얼 카메라에 접근
    /// </summary>
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    /// <summary>
    /// 초록 스위치 이미지
    /// </summary>
    [SerializeField] private Sprite onImage;
    /// <summary>
    /// 빨강 스위치 이미지
    /// </summary>
    [SerializeField] private Sprite offImage;


    //기본값 정리
    //감도 1f, 상하반전 true(1), 좌우반전 false(0)

    /// <summary>
    /// 마우스 감도 전역 변수
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// 최종 적용되는 마우스 감도
    /// </summary>
    public static float mouseSpeed = 500f;

    private void OnEnable()
    {
        //슬라이더 조절바에 이전게임에 저장됐던 설정값 적용
        slider.value = saveManager.LoadMouseSensitivity();
        //마우스 반전에 따른 버튼 이미지 변경
        verticalSwitch.sprite = saveManager.LoadMouseVerticalReverse() ? offImage : onImage;
        horizontalSwitch.sprite = saveManager.LoadMouseHorizontalReverse() ? onImage : offImage;
    }

    //마우스 설정창 비활성화시 업데이트 문은 안돌아감
    private void Update()
    {
        //mouseSpeed = mouseSensitivity * 500f;
        /*if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = true;
        }

        print(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput);*/
    }

    /// <summary>
    /// 슬라이더 조절바 사용시 자동 호출
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * 500f;
        saveManager.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// 마우스 상하반전
    /// </summary>
    public void MouseVerticalReverse()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = !virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput;
        saveManager.SaveMouseVerticalReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput);
        //버튼 이미지 변경(true일 경우 빨강, false일 경우 초록)
        verticalSwitch.sprite = saveManager.LoadMouseVerticalReverse() ? offImage : onImage;
    }

    /// <summary>
    /// 마우스 좌우 반전
    /// </summary>
    public void MouseHorizontalReverse()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput = !virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput;
        saveManager.SaveMouseHorizontalReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput);
        //버튼 이미지 변경(false일 경우 빨강, true일 경우 초록)
        horizontalSwitch.sprite = saveManager.LoadMouseHorizontalReverse() ? onImage : offImage;
    }


}
