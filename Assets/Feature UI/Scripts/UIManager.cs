using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField] GameObject uiCanvas;   
    public bool isMenuScreenActive = false;
    #region Menu
    [Header("Menu")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject soundSettingsScreen;
    [SerializeField] private GameObject resolutionSettingsScreen;
    [SerializeField] private GameObject mouseSettingsScreen;
    #endregion

    #region Tutorials
    [Header("Tutorials")]
    [SerializeField] private GameObject eyeOpenTutorial;
    [SerializeField] private GameObject leftMoveTutorial;
    [SerializeField] private GameObject openOptionTutorial;
    [SerializeField] private GameObject blinkTutorial;
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
            ShowMenuScreen();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void ShowTutorial(GameObject Tutorial, bool isActive)
    {
        if (Tutorial.activeSelf == isActive) return;

        Tutorial.SetActive(isActive);
        DOTweenAnimation[] dOTweenAnimations = Tutorial.GetComponentsInChildren<DOTweenAnimation>();
        foreach (var child in dOTweenAnimations) child.DORestart();
    }

    public void EyeOpenTutorial(bool isActive)
    {
        ShowTutorial(eyeOpenTutorial, isActive);
    }

    public bool GetEyeOpenManaul()
    {
        return eyeOpenTutorial.activeSelf;
    }

    public void LeftMoveTutorial(bool isActive)
    {
        ShowTutorial(leftMoveTutorial, isActive);
    }

    public bool GetLeftMoveTutorial()
    {
        return leftMoveTutorial.activeSelf;
    }

    public void OpenOptionTutorial(bool isActive)
    {
        ShowTutorial(openOptionTutorial, isActive);
    }

    public bool GetOpenOptionTutorial()
    {
        return openOptionTutorial.activeSelf;
    }

    public void BlinkTutorial(bool isActive)
    {
        ShowTutorial(blinkTutorial, isActive);
    }

    public bool GetBlinkTutorial()
    {
        return blinkTutorial.activeSelf;
    }
}
