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
    /// ���콺 ���� ���� ����
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// ���� ����Ǵ� ���콺 ����
    /// </summary>
    public static float mouseSpeed = 500f;
    /// <summary>
    /// ���콺 ����
    /// </summary>
    public static int mouseReverse = 1;

    private void Awake()
    {
        //�����̴� �����ٿ� �������ӿ� ����ƴ� ������ ����
        slider.value = saveManager.LoadMouseSensitivity();
    }

    //���콺 ����â ��Ȱ��ȭ�� �ȵ��ư�
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
    /// �����̴� ������ ���� �ڵ� ȣ��
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * 500f * mouseReverse;
        saveManager.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseReverse()
    {
        mouseReverse *= -1;
        saveManager.SaveMouseReverse(mouseReverse);
        print("���콺 ������ : " + mouseReverse + ", ���� : " + mouseSpeed);
    }


}
