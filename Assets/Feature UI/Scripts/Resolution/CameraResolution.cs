using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 카메라에 다 넣어야 함
/// Camera cam = GetComponent<Camera>(); 이거 다르게 참조하려면 다른 스크립트에 박아도 괜찮을듯
/// 
/// </summary>
public class CameraResolution : MonoBehaviour
{
    void Awake()
    {
        /*Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)16 / 9); // (가로 / 세로)
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
        camera.rect = rect;*/
    }

    //레터박스 완전 검은색으로 나오게 함
    //void OnPreCull() => GL.Clear(true, true, Color.black);
}
