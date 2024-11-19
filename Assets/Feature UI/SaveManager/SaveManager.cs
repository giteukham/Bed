using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// playerPrefs를 이용한 저장 시스템
/// </summary>
public class SaveManager : MonoSingleton<SaveManager>
{
    /// <summary>
    /// 플레이어의 버츄얼 카메라에 접근
    /// </summary>
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    /// <summary>
    /// 메인카메라에 접근
    /// </summary>
    [SerializeField] private Camera cam;

    //아래는 디버그용 변수들
    [SerializeField] private Text screenText;
    [SerializeField] private Text testText2;
    [SerializeField] private GameObject resolutionManagement;

    //게임 시작과 동시에 저장되어 있던 실제값 설정 가져옴
    private void Awake()
    {
        //마우스 관련 변수
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
        MouseManagement.mouseSpeed = MouseManagement.mouseSensitivity * 500f;
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = LoadMouseVerticalReverse();
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput = LoadMouseHorizontalReverse();
        InputSystem.xBodyReverse = LoadXBodyReverse();

        //프레임 관련 변수
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = LoadFrameRate();

        //해상도와 프레임은 자동으로 적용됨
        //하지만 카메라의 viewport Rect는 적용되지 않으므로 수동으로 불러옴
        //cam.rect = LoadCamRect();
    }

    private void Start()
    {
        print($"{LoadCamRect().x} : {LoadCamRect().y} : {LoadCamRect().width} : {LoadCamRect().height}");
        cam.rect = LoadCamRect();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            print("j 누름");
            GL.Clear(true, true, Color.black);  // 화면을 검은색으로 지움
        }

        if (resolutionManagement.activeSelf == false)
        {
            testText2.text = Application.targetFrameRate + "";
            screenText.text = Screen.width + " " + Screen.height;
        }

    }

    //마우스 감도 관련
    public void SaveMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }
    public float LoadMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }

    //마우스 상하반전 관련
    public void SaveMouseVerticalReverse(bool value)
    {
        PlayerPrefs.SetInt("MouseVerticalReverse", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadMouseVerticalReverse()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseVerticalReverse", 1));
    }

    //마우스 좌우반전 관련
    public void SaveMouseHorizontalReverse(bool value)
    {
        PlayerPrefs.SetInt("MouseHorizontalReverse", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadMouseHorizontalReverse()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseHorizontalReverse", 0));
    }
    public void SaveXBodyReverse(int value)
    {
        InputSystem.xBodyReverse = value;
        PlayerPrefs.SetInt("XBodyReverse", value);
        PlayerPrefs.Save();
    }
    public int LoadXBodyReverse()
    {
        return PlayerPrefs.GetInt("XBodyReverse", 1);
    }

    //화면 해상도 관련
    public void SaveResolution(int value1, int value2)
    {
        //PlayerPrefs.SetString("Resolution", value1 + " " + value2);

        PlayerPrefs.SetInt("ResolutionWidth", value1);
        PlayerPrefs.SetInt("ResolutionHeight", value2);

        PlayerPrefs.Save();
    }
    public void LoadResolution(out int value1, out int value2)
    {
        value1 = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        value2 = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        //return PlayerPrefs.GetString("Resolution", "1920 1080");
    }

    //화면 풀스크린 여부 관련
    public void SaveIsFullScreen(bool value)
    {
        PlayerPrefs.SetInt("IsFullScreen", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadIsFullScreen()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("IsFullScreen", 1));
    }

    //camRect 관련
    public void SaveCamRect(float value1, float value2, float value3, float value4)
    {
        PlayerPrefs.SetFloat("CamX", value1);
        PlayerPrefs.SetFloat("CamY", value2);
        PlayerPrefs.SetFloat("CamWidth", value3);
        PlayerPrefs.SetFloat("CamHeight", value4);
        PlayerPrefs.Save();
    }

    public Rect LoadCamRect()
    {
        Rect rect = new Rect
            (
            PlayerPrefs.GetFloat("CamX", 0), PlayerPrefs.GetFloat("CamY", 0),
            PlayerPrefs.GetFloat("CamWidth", 1), PlayerPrefs.GetFloat("CamHeight", 1)
            );
        return rect;
    }

    //프레임 레이트 관련
    public void SaveFrameRate(int value)
    {
        PlayerPrefs.SetInt("FrameRate", value);
    }
    public int LoadFrameRate()
    {
        return PlayerPrefs.GetInt("FrameRate", 60);
    }
}
