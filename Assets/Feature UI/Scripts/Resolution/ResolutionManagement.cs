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
    //해상도 설정 제일 마지막에 사용된 오브젝트 저장하는 용도
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
    
    // IsWindowedScreen, IsFullScreenReady에서 get은 상관 없는데 set할 때 반드시 이 Property를 통해서 값을 변경해야 함
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
        //저장된 풀스크린 여부 불러와서 isWindowedScreen 변수에 적용
        IsWindowedScreen = SaveManager.Instance.LoadIsWindowedScreen();

        //저장된 해상도 nowWidthPixel과 nowHeightPixel 변수에 적용
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

        //모니터 해상도에 맞는 currentList 추가
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

        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        //드롭다운 아이템 현재 리스트로 교체
        RedefineDropdown(isWindowedScreen);

        print($"마지막 오브젝트 : {lastApplyObject.name}");

        //저장을 드롭다운 이용해서 했을 때
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            for (int i = 0; i < nowList.Count; i++)
            {
                if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
                {
                    //드롭다운 아이템 저장된 값으로 드롭다운 아이템 선택
                    resolutiondropdown.value = i;
                    //프리뷰 설정
                    ReadyResolution(i);
                    //해상도 설정
                    ApplyResolution();
                    //반복문 종료
                    break;
                }
            }
        }
        //저장을 인풋 필드로 했을때
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
        //저장된 프레임 레이트 적용
        frameRateReady = SaveManager.Instance.LoadFrameRate();
        //프레임 드롭다운 아이템 저장된 값으로 변경
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
        //맨처음 시작시 Start에서 실행해야 오류 없음
        resolutiondropdown.captionText.text = "";
    }

    private void RedefineDropdown(bool checkWindowedScreen)
    {
        //16:9보다 가로가 더 긴 모니터의 경우 16:9 해상도만 나옴
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
            //드롭다운 아이템 모두 제거
            resolutiondropdown.ClearOptions();

            //드롭다운에 들어갈 아이템 리스트 제작
            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            //드롭다운에 아이템 리스트 삽입
            resolutiondropdown.AddOptions(temp);
            //드롭다운 새로고침
            resolutiondropdown.RefreshShownValue();

            nowList = currentList;
        }
    }

    //해상도 설정 메소드
    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, !isWindowedScreen);

        //SetResolution하고 바로 RescaleWindow 실행하면 적용 제대로 안되는 버그 있어서 1프레임 쉬었다 시작
        yield return null;

        RescaleWindow(width, height);

        SaveManager.Instance.SaveResolution((int)width, (int)height);
        nowWidthPixel = (int)width;
        nowHeightPixel = (int)height;
        yield break;
    }

    /// <summary>
    /// 화면 크기 조절 메소드
    /// </summary>
    /// <param name="width">'목표' 가로비율로 명명함</param>
    /// <param name="height">'목표' 세로비율로 명명함</param>
    private void RescaleWindow(float width, float height)
    {
        //print("리스케일 윈도우");
        GL.Clear(true, true, Color.black);  // 화면을 검은색으로 지움

        //16 / 9값과 비교하여 16:9 화면에서 세로길이 혹은 가로길이 중에서
        //어느 길이가 더 긴지 알아내는 판별용 변수
        float checkedValue = width / height;
        //CRITERIA_NUM = 16f / 9f;

        Rect rect = new Rect(0, 0, 1, 1);
        float temp1 = 0;
        //세로 비율을 9로 통일 후 변경될 목표 가로 비율
        float temp2 = 0;

        //목표 비율이 16 : 9일때
        if (checkedValue == CRITERIA_NUM)
        {
            cam.rect = rect;
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
            return;
        }
        //목표 비율의 '가로 비율'이 16 : 9보다 클때
        else if (checkedValue > CRITERIA_NUM)
        {
            //목표 비율 * X = 9가 성립될 때의 X값을 구함
            temp1 = 9 / height;
            //미지수 X와 '목표' 비율의 가로를 곱하여 '최종' 가로비율을 구함
            temp2 = temp1 * width;

            //수정된 '최종'비율은 다음과 같음  (temp2 : 9)

            //16 / '최종' 가로비율의 값을 rect.width에 대입
            rect.width = 16 / temp2;

            //1 - rect.width를 한 뒤 2를 나눈 값을 rect.x에 대입(밀린 화면 중앙으로 이동)
            rect.x = (1 - rect.width) / 2;
            BlinkEffect.StartPoint = BlinkEffect.BLINK_START_POINT_INIT;
        }
        //목표 비율의 '세로 비율'이 16 : 9보다 클때
        else if (checkedValue < CRITERIA_NUM)
        {
            //목표 비율 * X = 9가 성립될 때 X값을 구함
            temp1 = 9 / height;
            //미지수 X와 '목표' 비율의 가로를 곱하여 '최종' 가로비율을 구함
            temp2 = temp1 * width;

            //수정된 '최종'비율은 다음과 같음  (temp2 : 9)

            //'최종' 가로비율 / 16의 값을 rect.height에 대입
            rect.height = temp2 / 16;

            //1 - rect.height를 한 뒤 2를 나눈 값을 rect.y에 대입(밀린 화면 중앙으로 이동)
            rect.y = (1 - rect.height) / 2;

            float startPointConversionValue = Mathf.Lerp(BlinkEffect.BLINK_START_POINT_INIT + 0.13f, BlinkEffect.BLINK_START_POINT_INIT, (checkedValue - 1f) / (1.77f - 1f));
            BlinkEffect.StartPoint = startPointConversionValue;
        }

        //카메라에 최종적용
        cam.rect = rect;
        //camRect 저장
        SaveManager.Instance.SaveCamRect(rect.x, rect.y, rect.width, rect.height);
        SaveManager.Instance.SaveStartPoint(BlinkEffect.StartPoint);
    }

    //해상도 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
    public void ReadyResolution(int value)
    {
        DropdownItemClick.dropdownValue = resolutiondropdown.value;

        resolutiondropdown.captionText.text = "";

        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isWindowedScreenReady == false && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
            inputFieldWidth.text = $"{(int)currentList[value].x}";
            inputFieldHeight.text = $"{(int)currentList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            inputFieldWidth.text = $"{(int)hdList[value].x}";
            inputFieldHeight.text = $"{(int)hdList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
    }

    //확인버튼 누를시 해상도 적용
    private void ApplyResolution()
    {
        int value = resolutiondropdown.value;

        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isWindowedScreen == false && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
        {
            StartCoroutine(ResolutionWindow(currentList[value].x, currentList[value].y));
            SaveManager.Instance.SaveLastApplyObject(0);
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
            SaveManager.Instance.SaveLastApplyObject(0);
        }
    }

    //해상도 프리뷰, 체크버튼 모양만 건드림(체크 버튼 누를때 자동호출)
    public void ReadyFullScreenSwitch()
    {
        IsWindowedScreenReady = !isWindowedScreenReady;
        fullScreenSwitch.sprite = isWindowedScreenReady ? checkImage : nonCheckImage;

        RedefineDropdown(isWindowedScreenReady);

        //변경할 드롭다운이 현재 드롭다운과 번호가 같을때 대비 0으로 바꿔준뒤 다른 인덱스 적용(제일 큰해상도로 변경)
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;

        //드롭다운 표시값 비우기
        resolutiondropdown.captionText.text = "";

        lastApplyObject = resolutiondropdown.gameObject;
    }

    //확인버튼 누를시 풀스크린 여부 적용
    private void ApplyFullScreenSwitch()
    {
        IsWindowedScreen = isWindowedScreenReady;
        SaveManager.Instance.SaveIsWindowedScreen(isWindowedScreen);
    }

    //프레임 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
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

    //확인버튼 누를시 프레임 적용
    private void ApplyFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRateReady;
        SaveManager.Instance.SaveFrameRate(frameRateReady);
    }

    //매개변수 0은 inputFieldWidth, 1은 inputFieldHeight
    public void ReadyInputField(int num)
    {
        string result = "";
        int widthNum = 0;
        int heightNum = 0;
        int maxNum = 0;
        int minNum = 0;
        float ratio = 0f;

        if (num == 0)   //inputFieldWidth 입력시
        {
            result = inputFieldWidth.text;

            if (int.TryParse(result, out widthNum))  //입력값이 숫자인지 확인
            {
                //전체화면 여부에 따라서 최대값 숫자 정함
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

                //입력값이 최소값 최대값 넘지 않게 조정
                widthNum = Mathf.Clamp(widthNum, minNum, maxNum);
                inputFieldWidth.text = $"{widthNum}";

                heightNum = (int)Mathf.Round(widthNum / ratio);
                inputFieldHeight.text = $"{heightNum}";

                lastApplyObject = inputFieldWidth.gameObject;
            }
            else //숫자 아닐시 에러
            {
                inputFieldWidth.text = "ERROR";
                inputFieldHeight.text = "ERROR";
                return;
            }
        }
        else if (num == 1)  //inputFieldHeight 입력시
        {
            result = inputFieldHeight.text;

            if (int.TryParse(result, out heightNum))
            {

                //전체화면 여부에 따라서 최대값 숫자 정함
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

                //입력값이 최소값 최대값 넘지 않게 조정
                heightNum = Mathf.Clamp(heightNum, minNum, maxNum);
                inputFieldHeight.text = $"{heightNum}";

                widthNum = (int)Mathf.Round(heightNum * ratio);
                inputFieldWidth.text = $"{widthNum}";

                lastApplyObject = inputFieldHeight.gameObject;
            }
            else    //숫자 아닐시 에러(숫자가 너무 길어도 에러남)
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
        //허용되지 않는 값 넣을 시 메소드 종료
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

    //확인버튼 누를 때 자동 실행
    public void PressApply()
    {
        ApplyFullScreenSwitch();
        //ApplyFullScreenSwitch 다음에 ApplyResolution 혹은 ApplyInputField 실행 할 것
        //드롭다운 값 적용
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            ApplyResolution();
        }
        //인풋필드 값 적용
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

    //프리뷰 화면 사이즈 조정 메소드
    private void ResizePreviewImage(float targetWidth, float targetHeight, RectTransform rect)
    {
        float ratio1 = 0;
        float ratio2 = 0;
        
        if (rect == outside)
        {
            if (targetWidth >= targetHeight)
            {
                //목표 해상도는 1 : ratio로 표현 가능함
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
            
            //바깥테두리는 항상 약간 더 크게 그림
            outside.sizeDelta = new Vector2(outside.rect.width + 50, outside.rect.height + 50);
        }
    }
    
    public void ResizePreviewByOffsets(Vector2 offsetMin, Vector2 offsetMax)
    {
        InsideOffsetMin = offsetMin;
        InsideOffsetMax = offsetMax;
    }
    
    /// <summary>
    /// Rect의 크기가 변경될 때 호출되는 이벤트 핸들러
    /// </summary>
    /// <param name="rect"></param>
    private void RectSizeChangedHandler(RectTransform rect)
    {
        Vector2Int resolution = ConvertSizeToResolution(rect.sizeDelta);
        UpdateResolutionText(resolution.x, resolution.y);
        lastApplyObject = insideWindow.gameObject;
    }

    /// <summary>
    /// Rect의 크기가 변경되면 해상도 텍스트를 업데이트
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
    /// <returns>Index 0은 OffsetMin, 1은 OffsetMax</returns>
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
    /// 더블 클릭에 의한 줌인/줌아웃
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

    //레터박스 완전 검은색으로 나오게 함
    void OnPreCull() => GL.Clear(true, true, Color.black);


}
