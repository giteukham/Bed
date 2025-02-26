using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResolutionSettingsManegment : MonoBehaviour
{
    // 해상도 설정 값 데이터 저장용
    private int resolutionX;
    private int resolutionY;
    private int frameRate;
    private bool isWindowed;

    public UnityEvent<int, int> OnResolutionChanged = new UnityEvent<int, int>();
    public UnityEvent<int> OnFrameRateChanged = new UnityEvent<int>();
    public UnityEvent<bool> OnWindowModeChanged = new UnityEvent<bool>();

    private void Awake()
    {
        SaveManager.Instance.LoadResolution(out resolutionX, out resolutionY);
        frameRate = SaveManager.Instance.LoadFrameRate();
        isWindowed = SaveManager.Instance.LoadIsWindowedScreen();

        OnResolutionChanged.Invoke(resolutionX, resolutionY);
        OnFrameRateChanged.Invoke(frameRate);
        OnWindowModeChanged.Invoke(isWindowed);
    }

    // 인풋필드, 드롭다운에서 해상도 변경 했을때 호출
    public void SetResolution(int x, int y)
    {
        resolutionX = x;
        resolutionY = y;
        OnResolutionChanged.Invoke(x, y);
    }

    public void SetFrameRate(int frame)
    {
        frameRate = frame;
        OnFrameRateChanged.Invoke(frame);
    }

    public void SetWindowMode(bool windowed)
    {
        isWindowed = windowed;
        OnWindowModeChanged.Invoke(windowed);
    }

    // 프리뷰 패널에서 해상도 변경 했을때 호출
    public void OnPreviewResized(int x, int y)
    {
        resolutionX = x;
        resolutionY = y;
        OnResolutionChanged.Invoke(x, y);
    }
}

