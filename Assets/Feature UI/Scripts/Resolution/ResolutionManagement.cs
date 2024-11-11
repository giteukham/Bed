using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using static UnityEngine.Rendering.DebugUI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Text screenText;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Image fullScreenSwitch;
    [SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private Sprite onImage;
    [SerializeField] private Sprite offImage;

    bool isFullScreen = true;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;

    List<Vector2> hdList = new List<Vector2>
    {
        new Vector2(1280, 720),
        new Vector2(1600, 900),
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    List<Vector2> currentList = new List<Vector2>();

    private void Awake()
    {
        //����� Ǯ��ũ�� ���� �ҷ��ͼ� ����
        isFullScreen = saveManager.LoadIsFullScreen();
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        saveManager.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        int temp = 480;
        for (int i = 0; i < 5; i++)
        {
            //����� ���� �ȼ� / ����� ���� �ȼ��� 1.77���� ���� ��� ������ ��Ӵٿ� ����Ʈ ����
            currentList.Add(new Vector2(Display.main.systemWidth - temp, Display.main.systemHeight - temp));

            if (i == 2)
            {
                temp -= 60;
            }
            else if (i == 3)
            {
                temp -= 180;
            }
            else
            {
                temp -= 120;
            }
        }

        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        RedefineResolutionMenu();
        //��� ��Ӵٿ �ִ� �ػ� text�� ������ ���ΰ�?

    }

    private void OnEnable()
    {
        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        RedefineResolutionMenu();
        //��� ��Ӵٿ �ִ� �ػ� text�� ������ ���ΰ�?
        for (int i = 0; i < 5; i++)
        {

        }
        print(dropdown.options[0].text);

        print("���ξ��̺�");
    }

    private void Update()
    {
        //'�����'�� ���� �ػ󵵸� ������
        //screenText.text = Display.main.systemWidth + " " + Display.main.systemHeight;
        screenText.text = nowWidthPixel + " " + nowHeightPixel;
    }

    private void RedefineResolutionMenu()
    {
        float CRITERIA_NUM = 16f / 9f;

        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            dropdown.ClearOptions();

            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            dropdown.AddOptions(temp);
            dropdown.RefreshShownValue();
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            dropdown.ClearOptions();

            List<string> temp = new List<string>();
            for (int i = 0; i < hdList.Count; i++)
            {
                temp.Add($"{hdList[i].x} X {hdList[i].y}");
            }

            dropdown.AddOptions(temp);
            dropdown.RefreshShownValue();
        }
    }

    //Ǯ��ũ������ ����� ��ư(����ġ ��ư ������ �ڵ�ȣ��)
    public void FullScreenSwitch()
    {
        isFullScreen = !isFullScreen;
        saveManager.SaveIsFullScreen(isFullScreen);
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        //â ũ��, Ǯ��ũ�� ���� ����
        StartCoroutine(ResolutionWindow(nowWidthPixel, nowHeightPixel));
        //�ػ� �޴� ��� ������
        RedefineResolutionMenu();
    }

    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, isFullScreen);

        yield return null;

        RescaleWindow(width, height);

        saveManager.SaveResolution((int)width, (int)height);
        nowWidthPixel = (int)width;
        nowHeightPixel = (int)height;
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
        float CRITERIA_NUM = 16f / 9f;

        //����Ʈ������ �Ű����� �޾Ƽ� �־��ָ� �ɵ�
        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            StartCoroutine(ResolutionWindow(currentList[value].x, currentList[value].y));
            print("��");
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
            print("�Ʒ�");
        }


    }
}
