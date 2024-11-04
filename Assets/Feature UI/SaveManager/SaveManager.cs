using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// playerPrefs를 이용한 저장 시스템
/// </summary>
public class SaveManager : MonoBehaviour
{
    //게임 시작과 동시에 설정 가져옴
    private void Awake()
    {
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
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

    public void SaveMouseReverse(int value)
    {
        PlayerPrefs.SetInt("MouseReverse", value);
        PlayerPrefs.Save();
    }
    public float LoadMouseReverse()
    {
        return PlayerPrefs.GetInt("MouseReverse", 1);
    }
}
