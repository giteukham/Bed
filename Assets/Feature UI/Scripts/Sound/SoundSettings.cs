using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider gimmickVolumeSlider;
    [SerializeField] private Slider playerVolumeSlider;

    private float _masterVolume, _gimmickVolume, _playerVolume;

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            masterVolumeSlider.value = value;
            AudioManager.Instance.MasterBus.setVolume(value);
        }
    }

    public float GimmickVolume
    {
        get => _gimmickVolume;
        set
        {
            _gimmickVolume = value;
            gimmickVolumeSlider.value = value;
            AudioManager.Instance.GimmickBus.setVolume(value);
        }
    }

    public float PlayerVolume
    {
        get => _playerVolume;
        set
        {
            _playerVolume = value;
            playerVolumeSlider.value = value;
            AudioManager.Instance.PlayerBus.setVolume(value);
        }
    }


    private void SetMasterVolume()
    {
        MasterVolume = masterVolumeSlider.value;
    }

    private void SetGimmickVolume()
    {
        GimmickVolume = gimmickVolumeSlider.value;
    }

    private void SetPlayerVolume()
    {
        PlayerVolume = playerVolumeSlider.value;
    }

    public void UpMasterVolume()
    {
        MasterVolume += 0.1f;
    }

    public void DownMasterVolume()
    {
        MasterVolume -= 0.1f;
    }

    public void UpGimmickVolume()
    {
        GimmickVolume += 0.1f;
    }

    public void DownGimmickVolume()
    {
        GimmickVolume -= 0.1f;
    }

    public void UpPlayerVolume()
    {
        PlayerVolume += 0.1f;
    }

    public void DownPlayerVolume()
    {
        PlayerVolume -= 0.1f;
    }

    private void OnDisable()
    {
        SaveSettings();
    }

    public void InitSoundSettings()
    {
        masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume();});
        gimmickVolumeSlider.onValueChanged.AddListener(delegate { SetGimmickVolume();});
        playerVolumeSlider.onValueChanged.AddListener(delegate { SetPlayerVolume();});

        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        GimmickVolume = PlayerPrefs.GetFloat("GimmickVolume", 1);
        PlayerVolume = PlayerPrefs.GetFloat("PlayerVolume", 1);
        
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        gimmickVolumeSlider.value = PlayerPrefs.GetFloat("GimmickVolume", 1);
        playerVolumeSlider.value = PlayerPrefs.GetFloat("PlayerVolume", 1);
        Debug.Log("ㅁㄴㅇㄹ");
    }

    private void SaveSettings()
    {
        SaveManager.Instance.SaveMasterVolume(masterVolumeSlider.value);
        SaveManager.Instance.SaveGimmickVolume(gimmickVolumeSlider.value);
        SaveManager.Instance.SavePlayerVolume(playerVolumeSlider.value);
    }
}
