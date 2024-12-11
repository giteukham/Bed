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
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Dropdown resolutiondropdown;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private RectTransform outside;
    [SerializeField] private RectTransform inside;

    [SerializeField] private Sprite checkImage;
    [SerializeField] private Sprite nonCheckImage;
    [SerializeField] private Sprite fullscreenInside;
    [SerializeField] private Sprite windowedInside;
    
    #region Properties
    public Vector2 InsideAnchoredPosition
    {
        get => inside.anchoredPosition;
        set => inside.anchoredPosition = value;
    }
    
    public Vector2 InsideSize
    {
        get => inside.sizeDelta;
        set
        {
            if (inside.sizeDelta != value)
            {
                inside.sizeDelta = value;
                OnInsideSizeChanged?.Invoke(inside);
            }
        }
    }
    
    public Vector2 InsideOffsetMin
    {
        get => inside.offsetMin;
        set
        {
            if (inside.offsetMin != value)
            {
                inside.offsetMin = value;
                OnInsideOffsetMinChanged?.Invoke(inside);
            }
        }
    }
    
    public Vector2 InsideOffsetMax
    {
        get => inside.offsetMax;
        set
        {
            if (inside.offsetMax != value)
            {
                inside.offsetMax = value;
                OnInsideOffsetMaxChanged?.Invoke(inside);
            }
        }
    }
    
    public Vector2 OutsideAnchoredPosition => outside.anchoredPosition;
    public Vector2 OutsideSize => outside.sizeDelta;
    public Vector2 OutsideOffsetMin => outside.offsetMin;
    public Vector2 OutsideOffsetMax => outside.offsetMax;
    #endregion

    bool isFullScreen = true;
    bool isFullScreenReady = true;
    int frameRateReady = 60;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int previewMaxLength = 1000;
    float previewFontRatio = 11.25f;
    const float CRITERIA_NUM = 16f / 9f;
    //해상도 설정 제일 마지막에 사용된 오브젝트 저장하는 용도
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
    
    public UnityEvent<RectTransform> OnInsideSizeChanged;
    public UnityEvent<RectTransform> OnInsideOffsetMinChanged, OnInsideOffsetMaxChanged;

    private void Awake()
    {
        previewFontRatio = (previewMaxLength - 100) / previewText.fontSize;
        //저장된 풀스크린 여부 불러와서 isFullScreen 변수에 적용
        isFullScreen = SaveManager.Instance.LoadIsFullScreen();
        /*isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;*/

        //저장된 해상도 nowWidthPixel과 nowHeightPixel 변수에 적용
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //불러온 변수값들을 이용해 이전에 쓰던 해상도 설정 반영
        //Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        //print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        switch (SaveManager.Instance.LoadLastApplyObject())
        {
            case 0:
                lastApplyObject = resolutiondropdown.gameObject;
                break;
            case 1:
                lastApplyObject = inputField.gameObject;
                break;
        }

        //////////////////////////////////////////////////////////////////////////

        hdList.Clear();
        currentList.Clear();

        //모니터 해상도 가로,세로의 최대공약수 구함
        /*int gcd = GCD(Display.main.systemWidth, Display.main.systemHeight);
        //내 모니터의 비율 구한 뒤에 20을 곱해줌
        int value1 = Display.main.systemWidth / gcd * 20;
        int value2 = Display.main.systemHeight / gcd * 20;
        //드롭다운에 들어갈 최소값을 정함
        int startWidth = Display.main.systemWidth / 4;
        int startHeight = Display.main.systemHeight / 4;*/

        //모니터 해상도에 맞는 currentList 추가
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

        //모니터 해상도에 맞는 currentList 추가
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

        //Screen.resolutions은 사람들이 자주 쓰는 해상도를 모아놓은 것임(현재 내 모니터와 관계 없음)
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

            //내 모니터 비율과 근사값인가? && item 해상도가 내 모니터 이하 해상도인가?
            if (Mathf.Approximately(monitorNum, itemNum) && Display.main.systemWidth >= item.width && Display.main.systemHeight >= item.height)
            {
                ListValueDuplicateCheck(currentList, item);
            }
        }*/

    }

    private void OnEnable()
    {
        OnInsideSizeChanged.AddListener(OnSizeChangedHandler);
        OnInsideOffsetMinChanged.AddListener(OnSizeChangedHandler);
        OnInsideOffsetMaxChanged.AddListener(OnSizeChangedHandler);
        
        isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;
        //inside.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        //드롭다운 아이템 현재 리스트로 교체
        RedefineDropdown(isFullScreen);

        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            for (int i = 0; i < nowList.Count; i++)
            {
                if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
                {
                    //print(nowWidthPixel + " : " + nowList[i].x + " : " + nowHeightPixel + " : " + nowList[i].y);

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
        else if (lastApplyObject == inputField.gameObject)
        {
            //그런데 어차피 드롭다운 텍스트는 쓰지 않을 예정임
            //그러니 조건 상관없이 inputField.text에 항상 해상도값 텍스트 표시해야함
            //맨처음 대비 이거 해줘야함
            print(resolutiondropdown.captionText.text);
            resolutiondropdown.captionText.text = "";
            print(resolutiondropdown.captionText.text);
            //인풋필드에 저장된 값 불러오고 인풋필드 해상도 맞춰주면 될듯(순서 지켜야함)
            inputField.text = $"{nowWidthPixel} X {nowHeightPixel}";
            ReadyInputField();
            ApplyInputField();
        }

        //조건 상관없이 항상 드롭다운 텍스트 비워둠
        //resolutiondropdown.captionText.text = "";
        //조건 상관없이 항상 inputField에 해상도 표시
        //inputField.text = $"{nowWidthPixel} X {nowHeightPixel}";

        // V-Sync 비활성화
        //QualitySettings.vSyncCount = 0;
        //저장된 프레임 레이트 적용
        //Application.targetFrameRate = SaveManager.Instance.LoadFrameRate();
        frameRateReady = SaveManager.Instance.LoadFrameRate();
        //프레임 드롭다운 아이템 저장된 값으로 변경
        frameRateDropdown.value = frameRateReady / 30 - 1;
    }

    private void Start()
    {
        //맨처음 시작시 Start에서 실행해야 오류 없음
        resolutiondropdown.captionText.text = "";
    }

    /// <summary>
    /// 11-30 최무령 수정 : 해상도 텍스트 바꾸는 거 UpdateResolutionText로 이동
    /// </summary>
    private void Update()
    {
        //'모니터'의 현재 해상도를 가져옴
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
        //temp기본값은 list 마지막 인덱스
        //int temp = list.Count;

        for (int i = 0; i < list.Count; i++)
        {
            //같은게 있으면 넣지 않고 메소드 종료
            if (list[i].x == item.width && list[i].y == item.height)
            {
                return;
            }
            //아래 else if문은 드롭다운 리스트가 낮은 숫자부터 시작된다는 가정하에 작동함
            //리스트 가로보다 아이템 가로가 짧을때
            else if (list[i].x > item.width)
            {
                //temp = i;
                print($"{item.width}는 {list[i].x} 보다 짧음");
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
        //리스트 안에 같은게 없으면서 가장 큰 width를 가지고 있을때 리스트 마지막에 삽입
        list.Add(new Vector2(item.width, item.height));
        //list.Insert(temp, new Vector2(item.width, item.height));
    }

    private void RedefineDropdown(bool checkFullScreen)
    {
        //CRITERIA_NUM = 16f / 9f;

        //16:9보다 가로가 더 긴 모니터의 경우 16:9 해상도만 나옴
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
        Screen.SetResolution((int)width, (int)height, isFullScreen);

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
            //print("16 : 9");
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
        }

        //카메라에 최종적용
        cam.rect = rect;
        //camRect 저장
        SaveManager.Instance.SaveCamRect(rect.x, rect.y, rect.width, rect.height);
    }

    //해상도 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
    //해상도 프리뷰만 건드림
    /// <summary>
    /// 11-30 최무령 수정 : 해상도 텍스트 바꾸는 거 UpdateResolutionText로 이동
    /// </summary>
    /// <param name="value"></param>
    public void ReadyResolution(int value)
    {
        print("ReadyResolution 실행");
        resolutiondropdown.captionText.text = "";
        //CRITERIA_NUM = 16f / 9f;

        //리스트값으로 매개변수 받아서 넣어주면 될듯
        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreenReady == true && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
            inputField.text = $"{(int)currentList[value].x} X {(int)currentList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            inputField.text = $"{(int)hdList[value].x} X {(int)hdList[value].y}";
            lastApplyObject = resolutiondropdown.gameObject;
        }
    }

    //확인버튼 누를시 해상도 적용
    private void ApplyResolution()
    {
        int value = resolutiondropdown.value;
        //CRITERIA_NUM = 16f / 9f;

        //리스트값으로 매개변수 받아서 넣어주면 될듯
        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > (float)Display.main.systemWidth / Display.main.systemHeight))
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

        //해상도 메뉴 목록 재정의
        //isFullScreenReady = !isFullScreenReady;
        RedefineDropdown(isFullScreenReady);

        //변경할 드롭다운이 현재 드롭다운과 번호가 같을때 대비 0으로 바꿔준뒤 다른 인덱스 적용(제일 큰해상도로 변경)
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;

        //드롭다운 표시값 비우기
        resolutiondropdown.captionText.text = "";

        lastApplyObject = resolutiondropdown.gameObject;
        //ApplyResolution();
    }

    //확인버튼 누를시 풀스크린 여부 적용
    private void ApplyFullScreenSwitch()
    {
        //isFullScreen = !isFullScreen;
        isFullScreen = isFullScreenReady;
        SaveManager.Instance.SaveIsFullScreen(isFullScreen);
        //fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //해상도 메뉴 목록 재정의
        //RedefineDropdown(!isFullScreen);

        //변경할 드롭다운이 현재 드롭다운과 번호가 같을때 대비 0으로 바꿔준뒤 다른 인덱스 적용
        //resolutiondropdown.value = nowList.Count - 1;
        //이미 ApplyResolution에서 호출중
        //ApplyResolution();
    }

    //프레임 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
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

    //확인버튼 누를시 프레임 적용
    private void ApplyFrameRate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRateReady;
        SaveManager.Instance.SaveFrameRate(frameRateReady);
    }

    public void ReadyInputField()
    {
        //이거하면 오류남
        //inputField.text = $"{nowWidthPixel} X {nowHeightPixel}";
        char[] chars = inputField.text.ToCharArray();
        char[] separators = { 'X', ' ' };
        int widthNum = 0;
        int heightNum = 0;

        for (int i = 0; i < chars.Length; i++)
        {
            //인풋필드에 공백 혹은 X, x 있는지 확인
            if (chars[i] ==' ' || chars[i] == 'X' || chars[i] == 'x')
            {
                //공백과 X 기준으로 split
                string[] result = inputField.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                //만약 "1920 " 혹은 "1920X"로 적어서 result에 들어가는 숫자가 하나밖에 없을때 대비
                if (result.Length < 2)
                {
                    inputField.text = "ERROR";
                    return;
                }

                //split한 것이 숫자가 맞는지 확인
                if (int.TryParse(result[0], out widthNum) && int.TryParse(result[1], out heightNum))
                {
                    //창모드인지 전체화면인지 확인
                    if (isFullScreenReady == true)  //전체화면
                    {
                        if (widthNum > Display.main.systemWidth)    //현재 모니터가 허용하는 최대해상도 넘을시
                        {
                            widthNum = Display.main.systemWidth;
                            heightNum = Display.main.systemHeight;
                            inputField.text = $"{widthNum} X {heightNum}";
                        }
                        else if (Mathf.Approximately(widthNum / (float)heightNum, (float)Display.main.systemWidth / Display.main.systemHeight)) //현재 모니터 비율과 똑같으면 적용
                        {
                            inputField.text = $"{widthNum} X {heightNum}";
                        }
                        else    //현재 모니터와 비율 다르면 비율 다시 구해서 적용
                        {
                            /*widthNum = Display.main.systemWidth;
                            heightNum = Display.main.systemHeight;
                            inputField.text = $"{widthNum} X {heightNum}";*/

                            //세로값 다시 구함
                            //heightNum = Mathf.CeilToInt(widthNum / ((float)Display.main.systemWidth / Display.main.systemHeight));
                            heightNum = (int)Mathf.Round(widthNum / ((float)Display.main.systemWidth / Display.main.systemHeight)); //반올림 방식으로 변경
                            inputField.text = $"{widthNum} X {heightNum}";
                        }
                    }
                    else    //창모드
                    {
                        if (widthNum > hdList[hdList.Count - 1].x)    //현재 모니터가 허용하는 16:9 최대해상도 넘을시
                        {
                            widthNum = (int)hdList[hdList.Count - 1].x;
                            heightNum = (int)hdList[hdList.Count - 1].y;
                            inputField.text = $"{widthNum} X {heightNum}";
                        }
                        else if (Mathf.Approximately(widthNum / (float)heightNum, CRITERIA_NUM)) //현재 모니터 비율과 똑같으면 적용
                        {
                            print($"비율같음 : {widthNum} X {heightNum}");
                            inputField.text = $"{widthNum} X {heightNum}";
                        }
                        else    //16:9 비율 아닐시 가로값 기준으로 16:9 값 찾음
                        {
                            //세로값 다시 구함
                            print($"세로값1 : {widthNum / CRITERIA_NUM}");
                            //heightNum = Mathf.CeilToInt(widthNum / CRITERIA_NUM);
                            heightNum = (int)Mathf.Round(widthNum / CRITERIA_NUM);  //반올림 방식으로 변경
                            print($"세로값2 : {heightNum}");
                            inputField.text = $"{widthNum} X {heightNum}";
                        }
                    }
                    ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
                    ResizePreviewImage(widthNum, heightNum, inside);
                    previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
                    previewText.text = $"{widthNum} X {heightNum}\n{frameRateReady}hz";

                    lastApplyObject = inputField.gameObject;
                    return;
                }
                else
                {
                    inputField.text = "ERROR";
                    return;
                }
            }
        }

        //입력값에 X 혹은 공백이 들어가 있지 않을 시
        inputField.text = "ERROR";
        return;
    }

    public void ApplyInputField()
    {
        //허용되지 않는 값 넣을 시 메소드 종료
        if (inputField.text == "ERROR")
        {
            return;
        }

        char[] separators = { 'X', ' ' };
        string[] result = inputField.text.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        StartCoroutine(ResolutionWindow(float.Parse(result[0]), float.Parse(result[1])));
        SaveManager.Instance.SaveResolution(int.Parse(result[0]), int.Parse(result[1]));
        SaveManager.Instance.SaveLastApplyObject(1);
    }

    //확인버튼 누를 때 자동 실행
    public void PressApply()
    {
        ApplyFullScreenSwitch();
        //ApplyFullScreenSwitch 다음에 ApplyResolution 혹은 ApplyInputField 실행 할 것
        if (lastApplyObject == resolutiondropdown.gameObject)
        {
            ApplyResolution();
        }
        else
        {
            ApplyInputField();
        }
        ApplyFrameRate();
    }

    //프리뷰 화면 사이즈 조정 메소드
    /// <summary>
    /// 11-30 최무령 수정 : inside 부분 수정
    /// </summary>
    /// <param name="targetWidth"></param>
    /// <param name="targetHeight"></param>
    /// <param name="rect"></param>
    private void ResizePreviewImage(float targetWidth, float targetHeight, RectTransform rect)
    {
        float ratio1 = 0;
        float ratio2 = 0;
        //2560 1080에서 1920 1080 해상도 변환 기준
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns>Index 0은 OffsetMin, 1은 OffsetMax</returns>
    public Vector2[] GetOffsetsByResolution(float width, float height)
    {
        float ratio = width / height;
        Vector2 sizeDelta = new Vector2(previewMaxLength, previewMaxLength / ratio);
        
        Vector2 offsetMin = new Vector2(-sizeDelta.x / 2, -sizeDelta.y / 2);
        Vector2 offsetMax = new Vector2(sizeDelta.x / 2, sizeDelta.y / 2);

        return new Vector2[] { offsetMin, offsetMax };
    }
    
    public Vector2Int GetResolutionBySizeDelta(Vector2 sizeDelta)
    {
        int width = Mathf.FloorToInt((float)Display.main.systemWidth / previewMaxLength * sizeDelta.x);
        int height = Mathf.FloorToInt((float)Display.main.systemWidth / previewMaxLength * sizeDelta.y);

        return new Vector2Int(width, height);
    }
    
    /// <summary>
    /// Inside 내 마우스 로컬 위치
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    public Vector2 GetInsideLocalMousePoint(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inside, eventData.position, eventData.pressEventCamera, out Vector2 localMousePoint);
        return localMousePoint;
    }
    
    public Vector2 GetOutsideLocalMousePoint(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(outside, eventData.position, eventData.pressEventCamera, out Vector2 localMousePoint);
        return localMousePoint;
    }
    
    /// <summary>
    /// Rect의 크기가 변경될 때 호출되는 이벤트 핸들러
    /// </summary>
    /// <param name="rect"></param>
    private void OnSizeChangedHandler(RectTransform rect)
    {
        Vector2Int resolution = GetResolutionBySizeDelta(rect.sizeDelta);
        UpdateResolutionText(resolution.x, resolution.y);
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

        screenText.text = width + " " + height;
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

    //레터박스 완전 검은색으로 나오게 함
    void OnPreCull() => GL.Clear(true, true, Color.black);


}
