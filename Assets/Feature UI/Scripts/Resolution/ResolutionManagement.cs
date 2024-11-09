using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] Text screenText;

    int temp = 0;

    bool change = true;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;


    private void Awake()
    {
        // 1920 x 1080���� ����
        nowWidthPixel = 1920;
        nowHeightPixel = 1080;
        Screen.SetResolution(nowWidthPixel, nowHeightPixel, change);
        //screenText.text = nowWidthPixel + " x " + nowHeightPixel;

    }

    private void Update()
    {
        //'�����'�� ���� �ػ󵵸� ������
        screenText.text = Display.main.systemWidth + " " + Display.main.systemHeight;

        if (Input.GetKeyDown(KeyCode.M))
        {
            Screen.SetResolution(100, 100, change);
            change = !change;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("����");

            switch (temp)
            {
                case 0:
                    temp++;
                    nowWidthPixel = 1440;
                    nowHeightPixel = 1080;
                    break;

                case 1:
                    temp++;
                    nowWidthPixel = 1920;
                    nowHeightPixel = 1200;
                    break;

                case 2:
                    temp++;
                    nowWidthPixel = 1920;
                    nowHeightPixel = 1080;
                    break;

                case 3:
                    temp++;
                    nowWidthPixel = 2560;
                    nowHeightPixel = 1080;
                    break;

                case 4:
                    temp = 0;
                    nowWidthPixel = 3840;
                    nowHeightPixel = 1080;
                    change = !change;
                    break;
            }
            StartCoroutine(ResolutionWindow(nowWidthPixel, nowHeightPixel));
        }

    }

    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, change);

        yield return null;

        RescaleWindow(width, height);
        yield break;
    }

    /// <summary>
    /// ȭ�� ũ�� ���� �޼ҵ�
    /// </summary>
    /// <param name="width">'��ǥ' ���κ����� �����</param>
    /// <param name="height">'��ǥ' ���κ����� �����</param>
    private void RescaleWindow(float width, float height)
    {
        GL.Clear(true, true, Color.black);  // ȭ���� ���������� ����

        //���� Screen.SetResolution(3840, 1080, change);�� ����ƴٸ�
        //width�� 3840, height�� 1080��
        //float width = Screen.width;
        //float height = Screen.height;

        //16 / 9���� ���Ͽ� 16:9 ȭ�鿡�� ���α��� Ȥ�� ���α��� �߿���
        //��� ���̰� �� ���� �˾Ƴ��� �Ǻ��� ����
        float checkedValue = width / height;
        float CRITERIA_NUM = 16f / 9f;

        Rect rect = new Rect(0, 0, 1, 1);
        float temp1 = 0;
        //���� ������ 9�� ���� �� ����� ��ǥ ���� ����
        float temp2 = 0;

        //��ǥ ������ 16 : 9�϶�
        if (checkedValue == CRITERIA_NUM)
        {
            cam.rect = rect;
            print("16 : 9");
            return;
        }
        //��ǥ ������ '���� ����'�� 16 : 9���� Ŭ��
        else if (checkedValue > CRITERIA_NUM)
        {
            //��ǥ ���� * X = 9�� ������ ���� X���� ����
            temp1 = 9 / height;
            //������ X�� '��ǥ' ������ ���θ� ���Ͽ� '����' ���κ����� ����
            temp2 = temp1 * width;

            //������ '����'������ ������ ����  (temp2 : 9)

            //16 / '����' ���κ����� ���� rect.width�� ����
            rect.width = 16 / temp2;

            //1 - rect.width�� �� �� 2�� ���� ���� rect.x�� ����(�и� ȭ�� �߾����� �̵�)
            rect.x = (1 - rect.width) / 2;
        }
        //��ǥ ������ '���� ����'�� 16 : 9���� Ŭ��
        else if (checkedValue < CRITERIA_NUM)
        {
            //��ǥ ���� * X = 9�� ������ �� X���� ����
            temp1 = 9 / height;
            //������ X�� '��ǥ' ������ ���θ� ���Ͽ� '����' ���κ����� ����
            temp2 = temp1 * width;

            //������ '����'������ ������ ����  (temp2 : 9)

            //'����' ���κ��� / 16�� ���� rect.height�� ����
            rect.height = temp2 / 16;

            //1 - rect.height�� �� �� 2�� ���� ���� rect.y�� ����(�и� ȭ�� �߾����� �̵�)
            rect.y = (1 - rect.height) / 2;
        }

        //ī�޶� ��������
        cam.rect = rect;
    }

    //���͹ڽ� ���� ���������� ������ ��
    void OnPreCull() => GL.Clear(true, true, Color.black);

    //��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    public void EnterResolution(int value)
    {
        switch (value)
        {
            case 0:
                StartCoroutine(ResolutionWindow(1440, 1080));
                break;
            case 1:
                StartCoroutine(ResolutionWindow(1920, 1200));
                break;
            case 2:
                StartCoroutine(ResolutionWindow(1920, 1080));
                break;
            case 3:
                StartCoroutine(ResolutionWindow(2560, 1080));
                break;
            case 4:
                StartCoroutine(ResolutionWindow(3840, 1080));
                break;
            default:
                break;
        }

        //StartCoroutine(ResolutionWindow(float.Parse(nums[0]), float.Parse(nums[1])));
    }
}
