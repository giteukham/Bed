using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] Text screenText;

    int temp = 0;

    bool change = true;
    int nowWidthPixel = 0;
    int nowHeightPixel = 0;


    private void Awake()
    {
        // 1920 x 1080으로 시작
        nowWidthPixel = 1920;
        nowHeightPixel = 1080;
        Screen.SetResolution(nowWidthPixel, nowHeightPixel, change);
        //screenText.text = nowWidthPixel + " x " + nowHeightPixel;

    }

    private void Update()
    {
        //'모니터'의 현재 해상도를 가져옴
        screenText.text = Display.main.systemWidth + " " + Display.main.systemHeight;

        if (Input.GetKeyDown(KeyCode.M))
        {
            Screen.SetResolution(100, 100, change);
            change = !change;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("눌림");

            switch (temp)
            {
                case 0:
                    temp++;
                    nowWidthPixel = 1440;
                    nowHeightPixel = 1080;
                    break;

                case 1:
                    temp++;
                    nowWidthPixel = 1920;
                    nowHeightPixel = 1200;
                    break;

                case 2:
                    temp++;
                    nowWidthPixel = 1920;
                    nowHeightPixel = 1080;
                    break;

                case 3:
                    temp++;
                    nowWidthPixel = 2560;
                    nowHeightPixel = 1080;
                    break;

                case 4:
                    temp = 0;
                    nowWidthPixel = 3840;
                    nowHeightPixel = 1080;
                    change = !change;
                    break;
            }
            StartCoroutine(ResolutionWindow(nowWidthPixel, nowHeightPixel));
        }

    }

    private IEnumerator ResolutionWindow(float width, float height)
    {
        Screen.SetResolution((int)width, (int)height, change);

        yield return null;

        RescaleWindow(width, height);
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

        //만약 Screen.SetResolution(3840, 1080, change);가 실행됐다면
        //width는 3840, height는 1080임
        //float width = Screen.width;
        //float height = Screen.height;

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
        switch (value)
        {
            case 0:
                StartCoroutine(ResolutionWindow(1440, 1080));
                break;
            case 1:
                StartCoroutine(ResolutionWindow(1920, 1200));
                break;
            case 2:
                StartCoroutine(ResolutionWindow(1920, 1080));
                break;
            case 3:
                StartCoroutine(ResolutionWindow(2560, 1080));
                break;
            case 4:
                StartCoroutine(ResolutionWindow(3840, 1080));
                break;
            default:
                break;
        }

        //StartCoroutine(ResolutionWindow(float.Parse(nums[0]), float.Parse(nums[1])));
    }
}
