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

    //���� ���۰� ���ÿ� ����Ǿ� �ִ� ������ ���� ������
    private void Awake()
    {
        //������ ���� ����
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = LoadFrameRate();

        //�ػ󵵿� �������� �ڵ����� �����
        //������ ī�޶��� viewport Rect�� ������� �����Ƿ� �������� �ҷ���
        //cam.rect = LoadCamRect();
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
        return Convert.ToBoolean(PlayerPrefs.GetInt("MouseVerticalReverse", 0));
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
        //value1 = PlayerPrefs.GetInt("ResolutionWidth", 1920);
        //value2 = PlayerPrefs.GetInt("ResolutionHeight", 1080);
        value1 = PlayerPrefs.GetInt("ResolutionWidth", Display.main.systemWidth);
        value2 = PlayerPrefs.GetInt("ResolutionHeight", Display.main.systemHeight);
    }

    //ȭ�� Ǯ��ũ�� ���� ����
    public void SaveIsWindowedScreen(bool value)
    {
        PlayerPrefs.SetInt("IsWindowedScreen", Convert.ToInt32(value));
        PlayerPrefs.Save();
    }
    public bool LoadIsWindowedScreen()
    {
        return Convert.ToBoolean(PlayerPrefs.GetInt("IsWindowedScreen", 0));
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

    //BlinkEffect StartPoint ����
    public void SaveStartPoint(float value)
    {
         PlayerPrefs.SetFloat("BlinkEffectStartPoint", value);
         PlayerPrefs.Save();
    }

    public float LoadStartPoint()
    {
        return PlayerPrefs.GetFloat("BlinkEffectStartPoint", BlinkEffect.BLINK_START_POINT_INIT);
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

    //lastApplyObject ����
    public void SaveLastApplyObject(int value)
    {
        //0 : ��Ӵٿ�
        //1 : ��ǲ�ʵ�Width
        //2 : ��ǲ�ʵ�Height
        //3 : ���콺 �巡��
        PlayerPrefs.SetInt("SaveLastApplyObject", value);
    }

    public int LoadLastApplyObject()
    {
        return PlayerPrefs.GetInt("SaveLastApplyObject", 0);
    }
    
    public void SaveDeadZoneValue(float value)
    {
        PlayerPrefs.SetFloat("DeadZoneValue", value);
        PlayerPrefs.Save();
    }
    
    public float LoadDeadZoneValue()
    {
        return PlayerPrefs.GetFloat("DeadZoneValue", 0.1f);
    }
}
