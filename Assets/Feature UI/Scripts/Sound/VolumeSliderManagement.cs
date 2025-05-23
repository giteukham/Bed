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

    FMOD.Studio.Bus masterBus;
    FMOD.Studio.Bus gimmickBus;
    FMOD.Studio.Bus playerBus;

    private void OnEnable()
    {
        masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume();});
        gimmickVolumeSlider.onValueChanged.AddListener(delegate { SetGimmickVolume();});
        playerVolumeSlider.onValueChanged.AddListener(delegate { SetPlayerVolume();});
    }

    private void SetMasterVolume()
    {
        masterBus.setVolume(masterVolumeSlider.value);
    }

    private void SetGimmickVolume()
    {
        gimmickBus.setVolume(gimmickVolumeSlider.value);
    }

    private void SetPlayerVolume()
    {
        playerBus.setVolume(playerVolumeSlider.value);
    }

    public void UpMasterVolume()
    {
        masterVolumeSlider.value += 0.1f;
    }

    public void DownMasterVolume()
    {
        masterVolumeSlider.value -= 0.1f;
    }

    public void UpGimmickVolume()
    {
        gimmickVolumeSlider.value += 0.1f;
    }

    public void DownGimmickVolume()
    {
        gimmickVolumeSlider.value -= 0.1f;
    }

    public void UpPlayerVolume()
    {
        playerVolumeSlider.value += 0.1f;
    }

    public void DownPlayerVolume()
    {
        playerVolumeSlider.value -= 0.1f;
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

        masterBus.setVolume(PlayerPrefs.GetFloat("MasterVolume", 1));
        gimmickBus.setVolume(PlayerPrefs.GetFloat("GimmickVolume", 1));
        playerBus.setVolume(PlayerPrefs.GetFloat("PlayerVolume", 1));
        
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        gimmickVolumeSlider.value = PlayerPrefs.GetFloat("GimmickVolume", 1);
        playerVolumeSlider.value = PlayerPrefs.GetFloat("PlayerVolume", 1);
    }

    private void SaveSettings()
    {
        SaveManager.Instance.SaveMasterVolume(masterVolumeSlider.value);
        SaveManager.Instance.SaveGimmickVolume(gimmickVolumeSlider.value);
        SaveManager.Instance.SavePlayerVolume(playerVolumeSlider.value);
    }
}
