using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set;}
    
    #region Menu
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject soundSettingsScreen;
    [SerializeField] private GameObject resolutionSettingsScreen;
    [SerializeField] private GameObject mouseSettingsScreen;
    #endregion

    private void Awake() 
    {
        if (instance != null) Debug.LogError("UI Manager already exists");
        instance = this;
    }

    private void Update() 
    {
        //Menu Screen���� esc ������ �����Ͻðڽ��ϱ�? â�� ������ ������
        //�׸��� �ٸ� ȭ�鿡�� escâ�� ������ ����ȭ������ ���°ɷ� �ٲ�� ������
        if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetMouseButtonDown(1))
        {
            ShowMenuScreen();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        //���߿� ����Ű�� uiâ ���� Ű�� �ٲ����
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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
}
