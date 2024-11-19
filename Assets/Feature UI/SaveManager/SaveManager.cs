using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// playerPrefs�� �̿��� ���� �ý���
/// </summary>
public class SaveManager : MonoSingleton<SaveManager>
{
    /// <summary>
    /// �÷��̾��� ����� ī�޶� ����
    /// </summary>
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    /// <summary>
    /// ����ī�޶� ����
    /// </summary>
    [SerializeField] private Camera cam;

    //�Ʒ��� ����׿� ������
    [SerializeField] private Text screenText;
    [SerializeField] private Text testText2;
    [SerializeField] private GameObject resolutionManagement;

    //���� ���۰� ���ÿ� ����Ǿ� �ִ� ������ ���� ������
    private void Awake()
    {
        //���콺 ���� ����
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
        MouseManagement.mouseSpeed = MouseManagement.mouseSensitivity * 500f;
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = LoadMouseVerticalReverse();
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput = LoadMouseHorizontalReverse();
        InputSystem.xBodyReverse = LoadXBodyReverse();

        //������ ���� ����
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = LoadFrameRate();

        //�ػ󵵿� �������� �ڵ����� �����
        //������ ī�޶��� viewport Rect�� ������� �����Ƿ� �������� �ҷ���
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
            print("j ����");
            GL.Clear(true, true, Color.black);  // ȭ���� ���������� ����
        }

        if (resolutionManagement.activeSelf == false)
        {
            testText2.text = Application.targetFrameRate + "";
            screenText.text = Screen.width + " " + Screen.height;
        }

    }

    //���콺 ���� ����
    public void SaveMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();
    }
    public float LoadMouseSensitivity()
    {
        return PlayerPrefs.GetFloat("MouseSensitivity", 1f);
    }

    //���콺 ���Ϲ��� ����
    public void SaveMouseVerticalReverse(bool value)
    {
        PlayerPrefs.SetInt("MouseVerticalReverse", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadMouseVerticalReverse()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseVerticalReverse", 1));
    }

    //���콺 �¿���� ����
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

    //ȭ�� �ػ� ����
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

    //ȭ�� Ǯ��ũ�� ���� ����
    public void SaveIsFullScreen(bool value)
    {
        PlayerPrefs.SetInt("IsFullScreen", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadIsFullScreen()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("IsFullScreen", 1));
    }

    //camRect ����
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

    //������ ����Ʈ ����
    public void SaveFrameRate(int value)
    {
        PlayerPrefs.SetInt("FrameRate", value);
    }
    public int LoadFrameRate()
    {
        return PlayerPrefs.GetInt("FrameRate", 60);
    }
}
