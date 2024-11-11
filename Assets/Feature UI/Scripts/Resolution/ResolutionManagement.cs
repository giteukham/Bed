using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using static UnityEngine.Rendering.DebugUI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Text screenText;
    [SerializeField] private SaveManager saveManager;
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

    private void Awake()
    {
        //저장된 풀스크린 여부 불러와서 적용
        isFullScreen = saveManager.LoadIsFullScreen();
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        saveManager.LoadResolution(out nowWidthPixel, out nowHeightPixel);
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

        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        RedefineResolutionMenu();
        //어떻게 드롭다운에 있는 해상도 text를 가져올 것인가?

    }

    private void OnEnable()
    {
        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        RedefineResolutionMenu();
        //어떻게 드롭다운에 있는 해상도 text를 가져올 것인가?
        for (int i = 0; i < 5; i++)
        {

        }
        print(dropdown.options[0].text);

        print("온인애이블");
    }

    private void Update()
    {
        //'모니터'의 현재 해상도를 가져옴
        //screenText.text = Display.main.systemWidth + " " + Display.main.systemHeight;
        screenText.text = nowWidthPixel + " " + nowHeightPixel;
    }

    private void RedefineResolutionMenu()
    {
        float CRITERIA_NUM = 16f / 9f;

        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            dropdown.ClearOptions();

            List<string> temp = new List<string>();
            for (int i = 0; i < currentList.Count; i++)
            {
                temp.Add($"{currentList[i].x} X {currentList[i].y}");
            }

            dropdown.AddOptions(temp);
            dropdown.RefreshShownValue();
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
        }
    }

    //풀스크린으로 만드는 버튼(스위치 버튼 누를때 자동호출)
    public void FullScreenSwitch()
    {
        isFullScreen = !isFullScreen;
        saveManager.SaveIsFullScreen(isFullScreen);
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;

        //창 크기, 풀스크린 여부 적용
        StartCoroutine(ResolutionWindow(nowWidthPixel, nowHeightPixel));
        //해상도 메뉴 목록 재정의
        RedefineResolutionMenu();
    }

    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, isFullScreen);

        yield return null;

        RescaleWindow(width, height);

        saveManager.SaveResolution((int)width, (int)height);
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
            print("위");
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
            print("아래");
        }


    }
}
