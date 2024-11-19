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
    [SerializeField] private RectTransform outside;
    [SerializeField] private RectTransform inside;

    [SerializeField] private Sprite checkImage;
    [SerializeField] private Sprite nonCheckImage;
    [SerializeField] private Sprite fullscreenInside;
    [SerializeField] private Sprite windowedInside;

    bool isFullScreen = true;
    bool isFullScreenReady = true;
    int frameRateReady = 60;
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

    [SerializeField] private Text testText;
    [SerializeField] private Text testText2;

    private void Awake()
    {
        // V-Sync 비활성화
        QualitySettings.vSyncCount = 0;
        //저장된 프레임 레이트 적용
        Application.targetFrameRate = SaveManager.Instance.LoadFrameRate();
        //프레임 드롭다운 아이템 저장된 값으로 변경
        frameRateDropdown.value = SaveManager.Instance.LoadFrameRate() / 30 - 1;

        //저장된 풀스크린 여부 불러와서 isFullScreen 변수에 적용
        isFullScreen = SaveManager.Instance.LoadIsFullScreen();
        /*isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;*/

        //저장된 해상도 nowWidthPixel과 nowHeightPixel 변수에 적용
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //불러온 변수값들을 이용해 이전에 쓰던 해상도 설정 반영
        //Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        //////////////////////////////////////////////////////////////////////////
        
        hdList.Clear();
        currentList.Clear();

        int temp = 540;
        for (int i = 0; i < 9; i++)
        {
            currentList.Add(new Vector2(Display.main.systemWidth - temp, Display.main.systemHeight - temp));
            temp -= 60;
            if (i == 7)
            {
                temp = 0;
            }
        }

        //Screen.resolutions은 사람들이 자주 쓰는 해상도를 모아놓은 것임(현재 내 모니터와 관계 없음)
        List<Resolution> monitorResolutions = Screen.resolutions.ToList();

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

            if (Mathf.Approximately(monitorNum, itemNum) && Display.main.systemWidth >= item.width && Display.main.systemHeight >= item.height)
            {
                ListValueDuplicateCheck(currentList, item);
            }

        }



    }

    private void OnEnable()
    {
        isFullScreenReady = isFullScreen;
        fullScreenSwitch.sprite = isFullScreen ? checkImage : nonCheckImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        //드롭다운 아이템 현재 리스트로 교체
        RedefineDropdown(isFullScreen);
        print("nowList : " + nowList.Count);
        for (int i = 0; i < nowList.Count; i++)
        {
            if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
            {
                print(nowWidthPixel + " : " + nowList[i].x + " : " + nowHeightPixel + " : " + nowList[i].y);

                //드롭다운 아이템 저장된 값으로 드롭다운 아이템 선택
                resolutiondropdown.value = i;
                ApplyResolution();
                //메소드 종료
                return;
            }
        }
        //print(resolutiondropdown.options[0].text + "zfasdfasdfasdfasd");
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
        for (int i = 0; i < list.Count; i++)
        {
            //같은게 있으면 넣지 않고 메소드 종료
            if (list[i].x == item.width && list[i].y == item.height)
            {
                return;
            }
        }
        //리스트 안에 같은게 없으면 리스트에 삽입
        list.Add(new Vector2(item.width, item.height));
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
        print("리스케일 윈도우");
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
            print("16 : 9");
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
    }

    //해상도 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
    //해상도 프리뷰만 건드림
    public void ReadyResolution(int value)
    {
        float CRITERIA_NUM = 16f / 9f;

        //리스트값으로 매개변수 받아서 넣어주면 될듯
        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreenReady == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)currentList[value].x, (int)currentList[value].y, inside);
            previewText.text = $"{(int)currentList[value].x} X {(int)currentList[value].y}";
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage((int)hdList[value].x, (int)hdList[value].y, inside);
            previewText.text = $"{(int)hdList[value].x} X {(int)hdList[value].y}";
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
        insideImage.sprite = isFullScreenReady ? fullscreenInside : windowedInside;

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
        ApplyResolution();
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
        ApplyResolution();
        ApplyFrameRate();
    }

    //프리뷰 화면 사이즈 조정 메소드
    private void ResizePreviewImage(float targetWidth, float targetHeight, RectTransform rect)
    {
        float ratio = 0;
        if (rect == outside)
        {
            if (targetWidth >= targetHeight)
            {
                //목표 해상도는 1 : ratio로 표현 가능함
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
                //바깥 테두리와 안쪽 테두리가 몇배 차이나는지 조사후 1에서 그 값을 나눔
                ratio = 1 / (Display.main.systemWidth / targetWidth);
            }
            else
            {
                //바깥 테두리와 안쪽 테두리가 몇배 차이나는지 조사후 1에서 그 값을 나눔
                ratio = 1 / (Display.main.systemHeight / targetHeight);
            }
            //scaleDifference를 바깥 테두리의 가로와 세로에 곱해서 적용함
            if (Display.main.systemWidth == targetWidth)
            {
                //목표 해상도가 내 모니터 크기와 같을때 안쪽,바깥쪽 테두리가 겹쳐보이기에 -50 적당히 빼줌
                rect.sizeDelta = new Vector2(outside.rect.width * ratio - 50, outside.rect.height * ratio - 50);
            }
            else
            {
                rect.sizeDelta = new Vector2(outside.rect.width * ratio, outside.rect.height * ratio);
            }
        }
    }

    //레터박스 완전 검은색으로 나오게 함
    void OnPreCull() => GL.Clear(true, true, Color.black);


}
