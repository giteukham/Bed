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
    [SerializeField] private Image insideImage;
    [SerializeField] private TMP_Dropdown resolutiondropdown;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private RectTransform outside;
    [SerializeField] private RectTransform inside;

    [SerializeField] private Sprite onImage;
    [SerializeField] private Sprite offImage;
    [SerializeField] private Sprite fullscreenInside;
    [SerializeField] private Sprite windowedInside;

    bool isFullScreen = true;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;
    int frameRate = 60;

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
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //저장된 해상도 nowWidthPixel과 nowHeightPixel 변수에 적용
        SaveManager.Instance.LoadResolution(out nowWidthPixel, out nowHeightPixel);
        //불러온 변수값들을 이용해 이전에 쓰던 해상도 설정 반영
        //Screen.SetResolution(nowWidthPixel, nowHeightPixel, isFullScreen);

        print(Display.main.systemWidth + " : " + Display.main.systemHeight);

        //////////////////////////////////////////////////////////////////////////
        
        hdList.Clear();
        currentList.Clear();

        List<Resolution> monitorResolutions = Screen.resolutions.ToList();

        float hdNum = 16f / 9f;
        float monitorNum = (float)Display.main.systemWidth / (float)Display.main.systemHeight;
        float itemNum = 0f;
        foreach (Resolution item in monitorResolutions)
        {
            itemNum = (float)item.width / (float)item.height;

            if (Mathf.Approximately(hdNum, itemNum))
            {
                ListValueDuplicateCheck(hdList, item);
            }

            if (Mathf.Approximately(monitorNum, itemNum))
            {
                ListValueDuplicateCheck(currentList, item);
            }

        }



    }

    private void OnEnable()
    {
        //현재 적용된 화면 해상도와 드롭다운에 있는 해상도를 비교하여 자동으로 같은 해상도를 선택해야함
        //드롭다운 아이템 현재 리스트로 교체
        RedefineDropdown();
        print(nowList.Count);
        for (int i = 0; i < nowList.Count; i++)
        {
            if (nowWidthPixel == nowList[i].x && nowHeightPixel == nowList[i].y)
            {
                print(nowWidthPixel + " : " + nowList[i].x + " : " + nowHeightPixel + " : " + nowList[i].y);
                //0일때 0으로 드롭다운 value를 바꿔도 변화가 없기 때문에 수동으로 메소드 실행해줌
                if (i == 0)
                {
                    resolutiondropdown.value = i;
                    EnterResolution(i);
                }
                //드롭다운 아이템 저장된 값으로 드롭다운 아이템 선택
                resolutiondropdown.value = i;
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

        if (Input.GetKeyDown(KeyCode.M))
        {
            //이 코드 실행시 실제로 드롭다운 아이템에 해당하는 해상도로 변경됨
            resolutiondropdown.value = 2;
        }
    }

    private void ListValueDuplicateCheck(List<Vector2> list, Resolution item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            //같은게 있으면 넣지 않고 for문 종료
            if (list[i].x == item.width)
            {
                return;
            }
        }
        //리스트 안에 같은게 없으면 리스트에 삽입
        list.Add(new Vector2(item.width, item.height));
    }

    private void RedefineDropdown()
    {
        print("리디파인 드롭다운");
        float CRITERIA_NUM = 16f / 9f;

        //16:9보다 가로가 더 긴 모니터의 경우 16:9 해상도만 나옴
        if (CRITERIA_NUM < Display.main.systemWidth / Display.main.systemHeight || isFullScreen == false)
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
        else if (isFullScreen == true)
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

    //풀스크린으로 만드는 버튼(스위치 버튼 누를때 자동호출)
    //풀스크린과 창모드 드롭다운 아이템이 다르니 스위치 눌리면 드롭다운 인덱스 0번으로 적용하는 걸로
    public void FullScreenSwitch()
    {
        isFullScreen = !isFullScreen;
        SaveManager.Instance.SaveIsFullScreen(isFullScreen);
        fullScreenSwitch.sprite = isFullScreen ? onImage : offImage;
        insideImage.sprite = isFullScreen ? fullscreenInside : windowedInside;

        //해상도 메뉴 목록 재정의
        RedefineDropdown();

        //변경할 드롭다운이 현재 드롭다운과 번호가 같을때 대비 0으로 바꿔준뒤 다른 인덱스 적용
        resolutiondropdown.value = 0;
        resolutiondropdown.value = nowList.Count - 1;
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

    //해상도 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
    public void EnterResolution(int value)
    {
        print("엔터 리솔루션");
        float CRITERIA_NUM = 16f / 9f;

        //리스트값으로 매개변수 받아서 넣어주면 될듯
        //'전체화면'이면서 기준값 1.77 '미만'일때 - currentResolutions
        if (isFullScreen == true && (CRITERIA_NUM > Display.main.systemWidth / Display.main.systemHeight))
        {
            StartCoroutine(ResolutionWindow(currentList[value].x, currentList[value].y));
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage2(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage2((int)currentList[value].x, (int)currentList[value].y, inside);
        }
        //'창모드'이거나, '전체화면'이면서 기준값 1.77 '이상'일때 - hdResolutions
        else
        {
            StartCoroutine(ResolutionWindow(hdList[value].x, hdList[value].y));
            //항상 아웃사이드 먼저 해줘야함
            ResizePreviewImage2(Display.main.systemWidth, Display.main.systemHeight, outside);
            ResizePreviewImage2((int)hdList[value].x, (int)hdList[value].y, inside);
        }
    }

    private void ResizePreviewImage2(float targetWidth, float targetHeight, RectTransform rect)
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

    private void ResizePreviewImage(int width, int height, RectTransform rect)
    {
        //가로와 세로의 최대공약수 구함
        int gcd = GCD(width, height);
        //각 세로와 가로를 최대공약수로 나누어 해상도의 비율을 알아냄
        width = width / gcd;
        height = height / gcd;

        //비율에 곱해야하는 기준값
        int temp = 0;
        //자신을 감싸고 있는 부모의 가로 혹은 세로
        int parentWidth = 0;
        int parentHeight = 0;

        //자신을 감싸고 있는 오브젝트의 가로, 세로 길이를 대입
        //부모가 패널일 때
        if (rect == outside)
        {
            parentWidth = 1000;
            parentHeight = 1000;
        }
        //부모가 outside일 때
        else
        {
            parentWidth = (int)outside.rect.width;
            parentHeight = (int)outside.rect.height;
        }

        //목표 해상도 가로가 세로 수치 이상일때(가로로 길거나 1:1 비율의 화면이라는 뜻)
        if (width >= height)
        {
            //부모 영역에 최대한 꽉 차게 채울 수 있는 기준값을 구함
            temp = parentWidth / width;
            //세로값이 벗어날 수 있으니 기준값 조정해줌
            while (temp * height > parentHeight)
            {
                temp--;
            }
        }
        //목표 해상도 가로가 세로 수치 미만일때(세로가 길쭉한 화면이라는 뜻)
        else
        {
            //부모 영역에 최대한 꽉 차게 채울 수 있는 기준값을 구함
            temp = parentHeight / height;
            //가로값이 벗어날 수 있으니 기준값 조정해줌
            while (temp * height > parentWidth)
            {
                temp--;
            }
        }

        if (rect == inside)
        {
            print("temp : " + temp);
        }

        //비율에 기준값을 곱하여 부모 영역에 최대한 꽉 채울 width와 height를 구함
        width *= temp;
        height *= temp;

        if (rect == inside)
        {
            print("targetWidth : " + width);
            print("targetHeight : " + height);
        }

        if (rect == outside)
        {
            rect.sizeDelta = new Vector2(width, height);
        }
        else if (rect == inside)
        {
            //inside의 테두리가 outside와 겹칠 우려 있으므로 50씩 빼줌
            rect.sizeDelta = new Vector2(width - 50, height - 50);
        }
    }

    private int GCD(int a, int b)
    {
        if (a == b)
        {
            return a;
        }

        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    //프레임 드롭다운 아이템 클릭시 호출됨(자동으로 본인 인덱스를 매개변수로 전달)
    public void EnterFrameRate(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 30;
                SaveManager.Instance.SaveFrameRate(30);
                break;
            case 1:
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = 60;
                SaveManager.Instance.SaveFrameRate(60);
                break;
        }
    }
}
