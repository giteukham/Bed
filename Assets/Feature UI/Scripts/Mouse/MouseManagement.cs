using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MouseManagement : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image verticalSwitch;
    [SerializeField] private Image horizontalSwitch;

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

    /// <summary>
    /// �������� �ƴ� ui ��Ҹ� �ʱ�ȭ ��(������ �ʱ�ȭ�� SaveManager���� ������)
    /// </summary>
    private void OnEnable()
    {
        //�����̴� �����ٿ� �������ӿ� ����ƴ� ������ ����
        slider.value = SaveManager.Instance.LoadMouseSensitivity();
        //���콺 ������ ���� ��ư �̹��� ����
        verticalSwitch.sprite = SaveManager.Instance.LoadMouseVerticalReverse() ? offImage : onImage;
        horizontalSwitch.sprite = SaveManager.Instance.LoadMouseHorizontalReverse() ? onImage : offImage;
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
        SaveManager.Instance.SaveMouseSensitivity(mouseSensitivity);
    }

    /// <summary>
    /// ���콺 ���Ϲ���
    /// </summary>
    public void MouseVerticalReverse()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput = !virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput;
        SaveManager.Instance.SaveMouseVerticalReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InvertInput);
        //��ư �̹��� ����(true�� ��� ����, false�� ��� �ʷ�)
        verticalSwitch.sprite = SaveManager.Instance.LoadMouseVerticalReverse() ? offImage : onImage;
    }

    /// <summary>
    /// ���콺 �¿� ����
    /// </summary>
    public void MouseHorizontalReverse()
    {
        virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput = !virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput;
        SaveManager.Instance.SaveMouseHorizontalReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput);

        //InputSystem.xBodyReverse = virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput ? -1 : 1;
        SaveManager.Instance.SaveXBodyReverse(virtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InvertInput ? -1 : 1);
        //��ư �̹��� ����(false�� ��� ����, true�� ��� �ʷ�)
        horizontalSwitch.sprite = SaveManager.Instance.LoadMouseHorizontalReverse() ? onImage : offImage;
    }


}
