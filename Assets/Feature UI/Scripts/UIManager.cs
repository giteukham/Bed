using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>
{
    enum SettingScreenType
    {
        Off,
        Menu,
        SoundSettings,
        ResolutionSettings,
        MouseSettings
    }
    
    [SerializeField] GameObject menuUI;   
    //public bool isMenuScreenActive = false;
    #region Menu
    [Header("Menu")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject soundSettingsScreen;
    [SerializeField] private GameObject resolutionSettingsScreen;
    [SerializeField] private GameObject mouseSettingsScreen;
    #endregion
    
    #region Players
    [Header("Player")]
    [SerializeField] private Player player;
    #endregion

    private SettingScreenType currentScreenType = SettingScreenType.Off;
    
    public Action OnMenuScreenActive, OnMenuScreenDeactive;
    public Action OnSoundSettingsScreenActive, OnSoundSettingsScreenDeactive;
    public Action OnResolutionSettingsScreenActive, OnResolutionSettingsScreenDeactive;
    public Action OnMouseSettingsScreenActive, OnMouseSettingsScreenDeactive;

    #region Bible Verses
    [SerializeField] private GameObject deut;
    [SerializeField] private GameObject neh;
    #endregion

    public bool isRightClikHeld = false;
    private float rightClickStartTime  = 0f;

    private void Awake()
    {
        VolumeSliderManagement SoundSettings = soundSettingsScreen.GetComponent<VolumeSliderManagement>();
        SoundSettings.LoadSoundSettings();          // 사운드 값 불러오기
        MouseSettings.Instance.InitMouseSetting();  // 마우스 값 불러오기
        resolutionSettingsScreen.GetComponent<ResolutionManagement>().InitResolutionSetting(); // 해상도 값 불러오기


        if (menuUI.activeSelf) menuUI.SetActive(false);
        ToggleScreenObject(SettingScreenType.Off, false);
    }

    private void Update() 
    {
        ActivateUICanvas();

        if (Input.GetMouseButtonDown(1) && !menuScreen.activeSelf) ShowMenuScreen();
        else if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0) && menuScreen.activeSelf) 
        {
            PlayerConstant.isPlayerStop = false;
        }
        
        if (menuUI.activeSelf && menuScreen.activeSelf && Input.GetMouseButton(1)) 
        {
            if (!isRightClikHeld) rightClickStartTime = Time.time;
            isRightClikHeld = true;
        }
        if (Input.GetMouseButtonUp(1) || (isRightClikHeld && !menuUI.activeSelf && Time.time - rightClickStartTime >= GameManager.Instance.bothClickToleranceTime)) isRightClikHeld = false;
    }


    /// <summary>
    /// 처음 시작할 때 메뉴 안에 있는 화면을 비활성화 시키는 함수
    /// 11-22 최무령
    /// </summary>
    private void ToggleScreenObject(SettingScreenType screenType, bool isActive)
    {
        switch (screenType)
        {
            case SettingScreenType.Off:
                menuScreen.SetActive(false);
                soundSettingsScreen.SetActive(false);
                resolutionSettingsScreen.SetActive(false);
                mouseSettingsScreen.SetActive(false);
                break;
            case SettingScreenType.Menu:
                if (isActive) OnMenuScreenActive?.Invoke();
                else OnMenuScreenDeactive?.Invoke();
                
                menuScreen.SetActive(isActive);
                break;
            case SettingScreenType.SoundSettings:
                if (isActive) OnSoundSettingsScreenActive?.Invoke();
                else OnSoundSettingsScreenDeactive?.Invoke();
                
                soundSettingsScreen.SetActive(isActive);
                break;
            case SettingScreenType.ResolutionSettings:
                if (isActive) OnResolutionSettingsScreenActive?.Invoke();
                else OnResolutionSettingsScreenDeactive?.Invoke();
                
                resolutionSettingsScreen.SetActive(isActive);
                break;
            case SettingScreenType.MouseSettings:
                if (isActive) OnMouseSettingsScreenActive?.Invoke();
                else OnMouseSettingsScreenDeactive?.Invoke();
                
                mouseSettingsScreen.SetActive(isActive);
                break;
        }
    }

    private void ShowScreen(SettingScreenType nextScreenType)
    {
        if (currentScreenType == nextScreenType) return;
        ToggleScreenObject(currentScreenType, false);
        ToggleScreenObject(nextScreenType, true);
        currentScreenType = nextScreenType;
    }

    public void ShowMenuScreen()
    {
        ShowScreen(SettingScreenType.Menu);
    }

    public void ShowSoundSettingsScreen()
    {
        ShowScreen(SettingScreenType.SoundSettings);
    }

    public void ShowResolutionSettingsScreen()
    {
        ShowScreen(SettingScreenType.ResolutionSettings);
    }

    public void ShowMouseSettingsScreen()
    {
        ShowScreen(SettingScreenType.MouseSettings);
    }

    /// <summary>
    /// 수정 날짜 : 2024-11-14 최무령
    /// </summary>
    public void ActivateUICanvas()
    {
        // if (isActivate)
        // {
        //     player.StopPlayer(true);
        //     uiCanvas.SetActive(true);
        //     Cursor.visible = true;
        //     Cursor.lockState = CursorLockMode.None;
        //     ShowScreen(SettingScreenType.Menu);
        // }
        // else
        // {
        //     player.StopPlayer(false);
        //     uiCanvas.SetActive(false);
        //     Cursor.visible = false;
        //     Cursor.lockState = CursorLockMode.Locked;
        //     ShowScreen(SettingScreenType.Off);
        // }
        
        if (PlayerConstant.isPlayerStop == true)
        {
            if (PlayerConstant.isEyeOpen == false && !menuUI.activeSelf) 
            {
                menuUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        else Cursor.visible = false;
        
        if (PlayerConstant.isPlayerStop == false)
        {
            menuUI.SetActive(false);
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
