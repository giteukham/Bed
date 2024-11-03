using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManagement : MonoBehaviour
{

    [SerializeField] Camera cam;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("눌림");
            switch (Screen.width)
            {
                case 2560:
                    print("1920, 1080");
                    Screen.SetResolution(1920, 1080, true);
                    //ChangeResolution(1920, 1080);
                    break;
                case 1920:
                    print("1280, 720");
                    Screen.SetResolution(1280, 720, true);
                    //ChangeResolution(1280, 720);
                    break;
                case 1280:
                    print("720, 480");
                    Screen.SetResolution(720, 480, true);
                    //ChangeResolution(720, 480);
                    break;
                case 720:
                    print("2560, 1440");
                    Screen.SetResolution(2560, 1440, true);
                    //ChangeResolution(2560, 1440);
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
}
