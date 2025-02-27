using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResolutionManagement : MonoSingleton<ResolutionManagement>
{
    [SerializeField] private Camera cam;
    [SerializeField] private TMP_Text previewText;
    [SerializeField] private SwitchButton_Resolution fullScreenSwitch;
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

    [SerializeField] private Player player;

    bool isWindowedScreen = false;
    bool isWindowedScreenReady = false;
    int frameRate = 60;
    int frameRateReady = 60;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int previewMaxLength = 1000;
    float previewFontRatio = 11.25f;
    const float CRITERIA_NUM = 16f / 9f;
    //�ػ� ���� ���� �������� ���� ������Ʈ �����ϴ� �뵵
    GameObject lastApplyObject;
    ResolutionData savedData, readyData;

    List<Vector2> hdList = new List<Vector2>();

    List<Vector2> currentList = new List<Vector2>();

    List<Vector2> nowList = new List<Vector2>();

    List<string> frameList = new List<string>
    {
            "30",
            "60"
    };

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
        
    }

    private void OnEnable()
    {
        //â��� ���� �ʱ�ȭ
        IsWindowedScreenReady = isWindowedScreen;
        fullScreenSwitch.SwitchLoadDataApply(isWindowedScreen);

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

        //��ǲ�ʵ� �ؽ�Ʈ ���� �ʱ�ȭ
        //inputFieldWidth.text = $"{nowWidthPixel}";
        //inputFieldHeight.text = $"{nowHeightPixel}";

        //������ ��Ӵٿ�, ������ ������ �ؽ�Ʈ ���� �ʱ�ȭ
        frameRateDropdown.value = frameRate / 30 - 1;

        //�ػ� ���� �ؽ�Ʈ �ʱ�ȭ(��ǲ�ʵ�, ������ �ػ� �ؽ�Ʈ)
        UpdateResolutionText(nowWidthPixel, nowHeightPixel);

        //������ outside, inside ũ�� �ʱ�ȭ
        ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
        //ResizePreviewImage(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text), inside);
        ResizePreviewImage(nowWidthPixel, nowHeightPixel, inside);
        
        savedData = new ResolutionData(nowWidthPixel, nowHeightPixel, frameRate, isWindowedScreen);
        readyData = new ResolutionData(nowWidthPixel, nowHeightPixel, frameRate, isWindowedScreen);

        insideWindow.OnRectTransformReSize.AddListener(RectSizeChangedHandler);
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
                //temp.Add($"{hdList[i].x} X {hdList[i].y}");

                if (hdList[i].x < 1000)
                {
                    temp.Add($"{new string('\u200A', 6)}{hdList[i].x} X {hdList[i].y}");
                }
                else
                {
                    temp.Add($"{hdList[i].x} X {hdList[i].y}");
                }
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
                //temp.Add($"{currentList[i].x} X {currentList[i].y}");

                if (hdList[i].x < 1000)
                {
                    temp.Add($"{new string('\u200A', 6)}{currentList[i].x} X {currentList[i].y}");
                }
                else
                {
                    temp.Add($"{currentList[i].x} X {currentList[i].y}");
                }
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

        //SaveManager.Instance.SaveResolution((int)width, (int)height);
        nowWidthPixel = (int)width;
        nowHeightPixel = (int)height;
        //player.pixelationFactor = 0.25f / (nowWidthPixel / 1920f);
        //SaveManager.Instance.SavePixelationFactor(player.pixelationFactor);
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
            UpdateResolutionText((int)currentList[value].x, (int)currentList[value].y);
            // inputFieldWidth.text = $"{(int)currentList[value].x}";
            // inputFieldHeight.text = $"{(int)currentList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
        //'â���'�̰ų�, '��üȭ��'�̸鼭 ���ذ� 1.77 '�̻�'�϶� - hdResolutions
        else
        {
            //�׻� �ƿ����̵� ���� �������
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            UpdateResolutionText((int)hdList[value].x, (int)hdList[value].y);
            // inputFieldWidth.text = $"{(int)hdList[value].x}";
            // inputFieldHeight.text = $"{(int)hdList[value].y}";
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
        fullScreenSwitch.OnSwitchButtonClicked(isWindowedScreenReady);

        RedefineDropdown(isWindowedScreenReady);

        //������ ��Ӵٿ��� ���� ��Ӵٿ�� ��ȣ�� ������ ��� 0���� �ٲ��ص� �ٸ� �ε��� ����(���� ū�ػ󵵷� ����)
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;

        //��Ӵٿ� ǥ�ð� ����
        resolutiondropdown.captionText.text = "";

        lastApplyObject = resolutiondropdown.gameObject;
        
        readyData.isWindowedScreen = isWindowedScreenReady;
        ToggleAsteriskOnPreviewText(!savedData.Equals(readyData));
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
        
        readyData.frameRate = frameRateReady;
        ToggleAsteriskOnPreviewText(!savedData.Equals(readyData));
    }

    //Ȯ�ι�ư ������ ������ ����
    private void ApplyFrameRate()
    {
        frameRate = frameRateReady;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
        SaveManager.Instance.SaveFrameRate(frameRate);
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
                    //16:9���� ū ������ ������ ������ ������ ���� 16:9�� ����
                    ratio = (float)Display.main.systemWidth / Display.main.systemHeight > CRITERIA_NUM ? CRITERIA_NUM : (float)Display.main.systemWidth / Display.main.systemHeight;
                    //16:9�� ������ ������ �ִٸ� ����
                    if (ratio == CRITERIA_NUM)
                    {
                        int tempHeight = (int)(Display.main.systemWidth * 9 / 16.0f);
                        int tempWidth = (int)(Display.main.systemHeight * 16 / 9.0f);
                        //16:9�� ���� ���� ���̰� ���� ����ͺ��� ������ �˻�
                        maxNum = tempHeight > Display.main.systemHeight ? tempWidth : Display.main.systemWidth;
                        minNum = maxNum / 4;
                    }
                    //16:9 �������� ������
                    else
                    {
                        maxNum = Display.main.systemWidth;
                        minNum = Display.main.systemWidth / 4;
                    }
                    //maxNum = ratio != CRITERIA_NUM ? (int)(Display.main.systemHeight * CRITERIA_NUM) : Display.main.systemWidth;
                    print($"��� :{(float)Display.main.systemWidth / Display.main.systemHeight} : {ratio}");
                }
                else if (isWindowedScreenReady == true)
                {
                    //maxNum = 1920;
                    //minNum = 1920 / 4;
                    ratio = CRITERIA_NUM;
                    maxNum = (int)hdList[hdList.Count - 1].x;
                    minNum = (int)hdList[hdList.Count - 1].x / 4;
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
                    ratio = (float)Display.main.systemWidth / Display.main.systemHeight > CRITERIA_NUM ? CRITERIA_NUM : (float)Display.main.systemWidth / Display.main.systemHeight;
                    //16:9�� ������ ������ �ִٸ� ����
                    if (ratio == CRITERIA_NUM)
                    {
                        int tempHeight = (int)(Display.main.systemWidth * 9 / 16.0f);
                        int tempWidth = (int)(Display.main.systemHeight * 16 / 9.0f);
                        //16:9�� ���� ���� �ʺ� ���� ����ͺ��� ������ �˻�
                        maxNum = tempWidth > Display.main.systemWidth ? tempHeight : Display.main.systemHeight;
                        minNum = maxNum / 4;
                    }
                    //16:9 �������� ������
                    else
                    {
                        maxNum = Display.main.systemHeight;
                        minNum = Display.main.systemHeight / 4;
                    }

                }
                else if (isWindowedScreenReady == true)
                {
                    //maxNum = 1080;
                    //minNum = 1080 / 4;
                    ratio = CRITERIA_NUM;
                    maxNum = (int)hdList[hdList.Count - 1].y;
                    minNum = (int)hdList[hdList.Count - 1].y / 4;
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

        if (widthNum == readyData.width || heightNum == readyData.height) return;

        ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
        ResizePreviewImage(widthNum, heightNum, inside);
        //previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
        //previewText.text = $"{widthNum} X {heightNum}\n{frameRateReady}hz";
        readyData.width = widthNum;
        readyData.height = heightNum;
        UpdateResolutionText(widthNum, heightNum);
        //ToggleAsteriskOnPreviewText(!savedData.Equals(readyData));
    }

    public void ApplyInputField()
    {
        //������ �ʴ� �� ���� �� �޼ҵ� ����
        if (inputFieldWidth.text == "ERROR" || inputFieldHeight.text == "ERROR")
        {
            return;
        }

        StartCoroutine(ResolutionWindow(float.Parse(inputFieldWidth.text), float.Parse(inputFieldHeight.text)));
        //SaveManager.Instance.SaveResolution(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text));
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
        
        //SaveManager.Instance.SaveResolution(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text));
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

        savedData = readyData;
        ToggleAsteriskOnPreviewText(false);
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
            if (isWindowedScreenReady == false) //��üȭ���϶� ũ�� ����
            {
                ratio1 = 1;
                ratio2 = 1;
                //rect ��ȭ ���� ������ �ؽ�Ʈ ����, ũ�� ���� ���� ����
            }
            else
            {
                ratio1 = 1 / (Display.main.systemWidth / targetWidth);
                ratio2 = 1 / (Display.main.systemHeight / targetHeight);
            }
            
            previewText.text = $"{inputFieldWidth.text} X {inputFieldHeight.text}\n{frameRateReady}hz";
            previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;

            inputFieldWidth.text = $"{inputFieldWidth.text}";
            inputFieldHeight.text = $"{inputFieldHeight.text}";
        
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
        if (isWindowedScreenReady == false)
        {
            return;
        }
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
        
        readyData.width = width;
        readyData.height = height;
        ToggleAsteriskOnPreviewText(!savedData.Equals(readyData));
        
        //screenText.text = width + " " + height;
    }

    private void ToggleAsteriskOnPreviewText(bool isOn)
    {
        StringBuilder sb = new StringBuilder(previewText.text);
        
        if (isOn && !sb.ToString().Contains("*"))
        {
            sb.Replace("\n", "*\n");
        }
        else if (!isOn && sb.ToString().Contains("*"))
        {
            sb.Replace("*\n", "\n");
        }
        previewText.text = sb.ToString();
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
    public Vector2[] ConvertResolutionToOffsets(Vector2Int resolution, bool flag)
    {
        print($"���ַ�� : {resolution.x} {resolution.y}");
        //16 : 9���� ������ Ŭ�� && flag�� true�϶��� if�� ����
        if (CRITERIA_NUM < (float)Display.main.systemWidth / Display.main.systemHeight && flag)
        {
            //print($"��� : {Display.main.systemWidth} : {Display.main.systemHeight}");
            //resolution�� �ִ��ػ󵵿��� ���尡��� 16:9 ������ �� �־�� ��
            //���ΰ� ���κ��� �� �������� ������� ���(������ ���ΰ� �� �� ����͵��� ������ 16:9���� ���Ƽ� �Ű�Ƚᵵ ��)
            if (Display.main.systemWidth >= Display.main.systemHeight)
            {
                float tempHeight = Display.main.systemWidth * 9 / 16.0f;
                float tempWidth = Display.main.systemHeight * CRITERIA_NUM;
                //16:9�� ���� ���� ���̰� ���� ����ͺ��� ������ �˻�
                resolution.x = tempHeight > Display.main.systemHeight ? (int)tempWidth : Display.main.systemWidth;
                resolution.y = resolution.x == tempWidth ? Display.main.systemHeight : (int)tempHeight;

                //resolution.x = (int)tempWidth;
                resolution.y = Display.main.systemHeight;
            }
            
        }
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
    
    public Vector2Int GetLowestResolution() => new (Display.main.systemWidth / 4, Display.main.systemHeight / 4);

    //���͹ڽ� ���� ���������� ������ ��
    void OnPreCull() => GL.Clear(true, true, Color.black);

    public void InitResolutionSetting()
    {
        previewFontRatio = (previewMaxLength - 100) / previewText.fontSize;
        //����� Ǯ��ũ�� ���� �ҷ��ͼ� isWindowedScreen ������ ����
        IsWindowedScreen = SaveManager.Instance.LoadIsWindowedScreen();

        //����� �ػ� nowWidthPixel�� nowHeightPixel ������ ����
        //SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        Vector2[] offsets = ConvertResolutionToOffsets(new Vector2Int(nowWidthPixel, nowHeightPixel), true);
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

        //����� ������ ����Ʈ ����
        frameRateReady = SaveManager.Instance.LoadFrameRate();

        BlinkEffect.StartPoint = SaveManager.Instance.LoadStartPoint();

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

        //maxResolutionToOffsets = ConvertResolutionToOffsets(new Vector2Int((int) (Display.main.systemHeight / 9f * 16), (int) Display.main.systemHeight));
        maxResolutionToOffsets = ConvertResolutionToOffsets(new Vector2Int((int) (Display.main.systemHeight * ((float)Display.main.systemWidth / Display.main.systemHeight)), (int) Display.main.systemHeight), true);
    }

}
