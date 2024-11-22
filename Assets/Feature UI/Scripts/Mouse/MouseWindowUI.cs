using System;
using UnityEngine;

public class MouseWindowUI : MonoBehaviour, IWindowUIBase
{
    public static event Action OnScreenActive, OnScreenDeactive;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (Application.isPlaying) OnScreenActive?.Invoke();
    }
    
    private void OnDisable()
    {
        if (Application.isPlaying) OnScreenDeactive?.Invoke();
    }
}
