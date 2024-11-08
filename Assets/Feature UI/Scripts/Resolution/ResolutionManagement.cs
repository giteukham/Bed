using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] Text screenText;

    int temp = 0;
    int nowWidth = 0;
    int nowHeight = 0;

    //const float CRITERIA_NUM = 16 / 9;
    bool change = true;

    private void Awake()
    {
        //cam.clearFlags = CameraClearFlags.SolidColor;
        //cam.backgroundColor = Color.black;
        //GL.Clear(true, true, Color.black);
        //ChangeResolution(32, 9);
        //Hi();
    }

    private void Update()
    {

        //screenText.text = Screen.width + "";
        screenText.text = nowWidth + " : " + nowHeight;

        if (Input.GetKeyDown(KeyCode.N))
        {
            //GL.Clear(true, true, Color.black);
            //print("black");
            Hi();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Bye();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("눌림");

            switch (temp)
            {
                case 0:
                    temp++;
                    Screen.SetResolution(1440, 1080, change);
                    TestResolution();
                    break;

                case 1:
                    temp++;
                    Screen.SetResolution(1920, 1200, change);
                    TestResolution();
                    break;

                case 2:
                    temp++;
                    Screen.SetResolution(1920, 1080, change);
                    TestResolution();
                    break;

                case 3:
                    temp++;
                    Screen.SetResolution(2560, 1080, change);
                    TestResolution();
                    break;

                case 4:
                    temp = 0;
                    Screen.SetResolution(3840, 1080, change);
                    TestResolution();
                    change = !change;
                    break;
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width">'목표' 가로비율로 명명함</param>
    /// <param name="height">'목표' 세로비율로 명명함</param>
    private void TestResolution()
    {
        GL.Clear(true, true, Color.black);  // 화면을 검은색으로 지움

        float width = Screen.width;
        float height = Screen.height;

        //16 / 9값과 비교하여 16:9 화면에서 세로길이 혹은 가로길이 중에서
        //어느 길이가 더 긴지 알아내는 판별용 변수
        float checkedValue = Screen.width / Screen.height;
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

    private void Hi()
    {
        Screen.SetResolution(1440, 1080, true);
        print("dd");
    }

    private void Bye()
    {
        Screen.SetResolution(1920, 1080, true);
        print("dd");
    }

    //레터박스 완전 검은색으로 나오게 함
    //void OnPreCull() => GL.Clear(true, true, Color.black);

    public void ChangeResolution(float width, float height)
    {
        nowWidth = (int)width;
        nowHeight = (int)height;
        //cam = GetComponent<Camera>();
        //Rect rect = cam.rect;
        Rect rect = new Rect(0, 0, 1, 1);
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)width / height); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        cam.rect = rect;

        print(cam.rect);
    }

    //void OnPreCull() => GL.Clear(true, true, Color.black);


    private int ScreenSizeX = 0;
    private int ScreenSizeY = 0;

    private void RescaleCamera(float width, float height)
    {

        //if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;
        //Camera cam2 = Camera.main;
        //cam2.Render();  // 카메라가 현재 프레임을 다시 렌더링하게 함
        GL.Clear(true, true, Color.black);  // 화면을 검은색으로 지움
        print("asdfafdd");
        nowWidth = (int)width;
        nowHeight = (int)height;

        //float targetaspect = 16.0f / 9.0f;
        float targetaspect = width / height;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        //Camera camera = GetComponent<Camera>();

        if (scaleheight < 1.0f)
        {
            Rect rect = cam.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            cam.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = cam.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            cam.rect = rect;
        }

        ScreenSizeX = Screen.width;
        ScreenSizeY = Screen.height;
    }

    void OnPreCull()
    {
        if (Application.isEditor) return;
        Rect wp = Camera.main.rect;
        Rect nr = new Rect(0, 0, 1, 1);

        Camera.main.rect = nr;
        GL.Clear(true, true, Color.red);

        Camera.main.rect = wp;

    }
}
