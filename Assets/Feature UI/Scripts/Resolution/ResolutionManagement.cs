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
    [SerializeField] private TMP_Text previewText;
    [SerializeField] private Image fullScreenSwitch;
    [SerializeField] private Image insideImage;
    [SerializeField] private TMP_Dropdown resolutiondropdown;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private SpriteRenderer outside;
    [SerializeField] private SpriteRenderer inside;

    [SerializeField] private Sprite checkImage;
    [SerializeField] private Sprite nonCheckImage;
    [SerializeField] private Sprite fullscreenInside;
    [SerializeField] private Sprite windowedInside;

    bool isFullScreen = true;
    bool isFullScreenReady = true;
    int frameRateReady = 60;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int previewMaxLength = 1000;
    float previewFontRatio = 11.25f;

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
        previewFontRatio = (previewMaxLength - 100) / previewText.fontSize;
        //����� Ǯ��ũ�� ���� �ҷ��ͼ� isFullScreen ������ ����
        isFullScreen = SaveManager.Instance.LoadIsFullScreen();
        /*isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;*/

        //����� �ػ� nowWidthPixel�� nowHeightPixel ������ ����
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //�ҷ��� ���������� �̿��� ������ ���� �ػ� ���� �ݿ�
        //Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        //print(Display.main.systemWidth + " : " + Display.main.systemHeight);



        //////////////////////////////////////////////////////////////////////////
        
        hdList.Clear();
        currentList.Clear();

        //����� �ػ� ����,������ �ִ����� ����
        /*int gcd = GCD(Display.main.systemWidth, Display.main.systemHeight);
        //�� ������� ���� ���� �ڿ� 20�� ������
        int value1 = Display.main.systemWidth / gcd * 20;
        int value2 = Display.main.systemHeight / gcd * 20;
        //��Ӵٿ �� �ּҰ��� ����
        int startWidth = Display.main.systemWidth / 4;
        int startHeight = Display.main.systemHeight / 4;*/

        //����� �ػ󵵿� �´� currentList �߰�
        /*while (true)
        {
            currentList.Add(new Vector2(startWidth, startHeight));
            startWidth += value1;
            startHeight += value2;
            if (startWidth >= Display.main.systemWidth && startHeight >= Display.main.systemHeight)
            {
                currentList.Add(new Vector2(Display.main.systemWidth, Display.main.systemHeight));
                break;
            }
        }*/

        //����� �ػ󵵿� �´� currentList �߰�
        float widthNum = (Display.main.systemWidth - Display.main.systemWidth / 4f) / 9f;
        float heightNum = (Display.main.systemHeight - Display.main.systemHeight / 4f) / 9f;

        for (int i = 9; i >= 0; i--)
        {
            currentList.Add(new Vector2((int)Math.Round(Display.main.systemWidth - widthNum * i), (int)Math.Round(Display.main.systemHeight - heightNum * i)));
            print("currentList : " + (int)Math.Round(Display.main.systemWidth - widthNum * i) + " : " + (int)Math.Round(Display.main.systemHeight - heightNum * i));
        }

        if (Display.main.systemWidth / Display.main.systemHeight > 16f / 9f)
        {
            widthNum = Display.main.systemHeight / 9f * 16;
            heightNum = Display.main.systemHeight;
        }
        else if (Display.main.systemWidth / Display.main.systemHeight <= 16f / 9f)
        {
            widthNum = Display.main.systemWidth;
            heightNum = Display.main.systemWidth / 16f * 9;
        }

        float num1 = (widthNum - widthNum / 4f) / 9f;
        float num2 = (heightNum - heightNum / 4f) / 9f;

        for (int i = 9; i >= 0; i--)
        {
            hdList.Add(new Vector2((int)Math.Round(widthNum - num1 * i), (int)Math.Round(heightNum - num2 * i)));
            print("hdList : " + (int)Math.Round(widthNum - num1 * i) + " : " + (int)Math.Round(heightNum - num2 * i));
        }

        //Screen.resolutions�� ������� ���� ���� �ػ󵵸� ��Ƴ��� ����(���� �� ����Ϳ� ���� ����)
        /*List<Resolution> monitorResolutions = Screen.resolutions.ToList();

        float hdNum = 16f / 9f;
        float monitorNum = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
        float itemNum = 0f;

        foreach (Resolution item in monitorResolutions)
        {
            itemNum = (float)item.width / (float)item.height;

            if (Mathf.Approximately(hdNum, itemNum) && Display.main.systemWidth >= item.width && Display.main.systemHeight >= item.height)
            {
                ListValueDuplicateCheck(hdList, item);
            }

            //�� ����� ������ �ٻ簪�ΰ�? && item �ػ󵵰� �� ����� ���� �ػ��ΰ�?
            if (Mathf.Approximately(monitorNum, itemNum) && Display.main.systemWidth >= item.width && Display.main.systemHeight >= item.height)
            {
                ListValueDuplicateCheck(currentList, item);
            }
        }*/

        for (int i = 0; i < currentList.Count; i++)
        {
            //print($"Ŀ��Ʈ ����Ʈ : {currentList[i].x} : {currentList[i].y}");
        }

    }

    private void OnEnable()
    {
        isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;
        inside.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        //��Ӵٿ� ������ ���� ����Ʈ�� ��ü
        RedefineDropdown(isFullScreen);
        for (int i = 0; i < nowList.Count; i++)
        {
            if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
            {
                //print(nowWidthPixel + " : " + nowList[i].x + " : " + nowHeightPixel + " : " + nowList[i].y);

                //��Ӵٿ� ������ ����� ������ ��Ӵٿ� ������ ����
                resolutiondropdown.value = i;
                //������ ����
                ReadyResolution(i);
                //�ػ� ����
                ApplyResolution();
                //�ݺ��� ����
                break;
            }
        }

        // V-Sync ��Ȱ��ȭ
        //QualitySettings.vSyncCount = 0;
        //����� ������ ����Ʈ ����
        //Application.targetFrameRate = SaveManager.Instance.LoadFrameRate();
        frameRateReady = SaveManager.Instance.LoadFrameRate();
        //������ ��Ӵٿ� ������ ����� ������ ����
        frameRateDropdown.value = frameRateReady / 30 - 1;
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
    }

    private void ListValueDuplicateCheck(List<Vector2> list, Resolution item)
    {
        //temp�⺻���� list ������ �ε���
        //int temp = list.Count;

        for (int i = 0; i < list.Count; i++)
        {
            //������ ������ ���� �ʰ� �޼ҵ� ����
            if (list[i].x == item.width && list[i].y == item.height)
            {
                return;
            }
            //�Ʒ� else if���� ��Ӵٿ� ����Ʈ�� ���� ���ں��� ���۵ȴٴ� �����Ͽ� �۵���
            //����Ʈ ���κ��� ������ ���ΰ� ª����
            else if (list[i].x > item.width)
            {
                //temp = i;
                print($"{item.width}�� {list[i].x} ���� ª��");
                list.Insert(i, new Vector2(item.width, item.height));
                return;
            }
            else if (list[i].x == item.width && list[i].y > item.height)
            {
                //temp = i;
                list.Insert(i, new Vector2(item.width, item.height));
                return;
            }
        }
        //����Ʈ �ȿ� ������ �����鼭 ���� ū width�� ������ ������ ����Ʈ �������� ����
        list.Add(new Vector2(item.width, item.height));
        //list.Insert(temp, new Vector2(item.width, item.height));
    }

    private void RedefineDropdown(bool checkFullScreen)
    {
        float CRITERIA_NUM = 16f / 9f;

        //16:9���� ���ΰ� �� �� ������� ��� 16:9 �ػ󵵸� ����
        if (CRITERIA_NUM < Display.main.systemWidth / Display.main.systemHeight || checkFullScreen == false)
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
        else if (checkFullScreen == true)
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

    //�ػ� ���� �޼ҵ�
    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, isFullScreen);

        //SetResolution�ϰ� �ٷ� RescaleWindow �����ϸ� ���� ����� �ȵǴ� ���� �־ 1������ ������ ����
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
        //print("�������� ������");
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
            //print("16 : 9");
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
        //camRect ����
        SaveManager.Instance.SaveCamRect(rect.x, rect.y, rect.width, rect.height);
    }

    //�ػ� ��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    //�ػ� �����丸 �ǵ帲
    public void ReadyResolution(int value)
    {
        print("ReadyResolution ����");
        float CRITERIA_NUM = 16f / 9f;

        //����Ʈ������ �Ű����� �޾Ƽ� �־��ָ� �ɵ�
        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isFullScreenReady == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
            //previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
            previewText.fontSize = inside.size.x * previewFontRatio;
            previewText.text = $"{(int)currentList[value].x} X {(int)currentList[value].y}\n{frameRateReady}hz";
            //previewText.rectTransform.sizeDelta = Vector2();
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            //previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
            previewText.fontSize = inside.size.x * previewFontRatio;
            previewText.text = $"{(int)hdList[value].x} X {(int)hdList[value].y}\n{frameRateReady}hz";
        }
    }

    //Ȯ�ι�ư ������ �ػ� ����
    private void ApplyResolution()
    {
        int value = resolutiondropdown.value;
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

    //�ػ� ������, üũ��ư ��縸 �ǵ帲(üũ ��ư ������ �ڵ�ȣ��)
    public void ReadyFullScreenSwitch()
    {
        //isFullScreen = !isFullScreen;
        //SaveManager.Instance.SaveIsFullScreen(isFullScreen);
        //fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        isFullScreenReady = !isFullScreenReady;
        fullScreenSwitch.sprite = isFullScreenReady ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreenReady ? fullscreenInside : windowedInside;
        inside.sprite = isFullScreen ? fullscreenInside : windowedInside;

        /*if (fullScreenSwitch.sprite == checkImage)
        {
            fullScreenSwitch.sprite = nonCheckImage;
            insideImage.sprite = windowedInside;
        }
        else
        {
            fullScreenSwitch.sprite = checkImage;
            insideImage.sprite = fullscreenInside;
        }*/

        //�ػ� �޴� ��� ������
        //isFullScreenReady = !isFullScreenReady;
        RedefineDropdown(isFullScreenReady);

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;
        //ApplyResolution();
    }

    //Ȯ�ι�ư ������ Ǯ��ũ�� ���� ����
    private void ApplyFullScreenSwitch()
    {
        //isFullScreen = !isFullScreen;
        isFullScreen = isFullScreenReady;
        SaveManager.Instance.SaveIsFullScreen(isFullScreen);
        //fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //�ػ� �޴� ��� ������
        //RedefineDropdown(!isFullScreen);

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����
        //resolutiondropdown.value = nowList.Count - 1;
        //�̹� ApplyResolution���� ȣ����
        //ApplyResolution();
    }

    //������ ��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    public void ReadyFrameRate(int value)
    {
        switch (value)
        {
            case 0:
                frameRateReady = 30;
                //QualitySettings.vSyncCount = 0;
                //Application.targetFrameRate = 30;
                //SaveManager.Instance.SaveFrameRate(30);
                break;
            case 1:
                frameRateReady = 60;
                //QualitySettings.vSyncCount = 0;
                //Application.targetFrameRate = 60;
                //SaveManager.Instance.SaveFrameRate(60);
                break;
        }
        previewText.text = $"{(int)nowList[resolutiondropdown.value].x} X {(int)nowList[resolutiondropdown.value].y}\n{frameRateReady}hz";
    }

    //Ȯ�ι�ư ������ ������ ����
    private void ApplyFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRateReady;
        SaveManager.Instance.SaveFrameRate(frameRateReady);
    }

    //Ȯ�ι�ư ���� �� �ڵ� ����
    public void PressApply()
    {
        ApplyFullScreenSwitch();
        //ApplyFullScreenSwitch ������ ApplyResolution ���� �� ��
        ApplyResolution();
        ApplyFrameRate();
    }

    //������ ȭ�� ������ ���� �޼ҵ�
    private void ResizePreviewImage(float targetWidth, float targetHeight, SpriteRenderer rect)
    {
        float ratio1 = 0;
        float ratio2 = 0;
        //2560 1080���� 1920 1080 �ػ� ��ȯ ����
        if (rect == outside)
        {
            if (targetWidth >= targetHeight)
            {
                //��ǥ �ػ󵵴� 1 : ratio�� ǥ�� ������
                ratio1 = 1 / (targetWidth / targetHeight);
                //rect.sizeDelta = new Vector2(previewMaxLength, previewMaxLength * ratio1);
                //1000�� 1�� ���� �ִ� ������ �� �츮 ���� ���̴� ���� ���� ���δ� ������ ����
                //1 * 1000 : ratio1 * 1000
                //������ rect.size�� x,y ǥ�ô� ������ ����
                //1 : ratio1
                rect.size = new Vector2(1, ratio1);
            }
            else
            {
                ratio1 = 1 / (targetHeight / targetWidth);
                //rect.sizeDelta = new Vector2(previewMaxLength * ratio1, previewMaxLength);
                rect.size = new Vector2(ratio1, 1);
            }
        }
        else //rect == inside
        {
            ratio1 = 1 / (Display.main.systemWidth / targetWidth);
            ratio2 = 1 / (Display.main.systemHeight / targetHeight);
            print($"1 / {Display.main.systemWidth} / {targetWidth} = {1 / (Display.main.systemWidth / targetWidth)}");

            //rect.sizeDelta = new Vector2(outside.rect.width * ratio1, outside.rect.height * ratio2);
            //�ٱ��׵θ��� �׻� �ణ �� ũ�� �׸�
            //outside.sizeDelta = new Vector2(outside.rect.width + 50, outside.rect.height + 50);

            ratio1 = 1 / (targetWidth / targetHeight);
            ratio2 = 1 / (targetHeight / targetWidth);

            //inside ������ ��� �� ���ΰ�
            //inside�� ũ��� �׻� outside�� �������� �ϸ� outside���� �۰ų� ���ƾ���
            //�׷��Ƿ� inside.size�� x�� y�� outside.size�� x�� y�� ���� ���� �� ����
            //���⼭ ������ ����
            //scale�� �׻� 1000, 1000 �������� �����Ǿ�߸� ��
            //�̷��� �Ǹ� inside�� �׻� 1000, 1000�� ������ ������, outside�� �������� �󸶳� ������ ���Ѵٰ��ص� ���ǹ̰� ���� ����
            //��� 1 : ��ǥ �ػ� ���ΰ� �� ��ٰ� ������
            //ratio1 = 1 / (targetWidth / targetHeight); �� �ڵ带 ���ְ� �Ǹ� 1000, 1000�� ���� ����(��Ȯ���� ����)�� ���� �� ����, 1 : ratio1�� �Ǵ°��� ���� outside���� ���̰� ����
            //���� �ؾ��Ұ��� outside���� ������ �۰� �����ϴ� ����, ���� ����� ���ؼ� 1�� ratio1�� ������ �ؾ��ϴ°�?
            //�ϴ� �������� ��Ģ�������� ���� �Ұ� ����.
            //�� ����� ũ��� ��ǥ �ػ� �簢���� ��Ȯ�� ������� �˾Ƴ� ����� ����
            //rect.size = new Vector2(ratio1 / (Display.main.systemWidth / targetWidth), ratio2 / (Display.main.systemHeight / targetHeight));
            rect.size = new Vector2(1, ratio1);
            //�ٱ��׵θ��� �׻� �ణ �� ũ�� �׸�(�ӽ��ּ�**************)
            //outside.size = new Vector2(outside.size.x * 1.05f, outside.size.y * 1.05f);
        }
    }

    private int GCD(int a, int b)
    {
        if (a == b)
        {
            return a;
        }
        else if (a > b)
        {
            while (b != 0)
            {
                int temp = a % b;
                a = b;
                b = temp;
            }
            return a;
        }
        else
        {
            while (a != 0)
            {
                int temp = b % a;
                b = a;
                a = temp;
            }
            return b;
        }
    }

    //���͹ڽ� ���� ���������� ������ ��
    void OnPreCull() => GL.Clear(true, true, Color.black);


}
