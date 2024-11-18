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
    [SerializeField] private Image insideImage;
    [SerializeField] private TMP_Dropdown resolutiondropdown;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private RectTransform outside;
    [SerializeField] private RectTransform inside;

    [SerializeField] private Sprite onImage;
    [SerializeField] private Sprite offImage;
    [SerializeField] private Sprite fullscreenInside;
    [SerializeField] private Sprite windowedInside;

    bool isFullScreen = true;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int frameRate = 60;

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

    [SerializeField] private Text testText;
    [SerializeField] private Text testText2;

    private void Awake()
    {
        // V-Sync ��Ȱ��ȭ
        QualitySettings.vSyncCount = 0;
        //����� ������ ����Ʈ ����
        Application.targetFrameRate = SaveManager.Instance.LoadFrameRate();
        //������ ��Ӵٿ� ������ ����� ������ ����
        frameRateDropdown.value = SaveManager.Instance.LoadFrameRate() / 30 - 1;

        //����� Ǯ��ũ�� ���� �ҷ��ͼ� isFullScreen ������ ����
        isFullScreen = SaveManager.Instance.LoadIsFullScreen();
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //����� �ػ� nowWidthPixel�� nowHeightPixel ������ ����
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //�ҷ��� ���������� �̿��� ������ ���� �ػ� ���� �ݿ�
        //Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        //////////////////////////////////////////////////////////////////////////
        
        hdList.Clear();
        currentList.Clear();

        List<Resolution> monitorResolutions = Screen.resolutions.ToList();

        float hdNum = 16f / 9f;
        float monitorNum = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
        float itemNum = 0f;
        foreach (Resolution item in monitorResolutions)
        {
            itemNum = (float)item.width / (float)item.height;

            if (Mathf.Approximately(hdNum, itemNum))
            {
                ListValueDuplicateCheck(hdList, item);
            }

            if (Mathf.Approximately(monitorNum, itemNum))
            {
                ListValueDuplicateCheck(currentList, item);
            }

        }



    }

    private void OnEnable()
    {
        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        //��Ӵٿ� ������ ���� ����Ʈ�� ��ü
        RedefineDropdown();
        print(nowList.Count);
        for (int i = 0; i < nowList.Count; i++)
        {
            if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
            {
                print(nowWidthPixel + " : " + nowList[i].x + " : " + nowHeightPixel + " : " + nowList[i].y);
                //0�϶� 0���� ��Ӵٿ� value�� �ٲ㵵 ��ȭ�� ���� ������ �������� �޼ҵ� ��������
                if (i == 0)
                {
                    resolutiondropdown.value = i;
                    EnterResolution(i);
                }
                //��Ӵٿ� ������ ����� ������ ��Ӵٿ� ������ ����
                resolutiondropdown.value = i;
                //�޼ҵ� ����
                return;
            }
        }
        //print(resolutiondropdown.options[0].text + "zfasdfasdfasdfasd");
    }

    private void Update()
    {
        //'�����'�� ���� �ػ󵵸� ������
        //screenText.text = Display.main.systemWidth + " " + Display.main.systemHeight;
        screenText.text = nowWidthPixel + " " + nowHeightPixel;
        testText2.text = Application.targetFrameRate + "";
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
            resolutiondropdown.value = 2;
        }
    }

    private void ListValueDuplicateCheck(List<Vector2> list, Resolution item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //������ ������ ���� �ʰ� for�� ����
            if (list[i].x == item.width)
            {
                return;
            }
        }
        //����Ʈ �ȿ� ������ ������ ����Ʈ�� ����
        list.Add(new Vector2(item.width, item.height));
    }

    private void RedefineDropdown()
    {
        print("�������� ��Ӵٿ�");
        float CRITERIA_NUM = 16f / 9f;

        //16:9���� ���ΰ� �� �� ������� ��� 16:9 �ػ󵵸� ����
        if (CRITERIA_NUM < Display.main.systemWidth / Display.main.systemHeight || isFullScreen == false)
        {
            resolutiondropdown.ClearOptions();

            List<string> temp = new List<string>();
            for (int i = 0; i < hdList.Count; i++)
            {
                temp.Add($"{hdList[i].x} X {hdList[i].y}");
            }

            resolutiondropdown.AddOptions(temp);
            resolutiondropdown.RefreshShownValue();

            nowList = hdList;
        }
        else if (isFullScreen == true)
        {
            //��Ӵٿ� ������ ��� ����
            resolutiondropdown.ClearOptions();

            //��Ӵٿ �� ������ ����Ʈ ����
            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            //��Ӵٿ ������ ����Ʈ ����
            resolutiondropdown.AddOptions(temp);
            //��Ӵٿ� ���ΰ�ħ
            resolutiondropdown.RefreshShownValue();

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
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //�ػ� �޴� ��� ������
        RedefineDropdown();

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;
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

    //�ػ� ��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    public void EnterResolution(int value)
    {
        print("���� ���ַ��");
        float CRITERIA_NUM = 16f / 9f;

        //����Ʈ������ �Ű����� �޾Ƽ� �־��ָ� �ɵ�
        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            StartCoroutine(ResolutionWindow(currentList[value].x, currentList[value].y));
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage2(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage2((int)currentList[value].x, (int)currentList[value].y, inside);
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage2(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage2((int)hdList[value].x, (int)hdList[value].y, inside);
        }
    }

    private void ResizePreviewImage2(float targetWidth, float targetHeight, RectTransform rect)
    {
        float ratio = 0;
        if (rect == outside)
        {
            if (targetWidth >= targetHeight)
            {
                //��ǥ �ػ󵵴� 1 : ratio�� ǥ�� ������
                ratio = 1 / (targetWidth / targetHeight);
                rect.sizeDelta = new Vector2(1000, 1000 * ratio);
            }
            else
            {
                ratio = 1 / (targetHeight / targetWidth);
                rect.sizeDelta = new Vector2(1000 * ratio, 1000);
            }
        }
        else //rect == inside
        {
            if (targetWidth >= targetHeight)
            {
                //�ٱ� �׵θ��� ���� �׵θ��� ��� ���̳����� ������ 1���� �� ���� ����
                ratio = 1 / (Display.main.systemWidth / targetWidth);
            }
            else
            {
                //�ٱ� �׵θ��� ���� �׵θ��� ��� ���̳����� ������ 1���� �� ���� ����
                ratio = 1 / (Display.main.systemHeight / targetHeight);
            }
            //scaleDifference�� �ٱ� �׵θ��� ���ο� ���ο� ���ؼ� ������
            if (Display.main.systemWidth == targetWidth)
            {
                //��ǥ �ػ󵵰� �� ����� ũ��� ������ ����,�ٱ��� �׵θ��� ���ĺ��̱⿡ -50 ������ ����
                rect.sizeDelta = new Vector2(outside.rect.width * ratio - 50, outside.rect.height * ratio - 50);
            }
            else
            {
                rect.sizeDelta = new Vector2(outside.rect.width * ratio, outside.rect.height * ratio);
            }
        }
    }

    private void ResizePreviewImage(int width, int height, RectTransform rect)
    {
        //���ο� ������ �ִ����� ����
        int gcd = GCD(width, height);
        //�� ���ο� ���θ� �ִ������� ������ �ػ��� ������ �˾Ƴ�
        width = width / gcd;
        height = height / gcd;

        //������ ���ؾ��ϴ� ���ذ�
        int temp = 0;
        //�ڽ��� ���ΰ� �ִ� �θ��� ���� Ȥ�� ����
        int parentWidth = 0;
        int parentHeight = 0;

        //�ڽ��� ���ΰ� �ִ� ������Ʈ�� ����, ���� ���̸� ����
        //�θ� �г��� ��
        if (rect == outside)
        {
            parentWidth = 1000;
            parentHeight = 1000;
        }
        //�θ� outside�� ��
        else
        {
            parentWidth = (int)outside.rect.width;
            parentHeight = (int)outside.rect.height;
        }

        //��ǥ �ػ� ���ΰ� ���� ��ġ �̻��϶�(���η� ��ų� 1:1 ������ ȭ���̶�� ��)
        if (width >= height)
        {
            //�θ� ������ �ִ��� �� ���� ä�� �� �ִ� ���ذ��� ����
            temp = parentWidth / width;
            //���ΰ��� ��� �� ������ ���ذ� ��������
            while (temp * height > parentHeight)
            {
                temp--;
            }
        }
        //��ǥ �ػ� ���ΰ� ���� ��ġ �̸��϶�(���ΰ� ������ ȭ���̶�� ��)
        else
        {
            //�θ� ������ �ִ��� �� ���� ä�� �� �ִ� ���ذ��� ����
            temp = parentHeight / height;
            //���ΰ��� ��� �� ������ ���ذ� ��������
            while (temp * height > parentWidth)
            {
                temp--;
            }
        }

        if (rect == inside)
        {
            print("temp : " + temp);
        }

        //������ ���ذ��� ���Ͽ� �θ� ������ �ִ��� �� ä�� width�� height�� ����
        width *= temp;
        height *= temp;

        if (rect == inside)
        {
            print("targetWidth : " + width);
            print("targetHeight : " + height);
        }

        if (rect == outside)
        {
            rect.sizeDelta = new Vector2(width, height);
        }
        else if (rect == inside)
        {
            //inside�� �׵θ��� outside�� ��ĥ ��� �����Ƿ� 50�� ����
            rect.sizeDelta = new Vector2(width - 50, height - 50);
        }
    }

    private int GCD(int a, int b)
    {
        if (a == b)
        {
            return a;
        }

        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    //������ ��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    public void EnterFrameRate(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                SaveManager.Instance.SaveFrameRate(30);
                break;
            case 1:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
                SaveManager.Instance.SaveFrameRate(60);
                break;
        }
    }
}
