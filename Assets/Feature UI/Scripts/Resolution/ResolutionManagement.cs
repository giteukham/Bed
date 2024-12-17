using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResolutionManagement : MonoSingleton<ResolutionManagement>
{
    [SerializeField] private Camera cam;
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

    bool isWindowedScreen = true;
    bool isWindowedScreenReady = true;
    int frameRateReady = 60;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int previewMaxLength = 1000;
    float previewFontRatio = 11.25f;
    const float CRITERIA_NUM = 16f / 9f;
    //�ػ� ���� ���� �������� ���� ������Ʈ �����ϴ� �뵵
    GameObject lastApplyObject;

    List<Vector2> hdList = new List<Vector2>();

    List<Vector2> currentList = new List<Vector2>();

    List<Vector2> nowList = new List<Vector2>();
    
    public UnityEvent<bool> OnFullScreenSwitched;
    
    private Vector2[] maxResolutionToOffsets = new Vector2[2];
    
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
    
    // IsWindowedScreen, IsFullScreenReady���� get�� ��� ���µ� set�� �� �ݵ�� �� Property�� ���ؼ� ���� �����ؾ� ��
    public bool IsWindowedScreen
    {
        get => isWindowedScreen;
        private set
        {
            if (isWindowedScreen != value)
            {
                isWindowedScreen = value;
                OnFullScreenSwitched?.Invoke(isWindowedScreen);
            }
        }
    }
    
    public bool IsWindowedScreenReady
    {
        get => isWindowedScreenReady;
        private set
        {
            if (isWindowedScreenReady != value)
            {
                isWindowedScreenReady = value;
                OnFullScreenSwitched?.Invoke(isWindowedScreenReady);
            }
        }
    }
    
    public int PreviewMaxLength => previewMaxLength;
    #endregion

    private void Awake()
    {
        previewFontRatio = (previewMaxLength - 100) / previewText.fontSize;
        //����� Ǯ��ũ�� ���� �ҷ��ͼ� isWindowedScreen ������ ����
        IsWindowedScreen = SaveManager.Instance.LoadIsWindowedScreen();

        //����� �ػ� nowWidthPixel�� nowHeightPixel ������ ����
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        Vector2[] offsets = ConvertResolutionToOffsets(new Vector2Int(nowWidthPixel, nowHeightPixel));
        insideWindow.SaveOffsets(offsets[0], offsets[1]);

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
            case 3:
                lastApplyObject = insideWindow.gameObject;
                break;
        }

        hdList.Clear();
        currentList.Clear();

        //����� �ػ󵵿� �´� currentList �߰�
        float widthNum = (Display.main.systemWidth - Display.main.systemWidth / 4f) / 9f;
        float heightNum = (Display.main.systemHeight - Display.main.systemHeight / 4f) / 9f;

        for (int i = 9; i >= 0; i--)
        {
            currentList.Add(new Vector2((int)Math.Round(Display.main.systemWidth - widthNum * i), (int)Math.Round(Display.main.systemHeight - heightNum * i)));
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
        }

        maxResolutionToOffsets = ConvertResolutionToOffsets(new Vector2Int(Display.main.systemWidth, Display.main.systemHeight));
    }

    private void OnEnable()
    {
        IsWindowedScreenReady = isWindowedScreen;
        fullScreenSwitch.sprite = isWindowedScreen ? checkImage : nonCheckImage;

        //���� ����� ȭ�� �ػ󵵿� ��Ӵٿ �ִ� �ػ󵵸� ���Ͽ� �ڵ����� ���� �ػ󵵸� �����ؾ���
        //��Ӵٿ� ������ ���� ����Ʈ�� ��ü
        RedefineDropdown(isWindowedScreen);

        print($"������ ������Ʈ : {lastApplyObject.name}");

        //������ ��Ӵٿ� �̿��ؼ� ���� ��
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            for (int i = 0; i < nowList.Count; i++)
            {
                if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
                {
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
            resolutiondropdown.captionText.text = "";

            inputFieldWidth.text = $"{nowWidthPixel}";
            inputFieldHeight.text = $"{nowHeightPixel}";

            if (lastApplyObject == inputFieldWidth.gameObject)
            {
                ReadyInputField(0);
            }
            else
            {
                ReadyInputField(1);
            }
            ApplyInputField();
        }
        else if (lastApplyObject == insideWindow.gameObject)
        {
            inputFieldWidth.text = $"{nowWidthPixel}";
            inputFieldHeight.text = $"{nowHeightPixel}";
            ApplyInsideWindow();
        }
        //����� ������ ����Ʈ ����
        frameRateReady = SaveManager.Instance.LoadFrameRate();
        //������ ��Ӵٿ� ������ ����� ������ ����
        frameRateDropdown.value = frameRateReady / 30 - 1;
        
        insideWindow.OnRectTransformReSize.AddListener(RectSizeChangedHandler);

        BlinkEffect.StartPoint = SaveManager.Instance.LoadStartPoint();
    }
    
    private void OnDisable()
    {
        insideWindow.OnRectTransformReSize.RemoveListener(RectSizeChangedHandler);
    }

    private void Start()
    {
        //��ó�� ���۽� Start���� �����ؾ� ���� ����
        resolutiondropdown.captionText.text = "";
    }

    private void RedefineDropdown(bool checkWindowedScreen)
    {
        //16:9���� ���ΰ� �� �� ������� ��� 16:9 �ػ󵵸� ����
        if (CRITERIA_NUM < (float)Display.main.systemWidth / Display.main.systemHeight || checkWindowedScreen == true)
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
        else if (checkWindowedScreen == false)
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
        Screen.SetResolution((int)width, (int)height, !isWindowedScreen);

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
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
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
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
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

            float startPointConversionValue = Mathf.Lerp(BlinkEffect.BLINK_START_POINT_INIT + 0.13f, BlinkEffect.BLINK_START_POINT_INIT, (checkedValue - 1f) / (1.77f - 1f));
            BlinkEffect.StartPoint = startPointConversionValue;
        }

        //ī�޶� ��������
        cam.rect = rect;
        //camRect ����
        SaveManager.Instance.SaveCamRect(rect.x, rect.y, rect.width, rect.height);
        SaveManager.Instance.SaveStartPoint(BlinkEffect.StartPoint);
    }

    //�ػ� ��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    public void ReadyResolution(int value)
    {
        DropdownItemClick.dropdownValue = resolutiondropdown.value;

        resolutiondropdown.captionText.text = "";

        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isWindowedScreenReady == false && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
        {
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
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
            inputFieldWidth.text = $"{(int)hdList[value].x}";
            inputFieldHeight.text = $"{(int)hdList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
    }

    //Ȯ�ι�ư ������ �ػ� ����
    private void ApplyResolution()
    {
        int value = resolutiondropdown.value;

        //'��üȭ��'�̸鼭 ���ذ� 1.77 '�̸�'�϶� - currentResolutions
        if (isWindowedScreen == false && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
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
        IsWindowedScreenReady = !isWindowedScreenReady;
        fullScreenSwitch.sprite = isWindowedScreenReady ? checkImage : nonCheckImage;

        RedefineDropdown(isWindowedScreenReady);

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����(���� ū�ػ󵵷� ����)
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;

        //��Ӵٿ� ǥ�ð� ����
        resolutiondropdown.captionText.text = "";

        lastApplyObject = resolutiondropdown.gameObject;
    }

    //Ȯ�ι�ư ������ Ǯ��ũ�� ���� ����
    private void ApplyFullScreenSwitch()
    {
        IsWindowedScreen = isWindowedScreenReady;
        SaveManager.Instance.SaveIsWindowedScreen(isWindowedScreen);
    }

    //������ ��Ӵٿ� ������ Ŭ���� ȣ���(�ڵ����� ���� �ε����� �Ű������� ����)
    public void ReadyFrameRate(int value)
    {
        switch (value)
        {
            case 0:
                frameRateReady = 30;
                break;
            case 1:
                frameRateReady = 60;
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

    //�Ű����� 0�� inputFieldWidth, 1�� inputFieldHeight
    public void ReadyInputField(int num)
    {
        string result = "";
        int widthNum = 0;
        int heightNum = 0;
        int maxNum = 0;
        int minNum = 0;
        float ratio = 0f;

        if (num == 0)   //inputFieldWidth �Է½�
        {
            result = inputFieldWidth.text;

            if (int.TryParse(result, out widthNum))  //�Է°��� �������� Ȯ��
            {
                //��üȭ�� ���ο� ���� �ִ밪 ���� ����
                if (isWindowedScreenReady == false)
                {
                    maxNum = Display.main.systemWidth;
                    minNum = Display.main.systemWidth / 4;
                    ratio = (float)Display.main.systemWidth / Display.main.systemHeight;
                }
                else if (isWindowedScreenReady == true)
                {
                    //maxNum = 1920;
                    //minNum = 1920 / 4;
                    maxNum = (int)hdList[hdList.Count - 1].x;
                    minNum = (int)hdList[hdList.Count - 1].x / 4;
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
            result = inputFieldHeight.text;

            if (int.TryParse(result, out heightNum))
            {

                //��üȭ�� ���ο� ���� �ִ밪 ���� ����
                if (isWindowedScreenReady == false)
                {
                    maxNum = Display.main.systemHeight;
                    minNum = Display.main.systemHeight / 4;
                    ratio = (float)Display.main.systemWidth / Display.main.systemHeight;
                }
                else if (isWindowedScreenReady == true)
                {
                    //maxNum = 1080;
                    //minNum = 1080 / 4;
                    maxNum = (int)hdList[hdList.Count - 1].y;
                    minNum = (int)hdList[hdList.Count - 1].y / 4;

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
    }

    public void ApplyInputField()
    {
        //������ �ʴ� �� ���� �� �޼ҵ� ����
        if (inputFieldWidth.text == "ERROR" || inputFieldHeight.text == "ERROR")
        {
            return;
        }

        StartCoroutine(ResolutionWindow(float.Parse(inputFieldWidth.text), float.Parse(inputFieldHeight.text)));
        SaveManager.Instance.SaveResolution(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text));
        if (lastApplyObject == inputFieldWidth.gameObject)
        {
            SaveManager.Instance.SaveLastApplyObject(1);
        }
        else if (lastApplyObject == inputFieldHeight.gameObject)
        {
            SaveManager.Instance.SaveLastApplyObject(2);
        }
    }

    private void ApplyInsideWindow()
    {
        if (inputFieldWidth.text == "ERROR" || inputFieldHeight.text == "ERROR")
        {
            return;
        }
        StartCoroutine(ResolutionWindow(float.Parse(inputFieldWidth.text), float.Parse(inputFieldHeight.text)));
        ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
        ResizePreviewImage(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text), inside);
        UpdateResolutionText(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text));
        
        SaveManager.Instance.SaveResolution(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text));
        SaveManager.Instance.SaveLastApplyObject(3);
    }

    //Ȯ�ι�ư ���� �� �ڵ� ����
    public void PressApply()
    {
        ApplyFullScreenSwitch();
        //ApplyFullScreenSwitch ������ ApplyResolution Ȥ�� ApplyInputField ���� �� ��
        //��Ӵٿ� �� ����
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            ApplyResolution();
        }
        //��ǲ�ʵ� �� ����
        else if (lastApplyObject == inputFieldWidth.gameObject || lastApplyObject == inputFieldHeight.gameObject)
        {
            ApplyInputField();
        }
        else if (lastApplyObject == insideWindow.gameObject)
        {
            ApplyInsideWindow();
        }
        ApplyFrameRate();
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
    
    public void ResizePreviewByOffsets(Vector2 offsetMin, Vector2 offsetMax)
    {
        InsideOffsetMin = offsetMin;
        InsideOffsetMax = offsetMax;
    }
    
    /// <summary>
    /// Rect�� ũ�Ⱑ ����� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯
    /// </summary>
    /// <param name="rect"></param>
    private void RectSizeChangedHandler(RectTransform rect)
    {
        Vector2Int resolution = ConvertSizeToResolution(rect.sizeDelta);
        UpdateResolutionText(resolution.x, resolution.y);
        lastApplyObject = insideWindow.gameObject;
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

        inputFieldWidth.text = $"{width}";
        inputFieldHeight.text = $"{height}";
        
        //screenText.text = width + " " + height;
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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns>Index 0�� OffsetMin, 1�� OffsetMax</returns>
    public Vector2[] ConvertResolutionToOffsets(Vector2Int resolution)
    {
        Vector2 size = ConvertResolutionToSize(resolution);
        
        Vector2 offsetMin = new Vector2(-size.x / 2, -size.y / 2);
        Vector2 offsetMax = new Vector2(size.x / 2, size.y / 2);
        
        return new[] { offsetMin, offsetMax };
    }

    public void DoZoom()
    {
        if (!isWindowedScreenReady) return;
        
        if (insideWindow.ZoomState == ZoomState.Minimize)
        {
            insideWindow.ZoomState = ZoomState.Maximize;
            insideWindow.SaveOffsets(InsideOffsetMin, InsideOffsetMax);
            ResizePreviewByOffsets(maxResolutionToOffsets[0], maxResolutionToOffsets[1]);
        }
        else if (insideWindow.ZoomState == ZoomState.Maximize)
        {
            insideWindow.ZoomState = ZoomState.Minimize;
            ResizePreviewByOffsets(insideWindow.SavedOffsetMin, insideWindow.SavedOffsetMax);
        }
    }
    
    /// <summary>
    /// ���� Ŭ���� ���� ����/�ܾƿ�
    /// </summary>
    /// <param name="eventData"></param>
    public void DoZoom(PointerEventData eventData)
    {
        if (!isWindowedScreenReady) return;
        
        if (eventData.clickCount == 2 && insideWindow.ZoomState == ZoomState.Minimize)
        {
            insideWindow.ZoomState = ZoomState.Maximize;
            insideWindow.SaveOffsets(InsideOffsetMin, InsideOffsetMax);
            ResizePreviewByOffsets(maxResolutionToOffsets[0], maxResolutionToOffsets[1]);
        }
        else if (eventData.clickCount == 2 && insideWindow.ZoomState == ZoomState.Maximize)
        {
            insideWindow.ZoomState = ZoomState.Minimize;
            ResizePreviewByOffsets(insideWindow.SavedOffsetMin, insideWindow.SavedOffsetMax);
        }
    }
    
    public void SubscribeToInsideSizeChangedEvent(UnityAction<RectTransform> action)
    {
        insideWindow.OnRectTransformReSize.AddListener(action);
    }
    
    public void UnsubscribeFromInsideSizeChangedEvent(UnityAction<RectTransform> action)
    {
        insideWindow.OnRectTransformReSize.RemoveListener(action);
    }
    
    public Vector2Int GetLowestResolution() => new (Display.main.systemWidth / 4, Display.main.systemHeight / 4);

    //���͹ڽ� ���� ���������� ������ ��
    void OnPreCull() => GL.Clear(true, true, Color.black);


}
