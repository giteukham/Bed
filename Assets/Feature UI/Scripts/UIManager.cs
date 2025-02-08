using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    [SerializeField] GameObject uiCanvas;   
    public bool isMenuScreenActive = false;
    #region Menu
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

    private void Update() 
    {
        if (isMenuScreenActive == true)
        {
            if (Input.GetMouseButton(1)) ShowMenuScreen();
        }
    }

    private void Awake()
    {
        ToggleScreenObject(SettingScreenType.Off, false);
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
    public void ActivateUICanvas(bool isActivate)
    {
        if (isActivate)
        {
            player.StopPlayer(true);
            isMenuScreenActive = true;
            uiCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            ShowScreen(SettingScreenType.Menu);
        }
        else
        {
            player.StopPlayer(false);
            isMenuScreenActive = false;
            uiCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            ShowScreen(SettingScreenType.Off);
        }
        
        // if (PlayerConstant.isPlayerStop == true)
        // {
        //     if (PlayerConstant.isEyeOpen == false) 
        //     {
        //         uiCanvas.SetActive(true);
        //         Cursor.visible = true;
        //         Cursor.lockState = CursorLockMode.None;
        //     }
        // }
        // else Cursor.visible = false;
        //
        // if (PlayerConstant.isPlayerStop == false)
        // {
        //     uiCanvas.SetActive(false);
        //     ShowMenuScreen();
        //     Cursor.visible = false;
        //     Cursor.lockState = CursorLockMode.Locked;
        // }
    }
}
