using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using static UnityEngine.Rendering.DebugUI;

public class ResolutionManagement : MonoSingleton<ResolutionManagement>
{
    [SerializeField] private Camera cam;
    [SerializeField] private Text screenText;
    [SerializeField] private TMP_Text previewText;
    [SerializeField] private Image fullScreenSwitch;
    [SerializeField] private Image insideImage;
    [SerializeField] private TMP_InputField inputFieldWidth;
    [SerializeField] private TMP_InputField inputFieldHeight;
    [SerializeField] private TMP_Dropdown resolutiondropdown;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private RectTransform outside;
    [SerializeField] private RectTransform inside;

    [SerializeField] private Sprite checkImage;
    [SerializeField] private Sprite nonCheckImage;
    [SerializeField] private Sprite fullscreenInside;
    [SerializeField] private Sprite windowedInside;
    
    [SerializeField] private InsideWindow insideWindow;

    bool isFullScreen = true;
    bool isFullScreenReady = true;
    int frameRateReady = 60;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int previewMaxLength = 1000;
    float previewFontRatio = 11.25f;
    const float CRITERIA_NUM = 16f / 9f;
    //�ػ� ���� ���� �������� ���� ������Ʈ �����ϴ� �뵵
    GameObject lastApplyObject;

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
    
    public UnityEvent<bool> OnFullScreenSwitched;
    
    #region Properties
    public RectTransform InsideRectTransform => inside;
    public Vector2 InsideAnchoredPosition
    {
        get => inside.anchoredPosition;
        set => inside.anchoredPosition = value;
    }

    public Vector2 InsideSize
    {
        get => inside.sizeDelta;
        set => inside.sizeDelta = value;
    }

    public Vector2 InsideOffsetMin
    {
        get => inside.offsetMin;
        set => inside.offsetMin = value;
    }

    public Vector2 InsideOffsetMax
    {
        get => inside.offsetMax;
        set => inside.offsetMax = value;
    }
    
    public RectTransform OutsideRectTransform => outside;
    public Vector2 OutsideAnchoredPosition => outside.anchoredPosition;
    public Vector2 OutsideSize => outside.sizeDelta;
    public Vector2 OutsideOffsetMin => outside.offsetMin;
    public Vector2 OutsideOffsetMax => outside.offsetMax;
    
    public bool IsFullScreen
    {
        get => isFullScreen;
        set
        {
            if (isFullScreen != value)
            {
                isFullScreen = value;
                OnFullScreenSwitched?.Invoke(isFullScreen);
            }
        }
    }
    
    #endregion

    private void Awake()
    {
        previewFontRatio = (previewMaxLength - 100) / previewText.fontSize;
        //����� Ǯ��ũ�� ���� �ҷ��ͼ� isFullScreen ������ ����
        IsFullScreen = SaveManager.Instance.LoadIsFullScreen();
        /*isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;*/

        //����� �ػ� nowWidthPixel�� nowHeightPixel ������ ����
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //�ҷ��� ���������� �̿��� ������ ���� �ػ� ���� �ݿ�
        //Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        //print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        switch (SaveManager.Instance.LoadLastApplyObject())
        {
            case 0:
                lastApplyObject = resolutiondropdown.gameObject;
                break;
            case 1:
                lastApplyObject = inputFieldWidth.gameObject;
                break;
            case 2:
                lastApplyObject = inputFieldHeight.gameObject;
                break;
        }

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
            //print("currentList : " + (int)Math.Round(Display.main.systemWidth - widthNum * i) + " : " + (int)Math.Round(Display.main.systemHeight - heightNum * i));
        }

        if ((float)Display.main.systemWidth / Display.main.systemHeight > 16f / 9f)
        {
            widthNum = Display.main.systemHeight / 9f * 16;
            heightNum = Display.main.systemHeight;
        }
        else if ((float)Display.main.systemWidth / Display.main.systemHeight <= 16f / 9f)
        {
            widthNum = Display.main.systemWidth;
            heightNum = Display.main.systemWidth / 16f * 9;
        }

        float num1 = (widthNum - widthNum / 4f) / 9f;
        float num2 = (heightNum - heightNum / 4f) / 9f;

        for (int i = 9; i >= 0; i--)
        {
            hdList.Add(new Vector2((int)Math.Round(widthNum - num1 * i), (int)Math.Round(heightNum - num2 * i)));
            //print("hdList : " + (int)Math.Round(widthNum - num1 * i) + " : " + (int)Math.Round(heightNum - num2 * i));
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
    }

    private void OnEnable()
    {
        insideWindow.OnRectTransformReSize.AddListener(OnSizeChangedHandler);
        
        isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;
        //inside.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        //��Ӵٿ� ������ ���� ����Ʈ�� ��ü
        RedefineDropdown(isFullScreen);

        print($"������ ������Ʈ : {lastApplyObject.name}");

        //������ ��Ӵٿ� �̿��ؼ� ���� ��
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
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
        }
        //������ ��ǲ �ʵ�� ������
        else if (lastApplyObject == inputFieldWidth.gameObject || lastApplyObject == inputFieldHeight.gameObject)
        {
            print("�������");
            //�׷��� ������ ��Ӵٿ� �ؽ�Ʈ�� ���� ���� ������
            //�׷��� ���� ������� inputFieldWidth.text�� �׻� �ػ󵵰� �ؽ�Ʈ ǥ���ؾ���
            //��ó�� ��� �̰� �������
            print(resolutiondropdown.captionText.text);
            resolutiondropdown.captionText.text = "";
            print(resolutiondropdown.captionText.text);
            //��ǲ�ʵ忡 ����� �� �ҷ����� ��ǲ�ʵ� �ػ� �����ָ� �ɵ�(���� ���Ѿ���)
            //inputFieldWidth.text = $"{nowWidthPixel} X {nowHeightPixel}";
            inputFieldWidth.text = $"{nowWidthPixel}";
            inputFieldHeight.text = $"{nowHeightPixel}";
            //OldReadyInputField();
            if (lastApplyObject == inputFieldWidth.gameObject)
            {
                NewReadyInputField(0);
            }
            else
            {
                NewReadyInputField(1);
            }
            ApplyInputField();
        }

        //���� ������� �׻� ��Ӵٿ� �ؽ�Ʈ �����
        //resolutiondropdown.captionText.text = "";
        //���� ������� �׻� inputField�� �ػ� ǥ��
        //inputFieldWidth.text = $"{nowWidthPixel} X {nowHeightPixel}";

        // V-Sync ��Ȱ��ȭ
        //QualitySettings.vSyncCount = 0;
        //����� ������ ����Ʈ ����
        //Application.targetFrameRate = SaveManager.Instance.LoadFrameRate();
        frameRateReady = SaveManager.Instance.LoadFrameRate();
        //������ ��Ӵٿ� ������ ����� ������ ����
        frameRateDropdown.value = frameRateReady / 30 - 1;
    }
    
    private void OnDisable()
    {
        insideWindow.OnRectTransformReSize.RemoveListener(OnSizeChangedHandler);
    }

    private void Start()
    {
        //��ó�� ���۽� Start���� �����ؾ� ���� ����
        resolutiondropdown.captionText.text = "";
    }

    /// <summary>
    /// 11-30 �ֹ��� ���� : �ػ� �ؽ�Ʈ �ٲٴ� �� UpdateResolutionText�� �̵�
    /// </summary>
    private void Update()
    {
        //'�����'�� ���� �ػ󵵸� ������
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
        //CRITERIA_NUM = 16f / 9f;

        //16:9���� ���ΰ� �� �� ������� ��� 16:9 �ػ󵵸� ����
        if (CRITERIA_NUM < (float)Display.main.systemWidth / Display.main.systemHeight || checkFullScreen == false)
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
        //CRITERIA_NUM = 16f / 9f;

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
    /// <summary>
    /// 11-30 �ֹ��� ���� : �ػ� �ؽ�Ʈ �ٲٴ� �� UpdateResolutionText�� �̵�
    /// </summary>
    /// <param name="value"></param>
    public void ReadyResolution(int value)
    {
        print("ReadyResolution ����");
        resolutiondropdown.captionText.text = "";
        //CRITERIA_NUM = 16f / 9f;

        //����Ʈ������ �Ű����� �޾Ƽ� �־��ָ� �ɵ�
        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isFullScreenReady == true && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
        {
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
            //inputFieldWidth.text = $"{(int)currentList[value].x} X {(int)currentList[value].y}";
            inputFieldWidth.text = $"{(int)currentList[value].x}";
            inputFieldHeight.text = $"{(int)currentList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            //inputFieldWidth.text = $"{(int)hdList[value].x} X {(int)hdList[value].y}";
            inputFieldWidth.text = $"{(int)hdList[value].x}";
            inputFieldHeight.text = $"{(int)hdList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
    }

    //Ȯ�ι�ư ������ �ػ� ����
    private void ApplyResolution()
    {
        int value = resolutiondropdown.value;
        //CRITERIA_NUM = 16f / 9f;

        //����Ʈ������ �Ű����� �޾Ƽ� �־��ָ� �ɵ�
        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
        {
            StartCoroutine(ResolutionWindow(currentList[value].x, currentList[value].y));
            SaveManager.Instance.SaveLastApplyObject(0);
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
            SaveManager.Instance.SaveLastApplyObject(0);
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
        //inside.sprite = isFullScreen ? fullscreenInside : windowedInside;

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

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����(���� ū�ػ󵵷� ����)
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;

        //��Ӵٿ� ǥ�ð� ����
        resolutiondropdown.captionText.text = "";

        lastApplyObject = resolutiondropdown.gameObject;
        //ApplyResolution();
    }

    //Ȯ�ι�ư ������ Ǯ��ũ�� ���� ����
    private void ApplyFullScreenSwitch()
    {
        //isFullScreen = !isFullScreen;
        IsFullScreen = isFullScreenReady;
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
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            previewText.text = $"{(int)nowList[resolutiondropdown.value].x} X {(int)nowList[resolutiondropdown.value].y}\n{frameRateReady}hz";
        }
        else
        {
            string[] parts = previewText.text.Split('\n')[0].Split(" X ", StringSplitOptions.None);
            previewText.text = $"{parts[0].Trim()} X {parts[1].Trim()}\n{frameRateReady}hz";
        }
    }

    //Ȯ�ι�ư ������ ������ ����
    private void ApplyFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRateReady;
        SaveManager.Instance.SaveFrameRate(frameRateReady);
    }

    //���� �Ⱦ�
    public void OldReadyInputField()
    {
        //�̰��ϸ� ������
        //inputFieldWidth.text = $"{nowWidthPixel} X {nowHeightPixel}";
        char[] chars = inputFieldWidth.text.ToCharArray();
        char[] separators = { 'X', ' ' };
        int widthNum = 0;
        int heightNum = 0;

        for (int i = 0; i < chars.Length; i++)
        {
            //��ǲ�ʵ忡 ���� Ȥ�� X, x �ִ��� Ȯ��
            if (chars[i] ==' ' || chars[i] == 'X' || chars[i] == 'x')
            {
                //����� X �������� split
                string[] result = inputFieldWidth.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                //���� "1920 " Ȥ�� "1920X"�� ��� result�� ���� ���ڰ� �ϳ��ۿ� ������ ���
                if (result.Length < 2)
                {
                    inputFieldWidth.text = "ERROR";
                    return;
                }

                //split�� ���� ���ڰ� �´��� Ȯ��
                if (int.TryParse(result[0], out widthNum) && int.TryParse(result[1], out heightNum))
                {
                    //â������� ��üȭ������ Ȯ��
                    if (isFullScreenReady == true)  //��üȭ��
                    {
                        if (widthNum > Display.main.systemWidth)    //���� ����Ͱ� ����ϴ� �ִ��ػ� ������
                        {
                            widthNum = Display.main.systemWidth;
                            heightNum = Display.main.systemHeight;
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";
                        }
                        else if (Mathf.Approximately(widthNum / (float)heightNum, (float)Display.main.systemWidth / Display.main.systemHeight)) //���� ����� ������ �Ȱ����� ����
                        {
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";
                        }
                        else    //���� ����Ϳ� ���� �ٸ��� ���� �ٽ� ���ؼ� ����
                        {
                            /*widthNum = Display.main.systemWidth;
                            heightNum = Display.main.systemHeight;
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";*/

                            //���ΰ� �ٽ� ����
                            //heightNum = Mathf.CeilToInt(widthNum / ((float)Display.main.systemWidth / Display.main.systemHeight));
                            heightNum = (int)Mathf.Round(widthNum / ((float)Display.main.systemWidth / Display.main.systemHeight)); //�ݿø� ������� ����
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";
                        }
                    }
                    else    //â���
                    {
                        if (widthNum > hdList[hdList.Count - 1].x)    //���� ����Ͱ� ����ϴ� 16:9 �ִ��ػ� ������
                        {
                            widthNum = (int)hdList[hdList.Count - 1].x;
                            heightNum = (int)hdList[hdList.Count - 1].y;
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";
                        }
                        else if (Mathf.Approximately(widthNum / (float)heightNum, CRITERIA_NUM)) //���� ����� ������ �Ȱ����� ����
                        {
                            print($"�������� : {widthNum} X {heightNum}");
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";
                        }
                        else    //16:9 ���� �ƴҽ� ���ΰ� �������� 16:9 �� ã��
                        {
                            //���ΰ� �ٽ� ����
                            print($"���ΰ�1 : {widthNum / CRITERIA_NUM}");
                            //heightNum = Mathf.CeilToInt(widthNum / CRITERIA_NUM);
                            heightNum = (int)Mathf.Round(widthNum / CRITERIA_NUM);  //�ݿø� ������� ����
                            print($"���ΰ�2 : {heightNum}");
                            inputFieldWidth.text = $"{widthNum} X {heightNum}";
                        }
                    }
                    ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
                    ResizePreviewImage(widthNum, heightNum, inside);
                    previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
                    previewText.text = $"{widthNum} X {heightNum}\n{frameRateReady}hz";

                    lastApplyObject = inputFieldWidth.gameObject;
                    return;
                }
                else
                {
                    inputFieldWidth.text = "ERROR";
                    return;
                }
            }
        }

        //�Է°��� X Ȥ�� ������ �� ���� ���� ��
        inputFieldWidth.text = "ERROR";
        return;
    }

    //�Ű����� 0�� inputFieldWidth, 1�� inputFieldHeight
    public void NewReadyInputField(int num)
    {
        //string�� now���� �Ἥ �ؽ�Ʈ ����ְ� �������� �Ǵ�
        //���ڰ� �´ٸ� now������ �ݴ�Ǵ� �ؽ�Ʈ�� ���� �������
        //���� ����

        string result = "";
        int widthNum = 0;
        int heightNum = 0;
        int maxNum = 0;
        int minNum = 0;
        float ratio = 0f;

        if (num == 0)   //inputFieldWidth �Է½�
        {
            //lastApplyObject = inputFieldWidth.gameObject;
            result = inputFieldWidth.text;

            if (int.TryParse(result, out widthNum))  //�Է°��� �������� Ȯ��
            {
                //��üȭ�� ���ο� ���� �ִ밪 ���� ����
                if (isFullScreenReady == true)
                {
                    maxNum = Display.main.systemWidth;
                    minNum = Display.main.systemWidth / 5;
                    ratio = (float)Display.main.systemWidth / Display.main.systemHeight;
                }
                else if (isFullScreenReady == false)
                {
                    maxNum = 1920;
                    minNum = 1920 / 5;
                    ratio = CRITERIA_NUM;
                }

                //�Է°��� �ּҰ� �ִ밪 ���� �ʰ� ����
                widthNum = Mathf.Clamp(widthNum, minNum, maxNum);
                inputFieldWidth.text = $"{widthNum}";

                heightNum = (int)Mathf.Round(widthNum / ratio);
                inputFieldHeight.text = $"{heightNum}";

                lastApplyObject = inputFieldWidth.gameObject;
            }
            else //���� �ƴҽ� ����
            {
                inputFieldWidth.text = "ERROR";
                inputFieldHeight.text = "ERROR";
                return;
            }
        }
        else if (num == 1)  //inputFieldHeight �Է½�
        {
            //lastApplyObject = inputFieldHeight.gameObject;
            result = inputFieldHeight.text;

            if (int.TryParse(result, out heightNum))
            {

                //��üȭ�� ���ο� ���� �ִ밪 ���� ����
                if (isFullScreenReady == true)
                {
                    maxNum = Display.main.systemHeight;
                    minNum = Display.main.systemHeight / 5;
                    ratio = (float)Display.main.systemWidth / Display.main.systemHeight;
                }
                else if (isFullScreenReady == false)
                {
                    maxNum = 1080;
                    minNum = 1080 / 5;
                    ratio = CRITERIA_NUM;
                }

                //�Է°��� �ּҰ� �ִ밪 ���� �ʰ� ����
                heightNum = Mathf.Clamp(heightNum, minNum, maxNum);
                inputFieldHeight.text = $"{heightNum}";

                widthNum = (int)Mathf.Round(heightNum * ratio);
                inputFieldWidth.text = $"{widthNum}";

                lastApplyObject = inputFieldHeight.gameObject;
            }
            else    //���� �ƴҽ� ����(���ڰ� �ʹ� �� ������)
            {
                inputFieldWidth.text = "ERROR";
                inputFieldHeight.text = "ERROR";
                return;
            }
        }

        ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
        ResizePreviewImage(widthNum, heightNum, inside);
        previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
        previewText.text = $"{widthNum} X {heightNum}\n{frameRateReady}hz";

        print($"������ ������Ʈ {lastApplyObject.name} ������");

        //�̰� ��� ���� �����ϱ�(���� �� if���� �־���)
        //lastApplyObject = inputFieldWidth.gameObject;
    }

    public void ApplyInputField()
    {
        //������ �ʴ� �� ���� �� �޼ҵ� ����
        if (inputFieldWidth.text == "ERROR" || inputFieldHeight.text == "ERROR")
        {
            print("��ǲ�ʵ� ����");
            return;
        }

        /*char[] separators = { 'X', ' ' };
        string[] result = inputFieldWidth.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        StartCoroutine(ResolutionWindow(float.Parse(result[0]), float.Parse(result[1])));
        SaveManager.Instance.SaveResolution(int.Parse(result[0]), int.Parse(result[1]));
        SaveManager.Instance.SaveLastApplyObject(1);*/

        StartCoroutine(ResolutionWindow(float.Parse(inputFieldWidth.text), float.Parse(inputFieldHeight.text)));
        SaveManager.Instance.SaveResolution(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text));
        print($"ApplyInputField ���� ���� lastApplyObject�� = {lastApplyObject.name}");
        print($"lastApplyObject : inputFieldWidth = {lastApplyObject} : {inputFieldWidth} ��� : {lastApplyObject == inputFieldWidth}");
        if (lastApplyObject == inputFieldWidth.gameObject)
        {
            SaveManager.Instance.SaveLastApplyObject(1);
            print("��ǲ�ʵ� ��������1");
        }
        else if (lastApplyObject == inputFieldHeight.gameObject)
        {
            SaveManager.Instance.SaveLastApplyObject(2);
            print("��ǲ�ʵ� ��������2");
        }
    }

    //Ȯ�ι�ư ���� �� �ڵ� ����
    public void PressApply()
    {
        ApplyFullScreenSwitch();
        //ApplyFullScreenSwitch ������ ApplyResolution Ȥ�� ApplyInputField ���� �� ��
        //��Ӵٿ� �� ����
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            print("��Ӵٿ� ����");
            ApplyResolution();
        }
        //��ǲ�ʵ� �� ����
        else
        {
            print("��ǲ�ʵ� ����");
            ApplyInputField();
        }
        ApplyFrameRate();
        print($"������ ������Ʈ {lastApplyObject.name} ����Ϸ�");
    }

    //������ ȭ�� ������ ���� �޼ҵ�
    private void ResizePreviewImage(float targetWidth, float targetHeight, RectTransform rect)
    {
        float ratio1 = 0;
        float ratio2 = 0;
        
        if (rect == outside)
        {
            if (targetWidth >= targetHeight)
            {
                //��ǥ �ػ󵵴� 1 : ratio�� ǥ�� ������
                ratio1 = 1 / (targetWidth / targetHeight);
                rect.sizeDelta = new Vector2(previewMaxLength, previewMaxLength * ratio1);
            }
            else
            {
                ratio1 = 1 / (targetHeight / targetWidth);
                rect.sizeDelta = new Vector2(previewMaxLength * ratio1, previewMaxLength);
            }
        }
        else //rect == inside
        {
            ratio1 = 1 / (Display.main.systemWidth / targetWidth);
            ratio2 = 1 / (Display.main.systemHeight / targetHeight);
            print($"1 / {Display.main.systemWidth} / {targetWidth} = {1 / (Display.main.systemWidth / targetWidth)}");
        
            InsideSize = new Vector2(outside.rect.width * ratio1, outside.rect.height * ratio2);
            rect.anchoredPosition = Vector2.zero;
            
            //�ٱ��׵θ��� �׻� �ణ �� ũ�� �׸�
            outside.sizeDelta = new Vector2(outside.rect.width + 50, outside.rect.height + 50);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns>Index 0�� OffsetMin, 1�� OffsetMax</returns>
    public Vector2[] ConvertResolutionToOffsets(float width, float height)
    {
        float ratio = width / height;
        Vector2 sizeDelta = new Vector2(previewMaxLength, previewMaxLength / ratio);

        Vector2 offsetMin = new Vector2(-sizeDelta.x / 2, -sizeDelta.y / 2);
        Vector2 offsetMax = new Vector2(sizeDelta.x / 2, sizeDelta.y / 2);
        return new[] { offsetMin, offsetMax };
    }

    /// <summary>
    /// Rect�� ũ�Ⱑ ����� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    /// </summary>
    /// <param name="rect"></param>
    private void OnSizeChangedHandler(RectTransform rect)
    {
        Vector2Int resolution = ConvertSizeToResolution(rect.sizeDelta);
        UpdateResolutionText(resolution.x, resolution.y);
    }

    /// <summary>
    /// Rect�� ũ�Ⱑ ����Ǹ� �ػ� �ؽ�Ʈ�� ������Ʈ
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private void UpdateResolutionText(int width, int height)
    {
        previewText.text = $"{width} X {height}\n{frameRateReady}hz";
        previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;

        screenText.text = width + " " + height;
    }
    
    public Vector2Int ConvertSizeToResolution(Vector2 sizeDelta)
    {
        int width = Mathf.RoundToInt((float) Display.main.systemWidth / previewMaxLength * sizeDelta.x);
        int height = Mathf.RoundToInt((float) Display.main.systemWidth / previewMaxLength * sizeDelta.y);
        return new Vector2Int(width, height);
    }
    
    public Vector2 ConvertResolutionToSize(Vector2Int resolution)
    {
        float width = (float) resolution.x / Display.main.systemWidth * previewMaxLength;
        float height = (float) resolution.y / Display.main.systemWidth * previewMaxLength;
        return new Vector2(width, height);
    }
    
    public Vector2Int GetLowestResolution() => new (Display.main.systemWidth / 4, Display.main.systemHeight / 4);


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
