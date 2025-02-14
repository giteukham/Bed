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
    [SerializeField] private CinemachineVirtualCamera playerVirtualCamera;
    
    [Header("Cone Colider")]
    [SerializeField] private ConeCollider coneCollider;
    #endregion

    #region Camera Effect Variables
    private CinemachinePostProcessing postProcessing;
    private ColorGrading colorGrading;
    private Grain grain;
    private ChromaticAberration chromaticAberration;
    private DepthOfField depthOfField;
    private PSXPostProcessEffect psxPostProcessEffect;
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    public float pixelationFactor = 0.25f;
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
    [SerializeField]private Transform playerPillowSoundPosition;
    private Vector3 playerPillowSoundInitPosition;
    private Vector3 playerPillowSoundInitRotation;
    private float currentFearSFXVolume, currentStressSFXVolume, currentHeadMoveSFXVolume;
    private Coroutine headMoveSFXCoroutine;
    #endregion

    private void Start()
    {
        povCamera = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
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

        // Camera Noise
        //cameraNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        // Sound Play
        // fearHal 임시로 뺌뺌
        //AudioManager.Instance.PlaySound(AudioManager.Instance.fearHal, transform.position);
        AudioManager.Instance.PlaySound(AudioManager.Instance.stressHal, transform.position);
        AudioManager.Instance.PlaySound(AudioManager.Instance.headMove, transform.position);

        playerPillowSoundInitPosition = playerPillowSoundPosition.transform.position;
        playerPillowSoundInitRotation = playerPillowSoundPosition.transform.eulerAngles;

        pixelationFactor = SaveManager.Instance.LoadPixelationFactor();
    }

    void Update() 
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateStats();
            timeSinceLastUpdate = 0f;
        }
        UpdateCamera();
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

    private void UpdateCamera()
    {
        // if      (PlayerConstant.fearGauge >= 80) cameraNoise.m_FrequencyGain = 4f;
        // else if (PlayerConstant.fearGauge >= 60) cameraNoise.m_FrequencyGain = 3f;
        // else if (PlayerConstant.fearGauge >= 40) cameraNoise.m_FrequencyGain = 2f;
        // else if (PlayerConstant.fearGauge < 40)  cameraNoise.m_FrequencyGain = 1f;

        StartCoroutine(StressShake());
    }

    IEnumerator StressShake()
    {
        while (true)
        {
            float shakeIntensity = 0f;

            if      (PlayerConstant.stressGauge >= 80) shakeIntensity = 1.3f;
            else if (PlayerConstant.stressGauge >= 65) shakeIntensity = 0.75f;
            else if (PlayerConstant.stressGauge >= 50) shakeIntensity = 0.35f;
            else if (PlayerConstant.stressGauge >= 25) shakeIntensity = 0.1f;
            else if (PlayerConstant.stressGauge < 25)  shakeIntensity = 0f;

            playerVirtualCamera.m_Lens.Dutch = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            yield return new WaitForSeconds(0.1f);
        }
    }


    private void UpdateGauge() 
    {
        if (PlayerConstant.stressGauge >= PlayerConstant.stressGaugeMax) PlayerConstant.isFainting = true;
        else PlayerConstant.isFainting = false;
        
        if (PlayerConstant.fearGauge >= PlayerConstant.fearGaugeMax) PlayerConstant.isParalysis = true;
        else PlayerConstant.isParalysis = false;

        PlayerConstant.stressGauge = Mathf.Clamp(PlayerConstant.stressGauge, PlayerConstant.stressGaugeMin, PlayerConstant.stressGaugeMax);
        PlayerConstant.fearGauge = Mathf.Clamp(PlayerConstant.fearGauge, PlayerConstant.fearGaugeMin, PlayerConstant.fearGaugeMax);
    }

    private void UpdatePostProcessing()
    {
        StartCoroutine(ChromaticAberrationEffect());
        grain.intensity.value = PlayerConstant.fearGauge * 0.01f;

        psxPostProcessEffect._PixelationFactor = Mathf.Lerp(pixelationFactor, pixelationFactor * 0.4f, PlayerConstant.fearGauge / 100f);
        colorGrading.saturation.value = -PlayerConstant.fearGauge;

        depthOfField.focusDistance.overrideState = BlinkEffect.Blink > 0.3f;
        if      (BlinkEffect.Blink > 0.8f) depthOfField.kernelSize.value = KernelSize.VeryLarge;
        else if (BlinkEffect.Blink > 0.7f) depthOfField.kernelSize.value = KernelSize.Large;
        else if (BlinkEffect.Blink > 0.57f) depthOfField.kernelSize.value = KernelSize.Medium;
        else if (BlinkEffect.Blink > 0.45f) depthOfField.kernelSize.value = KernelSize.Small;
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

    private void UpdateSFX()
    {
        playerPillowSoundPosition.position = new Vector3(this.transform.position.x, playerPillowSoundInitPosition.y, playerPillowSoundInitPosition.z);
        playerPillowSoundPosition.eulerAngles = new Vector3(playerPillowSoundInitRotation.x, playerPillowSoundInitRotation.y, playerPillowSoundInitRotation.z);
        
        // 위치 조정
        //AudioManager.Instance.SetPosition(AudioManager.Instance.fearHal, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.stressHal, transform.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.headMove, playerPillowSoundPosition.position);
        // 위치 조정

        // --------머리 움직임 소리
        if (PlayerConstant.isShock)
        {
            if (headMoveSFXCoroutine != null) StopCoroutine(headMoveSFXCoroutine);
            headMoveSFXCoroutine = StartCoroutine(headMoveSFXSet(false));
        }
        else if ((deltaHorizontalMouseMovement > 0f && PlayerConstant.isPlayerStop == false) 
            || (deltaVerticalMouseMovement > 0f && PlayerConstant.isPlayerStop == false) 
            || PlayerConstant.isMovingState)  
        {
            if (PlayerConstant.isRightState || PlayerConstant.isLeftState) AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", 1.6f);
            else AudioManager.Instance.SetParameter(AudioManager.Instance.headMove, "Lowpass", 4.5f);
        
            if(AudioManager.Instance.GetVolume(AudioManager.Instance.headMove) < 1.0f) 
            {
            if (headMoveSFXCoroutine != null) StopCoroutine(headMoveSFXCoroutine);
            headMoveSFXCoroutine = StartCoroutine(headMoveSFXSet(true));
            }
        }
        else 
        {
            if(AudioManager.Instance.GetVolume(AudioManager.Instance.headMove) > 0.0f) 
            {
            if (headMoveSFXCoroutine != null) StopCoroutine(headMoveSFXCoroutine);
            headMoveSFXCoroutine = StartCoroutine(headMoveSFXSet(false));
            }
        }
        
        IEnumerator headMoveSFXSet(bool _Up)
        {
            float volume = AudioManager.Instance.GetVolume(AudioManager.Instance.headMove);
        
            if(_Up)
            {
                AudioManager.Instance.ResumeSound(AudioManager.Instance.headMove);
                while(volume < 1.0f)
                {
                    volume += 0.1f;
                    volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                    AudioManager.Instance.VolumeControl(AudioManager.Instance.headMove, volume);
                    yield return new WaitForSeconds(0.1f);
                }
                headMoveSFXCoroutine = null;
            }
            else
            {
                while(volume > 0.0f)
                {
                    volume -= 0.1f;
                    volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                    AudioManager.Instance.VolumeControl(AudioManager.Instance.headMove, volume);
                    yield return new WaitForSeconds(0.1f);
                }
                AudioManager.Instance.PauseSound(AudioManager.Instance.headMove);
                headMoveSFXCoroutine = null;
            }
        }   
        
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
}

