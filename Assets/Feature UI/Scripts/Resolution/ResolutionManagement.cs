using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] Camera cam;
    [SerializeField] Text screenText;

    private void Update()
    {

        screenText.text = Screen.width + "";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("눌림");
            switch (Screen.width)
            {
                case 3840:
                    print("2560, 1440");
                    Screen.SetResolution(2560, 1440, true);
                    RescaleCamera(2560, 1440);
                    //ChangeResolution(2560, 1440);
                    break;
                case 2560:
                    print("1920, 1080");
                    Screen.SetResolution(1920, 1080, true);
                    RescaleCamera(1920, 1080);
                    //ChangeResolution(1920, 1080);
                    break;
                case 1920:
                    print("1280, 720");
                    Screen.SetResolution(1280, 720, true);
                    RescaleCamera(1280, 720);
                    //ChangeResolution(1280, 720);
                    break;
                case 1280:
                    print("720, 480");
                    Screen.SetResolution(720, 480, true);
                    RescaleCamera(720, 480);
                    //ChangeResolution(720, 480);
                    break;
                case 720:
                    print("3840, 2160");
                    Screen.SetResolution(3840, 2160, true);
                    RescaleCamera(3840, 2160);
                    //ChangeResolution(3840, 2160);
                    break;
            }
        }
    }

    //레터박스 완전 검은색으로 나오게 함
    //void OnPreCull() => GL.Clear(true, true, Color.black);

    public void ChangeResolution(float width, float height)
    {
        //cam = GetComponent<Camera>();
        Rect rect = cam.rect;
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
    }


    private int ScreenSizeX = 0;
    private int ScreenSizeY = 0;

    private void RescaleCamera(float width, float height)
    {

        if (Screen.width == ScreenSizeX && Screen.height == ScreenSizeY) return;

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
        GL.Clear(true, true, Color.black);

        Camera.main.rect = wp;

    }
}
