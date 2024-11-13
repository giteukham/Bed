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
        //저장된 풀스크린 여부 불러와서 isFullScreen 변수에 적용
        isFullScreen = SaveManager.Instance.LoadIsFullScreen();
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        //저장된 해상도 nowWidthPixel과 nowHeightPixel 변수에 적용
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //불러온 변수값들을 이용해 이전에 쓰던 해상도 설정 반영
        Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        int temp = 480;
        for (int i = 0; i < 5; i++)
        {
            //모니터 가로 픽셀 / 모니터 세로 픽셀이 1.77보다 작을 경우 적용할 드롭다운 리스트 제작
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
            //print($"목록 {monitorResolutions[i].width} : {monitorResolutions[i].height}");
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
            //두수의 최대공약수 구함
            /*int a = GCD(item.width, item.height);
            int b = GCD(Display.main.systemWidth, Display.main.systemHeight);

            //비율 16:9인 것만 리스트에 넣음
            if (item.width / a == 16 && item.height / a == 9)
            {
                hdList.Add(new Vector2(item.width, item.height));
            }

            //본인 모니터 비율과 같은 것만 리스트에 넣음
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
        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        //드롭다운 아이템 현재 리스트로 교체
        RedefineDropdown();

        for (int i = 0; i < nowList.Count; i++)
        {
            if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
            {
                //드롭다운 아이템 저장된 값으로 드롭다운 아이템 선택
                dropdown.value = i;
                //메소드 종료
                return;
            }
        }
        //print(dropdown.options[0].text + "zfasdfasdfasdfasd");
    }

    private void Update()
    {
        //'모니터'의 현재 해상도를 가져옴
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
            //이 코드 실행시 실제로 드롭다운 아이템에 해당하는 해상도로 변경됨
            dropdown.value = 2;
        }
    }

    //최대공약수 계산 메소드
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

        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        /*if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            //드롭다운 아이템 모두 제거
            dropdown.ClearOptions();

            //드롭다운에 들어갈 아이템 리스트 제작
            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            //드롭다운에 아이템 리스트 삽입
            dropdown.AddOptions(temp);
            //드롭다운 새로고침
            dropdown.RefreshShownValue();

            nowList = currentList;
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
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

        //16:9보다 가로가 더 긴 모니터의 경우 16:9 해상도만 나옴
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
            //드롭다운 아이템 모두 제거
            dropdown.ClearOptions();

            //드롭다운에 들어갈 아이템 리스트 제작
            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            //드롭다운에 아이템 리스트 삽입
            dropdown.AddOptions(temp);
            //드롭다운 새로고침
            dropdown.RefreshShownValue();

            nowList = currentList;
        }


    }

    //풀스크린으로 만드는 버튼(스위치 버튼 누를때 자동호출)
    //풀스크린과 창모드 드롭다운 아이템이 다르니 스위치 눌리면 드롭다운 인덱스 0번으로 적용하는 걸로
    public void FullScreenSwitch()
    {
        isFullScreen = !isFullScreen;
        SaveManager.Instance.SaveIsFullScreen(isFullScreen);
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        //창 크기, 풀스크린 여부 적용
        //StartCoroutine(ResolutionWindow(nowWidthPixel, nowHeightPixel));
        //해상도 메뉴 목록 재정의
        RedefineDropdown();
        //아마처음부터 0이라 바꿀필요없어서 적용안되는듯
        //그래서 적용하려면 0이 아닌값을 적용하거나, 혹은 다른값으로 잠깐 바꾼뒤에 0을 다시 적용해야함
        //dropdown.value = 1;
        //dropdown.value = 0;

        //변경할 드롭다운이 현재 드롭다운과 번호가 같을때 대비 0으로 바꿔준뒤 다른 인덱스 적용
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
    /// 화면 크기 조절 메소드
    /// </summary>
    /// <param name="width">'목표' 가로비율로 명명함</param>
    /// <param name="height">'목표' 세로비율로 명명함</param>
    private void RescaleWindow(float width, float height)
    {
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

    //레터박스 완전 검은색으로 나오게 함
    void OnPreCull() => GL.Clear(true, true, Color.black);

    //드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
    public void EnterResolution(int value)
    {
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
}
