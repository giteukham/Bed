using System;
using UnityEngine;

// 해당 클래스가 들어 있는 오브젝트가 시작할 때 활성화 되어 있으면 오류 뜸.
public class MouseWindowUI : MonoBehaviour, IWindowUIBase
{
    public static event Action OnScreenActive, OnScreenDeactive;

    private void Start()
    {
        OnScreenActive?.Invoke();
    }
    
    private void OnDisable()
    {
        OnScreenDeactive?.Invoke();
    }
}
