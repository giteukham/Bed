using System;
using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Bed.Collider;
using PSXShaderKit;
using System.Collections;

public enum PlayerDirectionStateTypes
{
    Left,
    Middle,
    Right,
    Switching
}

public enum PlayerEyeStateTypes
{
    Open,
    Opening,
    Close,
    Closing,
    Blink
}

[RequireComponent(typeof(PlayerAnimation))]
public class Player : PlayerBase
{
    #region Player Components
    [Header("State Machine")]
    [SerializeField] private StateMachine playerDirectionStateMachine;
    [SerializeField] private StateMachine playerEyeStateMachine;
    
    [Header("Player Camera")]
    [SerializeField] private Camera playerCamera;
    
    [Header("Cone Colider")]
    [SerializeField] private ConeCollider coneCollider;

    [Header("Sound Effect")]
    [SerializeField] private PillowSound pillowSound;
    
    #endregion

    #region Camera Effect Variables
    private CinemachinePostProcessing postProcessing;
    private ColorGrading colorGrading;
    private Grain grain;
    private ChromaticAberration chromaticAberration;
    private DepthOfField depthOfField;
    private PSXPostProcessEffect psxPostProcessEffect;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    #endregion

    #region Player Stats Updtae Variables
    private float updateInterval = 0.1f; // ?��?��?��?�� 주기
    private float timeSinceLastUpdate = 0f;

    private float currentHorizontalCameraMovement;
    private float currentVerticalCameraMovement;
    private float recentHorizontalCameraMovement;
    private float recentVerticalCameraMovement;
    private float isCameraMovement;

    private float currentHorizontalMouseMovement;
    private float currentVerticalMouseMovement;
    private float recentHorizontalMouseMovement;
    private float recentVerticalMouseMovement;
    private float deltaHorizontalMouseMovement;
    private float deltaVerticalMouseMovement;
    #endregion
    
    #region State Machine
    private PlayerDirectionControl playerDirectionControl;
    private PlayerEyeControl playerEyeControl;
    #endregion
    
    #region Sound Effect Variables
    private float currentFearSFXVolume, currentStressSFXVolume, currentHeadMoveSFXVolume;
    #endregion

    private void Start()
    {
        playerDirectionControl = new PlayerDirectionControl(playerDirectionStateMachine);
        playerEyeControl = new PlayerEyeControl(playerEyeStateMachine);
        playerEyeControl.SubscribeToEvents();
        // Post Processing
        postProcessing = playerVirtualCamera.GetComponent<CinemachinePostProcessing>();
        postProcessing.m_Profile.TryGetSettings(out colorGrading);
        postProcessing.m_Profile.TryGetSettings(out grain);
        postProcessing.m_Profile.TryGetSettings(out chromaticAberration);
        postProcessing.m_Profile.TryGetSettings(out depthOfField);
        psxPostProcessEffect = playerCamera.GetComponent<PSXPostProcessEffect>();

        AudioManager.Instance.PlaySound(AudioManager.Instance.stressHal, transform.position);
        StartCameraEffect();
        StartPostProcessing();
    }

    void Update() 
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateStats();
            timeSinceLastUpdate = 0f;
        }
        UpdateGauge();
        SetPlayerState();
        UpdatePostProcessing();
        UpdateSFX();
        StopPlayer();
        coneCollider.SetColider();
        
        #if UNITY_EDITOR
            coneCollider.SetDebugImage();
        #endif
    }
    
    public void AnimationEvent_ChangeDirectionState(string toState)
    {
        switch (toState)
        {   
            case "Left":
                playerDirectionStateMachine.ChangeState(playerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Left]);
                break;
            case "Middle":
                playerDirectionStateMachine.ChangeState(playerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Middle]);
                break;
            case "Right":
                playerDirectionStateMachine.ChangeState(playerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Right]);
                break;
            case "Switching":
                playerDirectionStateMachine.ChangeState(playerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Switching]);
                break;
        }
    }

    private void UpdateStats()
    {
        // ----------------- Head Movement -----------------
        float cameraDeltaX = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        float cameraDeltaY = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
        recentHorizontalCameraMovement = currentHorizontalCameraMovement;
        recentVerticalCameraMovement = currentVerticalCameraMovement;
        currentHorizontalCameraMovement = cameraDeltaX;
        currentVerticalCameraMovement = cameraDeltaY;
        if(currentVerticalCameraMovement == recentVerticalCameraMovement && currentHorizontalCameraMovement == recentHorizontalCameraMovement) isCameraMovement = 0;
        else isCameraMovement = 1;
        
        float mouseDeltaX = Mathf.Abs(MouseSettings.Instance.MouseHorizontalSpeed);
        float mouseDeltaY = Mathf.Abs(MouseSettings.Instance.MouseVerticalSpeed);
        recentHorizontalMouseMovement = currentHorizontalMouseMovement;
        recentVerticalMouseMovement = currentVerticalMouseMovement;
        currentHorizontalMouseMovement = mouseDeltaX;
        currentVerticalMouseMovement = mouseDeltaY;
        if(currentVerticalMouseMovement == recentVerticalMouseMovement) deltaVerticalMouseMovement = 0;
        else deltaVerticalMouseMovement = Mathf.Abs(currentVerticalMouseMovement - recentVerticalMouseMovement);
        if(currentHorizontalMouseMovement == recentHorizontalMouseMovement) deltaHorizontalMouseMovement = 0;
        else deltaHorizontalMouseMovement = Mathf.Abs(currentHorizontalMouseMovement - recentHorizontalMouseMovement);
        
        PlayerConstant.headMoveSpeed = (deltaHorizontalMouseMovement + deltaVerticalMouseMovement) * isCameraMovement * 10 ;
        PlayerConstant.HeadMovementCAT += (deltaHorizontalMouseMovement + deltaVerticalMouseMovement) * isCameraMovement;
        PlayerConstant.HeadMovementLAT += (deltaHorizontalMouseMovement + deltaVerticalMouseMovement) * isCameraMovement;
        // ----------------- Head Movement -----------------

        // ----------------- Look Value -----------------
        if (PlayerConstant.isEyeOpen == false) return;
        else
        {
            float eulerY = playerCamera.transform.eulerAngles.y;
            float eulerX = playerCamera.transform.eulerAngles.x;

            if (eulerY < 105f) 
            {
                PlayerConstant.LeftLookCAT += timeSinceLastUpdate;
                PlayerConstant.LeftLookLAT += timeSinceLastUpdate;
            }
            else if (eulerY < 175f)
            {
                PlayerConstant.LeftFrontLookCAT += timeSinceLastUpdate;
                PlayerConstant.LeftFrontLookLAT += timeSinceLastUpdate;
            }
            else if (eulerY <= 185f)
            {
                PlayerConstant.FrontLookCAT += timeSinceLastUpdate;
                PlayerConstant.FrontLookLAT += timeSinceLastUpdate;
            }
            else if (eulerY <= 250f)
            {
                PlayerConstant.RightFrontLookCAT += timeSinceLastUpdate;
                PlayerConstant.RightFrontLookLAT += timeSinceLastUpdate;
            }
            else
            {
                PlayerConstant.RightLookCAT += timeSinceLastUpdate;
                PlayerConstant.RightLookLAT += timeSinceLastUpdate;
            }

            if (eulerX > 330f)
            {
                PlayerConstant.UpLookCAT += timeSinceLastUpdate;
                PlayerConstant.UpLookLAT += timeSinceLastUpdate;
            }
            else
            {
                PlayerConstant.DownLookCAT += timeSinceLastUpdate;
                PlayerConstant.DownLookLAT += timeSinceLastUpdate;
            }
        }
        // ----------------- Look Value -----------------
    }

    private void StartCameraEffect()
    {
        StartCoroutine(StressShake());
    }

    IEnumerator StressShake()
    {
        float shakeIntensity;
        while (PlayerConstant.stressGauge >= 25)
        {
            if      (PlayerConstant.stressGauge >= 80) shakeIntensity = 1.3f;
            else if (PlayerConstant.stressGauge >= 65) shakeIntensity = 0.75f;
            else if (PlayerConstant.stressGauge >= 50) shakeIntensity = 0.35f;
            else shakeIntensity = 0.1f;

            playerVirtualCamera.m_Lens.Dutch = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateGauge() 
    {
        if (PlayerConstant.stressGauge >= PlayerConstant.stressGaugeMax) PlayerConstant.isFainting = true;
        else PlayerConstant.isFainting = false;
        
        // if (PlayerConstant.fearGauge >= PlayerConstant.fearGaugeMax) PlayerConstant.isParalysis = true;
        // else PlayerConstant.isParalysis = false;

        PlayerConstant.stressGauge = Mathf.Clamp(PlayerConstant.stressGauge, PlayerConstant.stressGaugeMin, PlayerConstant.stressGaugeMax);
        PlayerConstant.fearGauge = Mathf.Clamp(PlayerConstant.fearGauge, PlayerConstant.fearGaugeMin, PlayerConstant.fearGaugeMax);
    }

    private void StartPostProcessing()
    {
        StartCoroutine(ChromaticAberrationEffect());
    }

    IEnumerator ChromaticAberrationEffect()
    {
        while (true)
        {
            chromaticAberration.intensity.value = PlayerConstant.stressGauge * 0.01f;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.12f));
            chromaticAberration.intensity.value = PlayerConstant.stressGauge * 0.01f / UnityEngine.Random.Range(2f, 4f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.1f));
        }
    }

    private void UpdatePostProcessing()
    {
        grain.intensity.value = PlayerConstant.fearGauge * 0.01f;

        psxPostProcessEffect._PixelationFactor = Mathf.Lerp(PlayerConstant.pixelationFactor, PlayerConstant.pixelationFactor * 0.4f, PlayerConstant.fearGauge / 100f);
        colorGrading.saturation.value = -PlayerConstant.fearGauge;

        depthOfField.focusDistance.overrideState = BlinkEffect.Blink > 0.3f;
        if      (BlinkEffect.Blink > 0.8f) depthOfField.kernelSize.value = KernelSize.VeryLarge;
        else if (BlinkEffect.Blink > 0.7f) depthOfField.kernelSize.value = KernelSize.Large;
        else if (BlinkEffect.Blink > 0.57f) depthOfField.kernelSize.value = KernelSize.Medium;
        else if (BlinkEffect.Blink > 0.45f) depthOfField.kernelSize.value = KernelSize.Small;
    }


    private void UpdateSFX()
    {
        // 위치 조정
        //AudioManager.Instance.SetPosition(AudioManager.Instance.fearHal, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.stressHal, transform.position);
        // 위치 조정
        
        // -------------------------------------머리 움직임 효과음
        pillowSound.PlaySound();
        
        // -------------------------------------게이지 효과음
        float targetFearSFXVolume, targetStressSFXVolume;

        if (PlayerConstant.fearGauge < 40) targetFearSFXVolume = 0.0f;
        else targetFearSFXVolume = Mathf.Clamp((PlayerConstant.fearGauge - 39) / 60f, 0f, 1f);

        if (PlayerConstant.stressGauge < 25) targetStressSFXVolume = 0.0f;
        else targetStressSFXVolume = Mathf.Clamp((PlayerConstant.stressGauge - 25) / 75f, 0f, 1f);

        if (currentFearSFXVolume > targetFearSFXVolume)
        {
            currentFearSFXVolume -= 0.1f * Time.deltaTime;
            currentFearSFXVolume = Mathf.Max(currentFearSFXVolume, targetFearSFXVolume);
        }

        if (currentFearSFXVolume < targetFearSFXVolume)
        {
            currentFearSFXVolume += 0.1f * Time.deltaTime;
            currentFearSFXVolume = Mathf.Min(currentFearSFXVolume, targetFearSFXVolume);
        }

        if (currentStressSFXVolume > targetStressSFXVolume)
        {
            currentStressSFXVolume -= 0.1f * Time.deltaTime;
            currentStressSFXVolume = Mathf.Max(currentStressSFXVolume, targetStressSFXVolume);
        }

        if (currentStressSFXVolume < targetStressSFXVolume)
        {
            currentStressSFXVolume += 0.1f * Time.deltaTime;
            currentStressSFXVolume = Mathf.Min(currentStressSFXVolume, targetStressSFXVolume);
        }

        //AudioManager.Instance.VolumeControl(AudioManager.Instance.fearHal, currentFearSFXVolume);
        AudioManager.Instance.VolumeControl(AudioManager.Instance.stressHal, currentStressSFXVolume);
        // -------------------------------------게이지 효과음
    }
    
    private void StopPlayer()
    {
        base.StopPlayer(PlayerConstant.isPlayerStop);
        if (PlayerConstant.isPlayerStop && PlayerConstant.isEyeOpen)
        {
            playerEyeControl.ChangeEyeState(PlayerEyeStateTypes.Close);
        }
    }
    
    private void SetPlayerState()
    {   
        if (PlayerConstant.isParalysis)
        {
            playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 5f;
            playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 5f;
        }
        else
        {
            playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = MouseSettings.Instance.MouseMaxSpeed;
            playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = MouseSettings.Instance.MouseMaxSpeed;
        }
    }

    public void EnablePlayerObject(bool isActivate)
    {
        gameObject?.SetActive(isActivate);
    }

    public void EyeControl(PlayerEyeStateTypes types)
    {
        playerEyeControl.ChangeEyeState(types);
    }

    public void DirectionControl(PlayerDirectionStateTypes types)
    {
        playerDirectionControl.ChangeDirectionState(types);
    }

    public void DirectionControlNoSound(PlayerDirectionStateTypes types)
    {
        playerDirectionControl.ChangeDirectionStateNoSound(types);
    }
}

