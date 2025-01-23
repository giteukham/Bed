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
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject soundSettingsScreen;
    [SerializeField] private GameObject resolutionSettingsScreen;
    [SerializeField] private GameObject mouseSettingsScreen;
    #endregion

    #region Manuals
    [SerializeField] private GameObject eyeOpenManual;
    [SerializeField] private GameObject leftMoveManaul;
    [SerializeField] private GameObject openOptionManual;
    [SerializeField] private GameObject blinkManaual;
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

    private void ShowManual(GameObject manual, bool isActive)
    {
        if (manual.activeSelf == isActive) return;

        manual.SetActive(isActive);
        DOTweenAnimation[] dOTweenAnimations = manual.GetComponentsInChildren<DOTweenAnimation>();
        foreach (var child in dOTweenAnimations) child.DORestart();
    }

    public void EyeOpenManual(bool isActive)
    {
        ShowManual(eyeOpenManual, isActive);
    }

    public bool GetEyeOpenManaul()
    {
        return eyeOpenManual.activeSelf;
    }

    public void LeftMoveManual(bool isActive)
    {
        ShowManual(leftMoveManaul, isActive);
    }

    public bool GetLeftMoveManual()
    {
        return leftMoveManaul.activeSelf;
    }

    public void OpenOptionManual(bool isActive)
    {
        ShowManual(openOptionManual, isActive);
    }

    public bool GetOpenOptionManual()
    {
        return openOptionManual.activeSelf;
    }

    public void BlinkManual(bool isActive)
    {
        ShowManual(blinkManaual, isActive);
    }

    public bool GetBlinkManual()
    {
        return blinkManaual.activeSelf;
    }
}
