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

        for (int i = 0; i < currentList.Count; i++)
        {
            //print($"커런트 리스트 : {currentList[i].x} : {currentList[i].y}");
        }

    }

    private void OnEnable()
    {
        isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        //insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;
        inside.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        //드롭다운 아이템 현재 리스트로 교체
        RedefineDropdown(isFullScreen);
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

        // V-Sync 비활성화
        //QualitySettings.vSyncCount = 0;
        //저장된 프레임 레이트 적용
        //Application.targetFrameRate = SaveManager.Instance.LoadFrameRate();
        frameRateReady = SaveManager.Instance.LoadFrameRate();
        //프레임 드롭다운 아이템 저장된 값으로 변경
        frameRateDropdown.value = frameRateReady / 30 - 1;
    }

    private void Update()
    {
        //'모니터'의 현재 해상도를 가져옴
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
        float CRITERIA_NUM = 16f / 9f;

        //16:9보다 가로가 더 긴 모니터의 경우 16:9 해상도만 나옴
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
        float CRITERIA_NUM = 16f / 9f;

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
    public void ReadyResolution(int value)
    {
        print("ReadyResolution 실행");
        float CRITERIA_NUM = 16f / 9f;

        //리스트값으로 매개변수 받아서 넣어주면 될듯
        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreenReady == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
            //previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
            previewText.fontSize = inside.size.x * previewFontRatio;
            previewText.text = $"{(int)currentList[value].x} X {(int)currentList[value].y}\n{frameRateReady}hz";
            //previewText.rectTransform.sizeDelta = Vector2();
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            //previewText.fontSize = (inside.rect.width - 100) / previewFontRatio;
            previewText.fontSize = inside.size.x * previewFontRatio;
            previewText.text = $"{(int)hdList[value].x} X {(int)hdList[value].y}\n{frameRateReady}hz";
        }
    }

    //확인버튼 누를시 해상도 적용
    private void ApplyResolution()
    {
        int value = resolutiondropdown.value;
        float CRITERIA_NUM = 16f / 9f;

        //리스트값으로 매개변수 받아서 넣어주면 될듯
        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            StartCoroutine(ResolutionWindow(currentList[value].x, currentList[value].y));
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
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

        //해상도 메뉴 목록 재정의
        //isFullScreenReady = !isFullScreenReady;
        RedefineDropdown(isFullScreenReady);

        //변경할 드롭다운이 현재 드롭다운과 번호가 같을때 대비 0으로 바꿔준뒤 다른 인덱스 적용
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;
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

    //확인버튼 누를 때 자동 실행
    public void PressApply()
    {
        ApplyFullScreenSwitch();
        //ApplyFullScreenSwitch 다음에 ApplyResolution 실행 할 것
        ApplyResolution();
        ApplyFrameRate();
    }

    //프리뷰 화면 사이즈 조정 메소드
    private void ResizePreviewImage(float targetWidth, float targetHeight, SpriteRenderer rect)
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
                //rect.sizeDelta = new Vector2(previewMaxLength, previewMaxLength * ratio1);
                //1000을 1로 보고 있는 상태임 즉 우리 눈에 보이는 실제 가로 세로는 다음과 같음
                //1 * 1000 : ratio1 * 1000
                //하지만 rect.size의 x,y 표시는 다음과 같음
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
            //바깥테두리는 항상 약간 더 크게 그림
            //outside.sizeDelta = new Vector2(outside.rect.width + 50, outside.rect.height + 50);

            ratio1 = 1 / (targetWidth / targetHeight);
            ratio2 = 1 / (targetHeight / targetWidth);

            //inside 공식을 어떻게 할 것인가
            //inside의 크기는 항상 outside를 기준으로 하며 outside보다 작거나 같아야함
            //그러므로 inside.size의 x와 y는 outside.size의 x와 y의 값을 넘을 수 없음
            //여기서 문제가 있음
            //scale은 항상 1000, 1000 고정으로 유지되어야만 함
            //이렇게 되면 inside는 항상 1000, 1000의 영향을 받으며, outside를 기준으로 얼마나 작은지 구한다고해도 별의미가 없을 것임
            //방법 1 : 목표 해상도 가로가 더 길다고 가정시
            //ratio1 = 1 / (targetWidth / targetHeight); 이 코드를 써주게 되면 1000, 1000에 대한 비율(정확히는 세로)을 얻을 수 있음, 1 : ratio1이 되는거임 현재 outside에서 쓰이고 있음
            //이제 해야할것은 outside보다 비율을 작게 조정하는 것임, 이제 어떤값을 정해서 1과 ratio1에 적용을 해야하는가?
            //일단 나눗셈이 사칙연산으로 들어가야 할것 같다.
            //내 모니터 크기와 목표 해상도 사각형이 정확히 몇배인지 알아낼 방법도 없다
            //rect.size = new Vector2(ratio1 / (Display.main.systemWidth / targetWidth), ratio2 / (Display.main.systemHeight / targetHeight));
            rect.size = new Vector2(1, ratio1);
            //바깥테두리는 항상 약간 더 크게 그림(임시주석**************)
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

    //레터박스 완전 검은색으로 나오게 함
    void OnPreCull() => GL.Clear(true, true, Color.black);


}
