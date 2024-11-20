using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] GameObject uiCanvas;   
    public bool isMenuScreenActive = false;
    #region Menu
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject soundSettingsScreen;
    [SerializeField] private GameObject resolutionSettingsScreen;
    [SerializeField] private GameObject mouseSettingsScreen;
    #endregion

    private void Update() 
    {
        ActivateUICanvas();

        if (Input.GetMouseButton(1)) ShowMenuScreen();
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
        if (PlayerConstant.isPlayerStop == false && mouseSettingsScreen.activeSelf == false)
        {
            uiCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
