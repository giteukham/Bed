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
    /// �÷��̾��� ����� ī�޶� ����
    /// </summary>
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    /// <summary>
    /// �ʷ� ����ġ �̹���
    /// </summary>
    [SerializeField] private Sprite onImage;
    /// <summary>
    /// ���� ����ġ �̹���
    /// </summary>
    [SerializeField] private Sprite offImage;


    //�⺻�� ����
    //���� 1f, ���Ϲ��� true(1), �¿���� false(0)

    /// <summary>
    /// ���콺 ���� ���� ����
    /// </summary>
    public static float mouseSensitivity = 1f;
    /// <summary>
    /// ���� ����Ǵ� ���콺 ����
    /// </summary>
    public static float mouseSpeed = 500f;

    private void OnEnable()
    {
        //�����̴� �����ٿ� �������ӿ� ����ƴ� ������ ����
        slider.value = saveManager.LoadMouseSensitivity();
        //���콺 ������ ���� ��ư �̹��� ����
        verticalSwitch.sprite = saveManager.LoadMouseVerticalReverse() ? offImage : onImage;
        horizontalSwitch.sprite = saveManager.LoadMouseHorizontalReverse() ? onImage : offImage;
    }

    //���콺 ����â ��Ȱ��ȭ�� ������Ʈ ���� �ȵ��ư�
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
    /// �����̴� ������ ���� �ڵ� ȣ��
    /// </summary>
    public void ChangeSensitivity()
    {
        mouseSensitivity = slider.value;
        mouseSpeed = mouseSensitivity * 500f;
        saveManager.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = !virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput;
        saveManager.SaveMouseVerticalReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput);
        //��ư �̹��� ����(true�� ��� ����, false�� ��� �ʷ�)
        verticalSwitch.sprite = saveManager.LoadMouseVerticalReverse() ? offImage : onImage;
    }

    /// <summary>
    /// ���콺 �¿� ����
    /// </summary>
    public void MouseHorizontalReverse()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput = !virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput;
        saveManager.SaveMouseHorizontalReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput);
        //��ư �̹��� ����(false�� ��� ����, true�� ��� �ʷ�)
        horizontalSwitch.sprite = saveManager.LoadMouseHorizontalReverse() ? onImage : offImage;
    }


}
