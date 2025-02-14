using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] GameObject uiCanvas;   
    //public bool isMenuScreenActive = false;
    #region Menu
    [Header("Menu")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject soundSettingsScreen;
    [SerializeField] private GameObject resolutionSettingsScreen;
    [SerializeField] private GameObject mouseSettingsScreen;
    #endregion

    #region Bible Verses
    [SerializeField] private GameObject deut;
    [SerializeField] private GameObject neh;
    #endregion

    public bool isRightClikHeld = false;
    private float rightClickStartTime  = 0f;

    private void Update() 
    {
        ActivateUICanvas();

        if (Input.GetMouseButtonDown(1) && !menuScreen.activeSelf) ShowMenuScreen();
        else if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0) && menuScreen.activeSelf) PlayerConstant.isPlayerStop = false;

        
        if (uiCanvas.activeSelf && menuScreen.activeSelf && Input.GetMouseButton(1)) 
        {
            if (!isRightClikHeld) rightClickStartTime = Time.time;
            isRightClikHeld = true;
        }
        if (Input.GetMouseButtonUp(1) || (isRightClikHeld && !uiCanvas.activeSelf && Time.time - rightClickStartTime >= GameManager.Instance.bothClickToleranceTime)) isRightClikHeld = false;
    }

    private void Awake()
    {
        DeActivateActivatedScreen();
    }

    /// <summary>
    /// 처음 시작할 때 메뉴 안에 있는 화면을 비활성화 시키는 함수
    /// 11-22 최무령
    /// </summary>
    private void DeActivateActivatedScreen()
    {
        if (menuScreen.activeSelf)
        {
            menuScreen.SetActive(false);
        }
        if (soundSettingsScreen.activeSelf)
        {
            soundSettingsScreen.SetActive(false);
        }
        if (resolutionSettingsScreen.activeSelf)
        {
            resolutionSettingsScreen.SetActive(false);
        }
        if (mouseSettingsScreen.activeSelf)
        {
            Debug.LogError("마우스 설정 스크린은 게임 시작 전에 비활성화 되어야 합니다.");
        }
    }

    private void OnEnable()
    {
        ShowMenuScreen();
    }

    private void ShowScreen(GameObject _screen)
    {
        if(_screen.activeSelf) return;
        menuScreen.SetActive(false);
        soundSettingsScreen.SetActive(false);
        resolutionSettingsScreen.SetActive(false);
        mouseSettingsScreen.SetActive(false);

        _screen.SetActive(true);
    }

    public void ShowMenuScreen()
    {
        ShowScreen(menuScreen);
    }

    public void ShowSoundSettingsScreen()
    {
        ShowScreen(soundSettingsScreen);
    }

    public void ShowResolutionSettingsScreen()
    {
        ShowScreen(resolutionSettingsScreen);
    }

    public void ShowMouseSettingsScreen()
    {
        ShowScreen(mouseSettingsScreen);
    }

    /// <summary>
    /// 수정 날짜 : 2024-11-14 최무령
    /// </summary>
    void ActivateUICanvas()
    {
        if (PlayerConstant.isPlayerStop == true)
        {
            if (PlayerConstant.isEyeOpen == false) 
            {
                uiCanvas.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else Cursor.visible = false;
        
        if (PlayerConstant.isPlayerStop == false)
        {
            uiCanvas.SetActive(false);
            ShowMenuScreen();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void DeutActivate(bool isActive)
    {
        deut.SetActive(isActive);
    }

    public void NehActivate(bool isActive)
    {
        neh.SetActive(isActive);
    }
}
