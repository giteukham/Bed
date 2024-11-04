using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// playerPrefs�� �̿��� ���� �ý���
/// </summary>
public class SaveManager : MonoBehaviour
{
    //���� ���۰� ���ÿ� ���� ������
    private void Awake()
    {
        MouseManagement.mouseSensitivity = LoadMouseSensitivity();
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
