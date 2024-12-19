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

    private void Awake()
    {
        DeActivateActivatedScreen();
    }

    /// <summary>
    /// ó�� ������ �� �޴� �ȿ� �ִ� ȭ���� ��Ȱ��ȭ ��Ű�� �Լ�
    /// 11-22 �ֹ���
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
            Debug.LogError("���콺 ���� ��ũ���� ���� ���� ���� ��Ȱ��ȭ �Ǿ�� �մϴ�.");
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
    /// ���� ��¥ : 2024-11-14 �ֹ���
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
