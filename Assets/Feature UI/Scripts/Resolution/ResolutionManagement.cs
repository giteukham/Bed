using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using static UnityEngine.Rendering.DebugUI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Text screenText;
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

    List<Vector2> nowList = new List<Vector2>();

    [SerializeField] private TMP_Dropdown testDropdown;
    [SerializeField] private Text testText;

    private void Awake()
    {
        //����� Ǯ��ũ�� ���� �ҷ��ͼ� isFullScreen ������ ����
        isFullScreen = SaveManager.Instance.LoadIsFullScreen();
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        //����� �ػ� nowWidthPixel�� nowHeightPixel ������ ����
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //�ҷ��� ���������� �̿��� ������ ���� �ػ� ���� �ݿ�
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

        ///////////////////////////////////////////////////////////////////
        ///
        hdList.Clear();
        currentList.Clear();

        //Resolution[] resolutions = Screen.resolutions;
        List<Resolution> monitorResolutions = Screen.resolutions.ToList();

        List<string> testList = new List<string>();


        for (int i = 0; i < monitorResolutions.Count; i++)
        {
            //print($"��� {monitorResolutions[i].width} : {monitorResolutions[i].height}");
            testList.Add($"{monitorResolutions[i].width} : {monitorResolutions[i].height} : {monitorResolutions[i].refreshRate}");
        }

        testDropdown.ClearOptions();
        testDropdown.AddOptions(testList);
        dropdown.RefreshShownValue();

        float hdNum = 16f / 9f;
        float monitorNum = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
        float itemNum = 0f;
        foreach (Resolution item in monitorResolutions)
        {
            //�μ��� �ִ����� ����
            /*int a = GCD(item.width, item.height);
            int b = GCD(Display.main.systemWidth, Display.main.systemHeight);

            //���� 16:9�� �͸� ����Ʈ�� ����
            if (item.width / a == 16 && item.height / a == 9)
            {
                hdList.Add(new Vector2(item.width, item.height));
            }

            //���� ����� ������ ���� �͸� ����Ʈ�� ����
            if (item.width / a == Display.main.systemWidth / b && item.height / a == Display.main.systemHeight / b)
            {
                currentList.Add(new Vector2(item.width, item.height));
            }*/

            itemNum = (float)item.width / (float)item.height;

            if (Mathf.Approximately(hdNum, itemNum))
            {
                hdList.Add(new Vector2(item.width, item.height));
            }

            if (Mathf.Approximately(monitorNum, itemNum))
            {
                currentList.Add(new Vector2(item.width, item.height));
            }

        }



    }

    private void OnEnable()
    {
        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        //��Ӵٿ� ������ ���� ����Ʈ�� ��ü
        RedefineDropdown();

        for (int i = 0; i < nowList.Count; i++)
        {
            if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
            {
                //��Ӵٿ� ������ ����� ������ ��Ӵٿ� ������ ����
                dropdown.value = i;
                //�޼ҵ� ����
                return;
            }
        }
        //print(dropdown.options[0].text + "zfasdfasdfasdfasd");
    }

    private void Update()
    {
        //'�����'�� ���� �ػ󵵸� ������
        //screenText.text = Display.main.systemWidth + " " + Display.main.systemHeight;
        screenText.text = nowWidthPixel + " " + nowHeightPixel;
        if (nowList == hdList)
        {
            testText.text = "hdList";
        }
        else if (nowList == currentList)
        {
            testText.text = "currentList";
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            //�� �ڵ� ����� ������ ��Ӵٿ� �����ۿ� �ش��ϴ� �ػ󵵷� �����
            dropdown.value = 2;
        }
    }

    //�ִ����� ��� �޼ҵ�
    public int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private void RedefineDropdown()
    {
        float CRITERIA_NUM = 16f / 9f;

        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        /*if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            //��Ӵٿ� ������ ��� ����
            dropdown.ClearOptions();

            //��Ӵٿ �� ������ ����Ʈ ����
            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            //��Ӵٿ ������ ����Ʈ ����
            dropdown.AddOptions(temp);
            //��Ӵٿ� ���ΰ�ħ
            dropdown.RefreshShownValue();

            nowList = currentList;
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

            nowList = hdList;
        }*/

        //16:9���� ���ΰ� �� �� ������� ��� 16:9 �ػ󵵸� ����
        if (CRITERIA_NUM < Display.main.systemWidth / Display.main.systemHeight || isFullScreen == false)
        {
            dropdown.ClearOptions();

            List<string> temp = new List<string>();
            for (int i = 0; i < hdList.Count; i++)
            {
                temp.Add($"{hdList[i].x} X {hdList[i].y}");
            }

            dropdown.AddOptions(temp);
            dropdown.RefreshShownValue();

            nowList = hdList;
        }
        else if (isFullScreen == true)
        {
            //��Ӵٿ� ������ ��� ����
            dropdown.ClearOptions();

            //��Ӵٿ �� ������ ����Ʈ ����
            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            //��Ӵٿ ������ ����Ʈ ����
            dropdown.AddOptions(temp);
            //��Ӵٿ� ���ΰ�ħ
            dropdown.RefreshShownValue();

            nowList = currentList;
        }


    }

    //Ǯ��ũ������ ����� ��ư(����ġ ��ư ������ �ڵ�ȣ��)
    //Ǯ��ũ���� â��� ��Ӵٿ� �������� �ٸ��� ����ġ ������ ��Ӵٿ� �ε��� 0������ �����ϴ� �ɷ�
    public void FullScreenSwitch()
    {
        isFullScreen = !isFullScreen;
        SaveManager.Instance.SaveIsFullScreen(isFullScreen);
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        //â ũ��, Ǯ��ũ�� ���� ����
        //StartCoroutine(ResolutionWindow(nowWidthPixel, nowHeightPixel));
        //�ػ� �޴� ��� ������
        RedefineDropdown();
        //�Ƹ�ó������ 0�̶� �ٲ��ʿ��� ����ȵǴµ�
        //�׷��� �����Ϸ��� 0�� �ƴѰ��� �����ϰų�, Ȥ�� �ٸ������� ��� �ٲ۵ڿ� 0�� �ٽ� �����ؾ���
        //dropdown.value = 1;
        //dropdown.value = 0;

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����
        dropdown.value = 0;
        dropdown.value = nowList.Count - 1;
    }

    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, isFullScreen);

        yield return null;

        RescaleWindow(width, height);

        SaveManager.Instance.SaveResolution((int)width, (int)height);
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
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
        }


    }
}
