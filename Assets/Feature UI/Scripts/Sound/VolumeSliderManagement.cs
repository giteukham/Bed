using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderManagement : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider gimmickVolumeSlider;
    [SerializeField] private Slider playerVolumeSlider;

    FMOD.Studio.Bus masterBus;
    FMOD.Studio.Bus gimmickBus;
    FMOD.Studio.Bus playerBus;

    private void OnEnable()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        gimmickBus = FMODUnity.RuntimeManager.GetBus("bus:/Gimmick SFX");
        playerBus = FMODUnity.RuntimeManager.GetBus("bus:/Player SFX");

        LoadSettings();
        
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
        SaveManager.Instance.SaveVolumes(masterVolumeSlider.value, gimmickVolumeSlider.value, playerVolumeSlider.value);
    }

    private void LoadSettings()
    {
        SaveManager.Instance.LoadVolumes(out float masterVolume, out float gimmickVolume, out float playerVolume);

        masterVolumeSlider.value = masterVolume;
        gimmickVolumeSlider.value = gimmickVolume;
        playerVolumeSlider.value = playerVolume;
    }
}
