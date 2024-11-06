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
                    print("눌림1");
                    temp++;
                    print("눌림2");
                    RescaleCamera(4, 3);
                    print("눌림3");
                    break;

                case 1:
                    temp++;
                    RescaleCamera(16, 10);
                    break;

                case 2:
                    temp++;
                    RescaleCamera(16, 9);
                    break;

                case 3:
                    temp++;
                    RescaleCamera(21, 9);
                    break;

                case 4:
                    temp = 0;
                    RescaleCamera(32, 9);
                    break;
            }

        }
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

            //rect.width = 1.0f;
            rect.width = scaleheight;
            rect.height = scaleheight;
            //rect.x = 0;
            rect.x = (1.0f - scaleheight) / 2.0f;
            rect.y = (1.0f - scaleheight) / 2.0f;

            cam.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = cam.rect;

            rect.width = scalewidth;
            //rect.height = 1.0f;
            rect.height = scalewidth;
            rect.x = (1.0f - scalewidth) / 2.0f;
            //rect.y = 0;
            rect.y = (1.0f - scalewidth) / 2.0f;

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
