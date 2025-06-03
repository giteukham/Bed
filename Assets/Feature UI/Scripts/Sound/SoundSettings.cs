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
            masterBus.setVolume(value);
        }
    }

    public float GimmickVolume
    {
        get => _gimmickVolume;
        set
        {
            _gimmickVolume = value;
            gimmickVolumeSlider.value = value;
            gimmickBus.setVolume(value);
        }
    }

    public float PlayerVolume
    {
        get => _playerVolume;
        set
        {
            _playerVolume = value;
            playerVolumeSlider.value = value;
            playerBus.setVolume(value);
        }
    }

    FMOD.Studio.Bus masterBus;
    FMOD.Studio.Bus gimmickBus;
    FMOD.Studio.Bus playerBus;


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
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        gimmickBus = FMODUnity.RuntimeManager.GetBus("bus:/Gimmick SFX");
        playerBus = FMODUnity.RuntimeManager.GetBus("bus:/Player SFX");

        masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume();});
        gimmickVolumeSlider.onValueChanged.AddListener(delegate { SetGimmickVolume();});
        playerVolumeSlider.onValueChanged.AddListener(delegate { SetPlayerVolume();});

        masterBus.setVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        gimmickBus.setVolume(PlayerPrefs.GetFloat("GimmickVolume", 1));
        playerBus.setVolume(PlayerPrefs.GetFloat("PlayerVolume", 1));
        
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
