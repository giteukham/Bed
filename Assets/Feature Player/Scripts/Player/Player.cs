using Bed.PostProcessing;
using Cinemachine;
using Cinemachine.PostFX;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Rendering.PostProcessing;
using Bed.Collider;
using PSXShaderKit;
using System.Collections;
using Unity.VisualScripting;

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
public class Player : MonoBehaviour
{
    #region Player Components
    [Header("Player Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    
    [Header("State Machine")]
    [SerializeField] private StateMachine playerDirectionStateMachine;
    [SerializeField] private StateMachine playerEyeStateMachine;
    
    private PlayerAnimation playerAnimation;
    
    [Header("Input System")]
    [SerializeField] private InputSystem inputSystem;
    
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
    #endregion
    
    #region Player Control Classes
    private PlayerDirectionControl playerDirectionControl;
    private PlayerEyeControl playerEyeControl;
    #endregion

    #region Main Camera
    [Header("Main Camera")]
    [SerializeField] private Camera mainCamera;
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
    
    #region Sound Effect Variables
    [SerializeField]private Transform playerPillowSoundPosition;
    private Vector3 playerPillowSoundInitPosition;
    private Vector3 playerPillowSoundInitRotation;
    private float breathTime;
    private float currentFearSFXVolume, currentStressSFXVolume, currentHeadMoveSFXVolume;
    private bool exhaleCheck = false; // false면 들숨, true면 날숨
    private Coroutine headMoveSFXCoroutine;
    #endregion

    // UI manager로 옮기기
    [SerializeField] GameObject uiCanvas;

    private void Start()
    {
        TryGetComponent(out playerAnimation);
        
        playerDirectionControl = new PlayerDirectionControl(playerDirectionStateMachine);
        playerEyeControl = new PlayerEyeControl(playerEyeStateMachine);
        playerEyeControl.SubscribeToEvents();

        // Post Processing
        postProcessing = playerCamera.GetComponent<CinemachinePostProcessing>();
        postProcessing.m_Profile.TryGetSettings(out colorGrading);
        postProcessing.m_Profile.TryGetSettings(out grain);
        postProcessing.m_Profile.TryGetSettings(out chromaticAberration);
        postProcessing.m_Profile.TryGetSettings(out depthOfField);
        psxPostProcessEffect = mainCamera.GetComponent<PSXPostProcessEffect>();

        // Camera Noise
        cameraNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        // Sound Play
        AudioManager.instance.PlaySound(AudioManager.instance.fearHal, transform.position);
        AudioManager.instance.PlaySound(AudioManager.instance.stressHal, transform.position);
        AudioManager.instance.PlaySound(AudioManager.instance.headMove, transform.position);

        playerPillowSoundInitPosition = playerPillowSoundPosition.transform.position;
        playerPillowSoundInitRotation = playerPillowSoundPosition.transform.eulerAngles;
    }

    public void AnimationEvent_ChangeDirectionState(string toState)
    {
        switch (toState)
        {   
            case "Left":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Left]);
                break;
            case "Middle":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Middle]);
                break;
            case "Right":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Right]);
                break;
            case "Switching":
                playerDirectionStateMachine.ChangeState(PlayerDirectionControl.DirectionStates[PlayerDirectionStateTypes.Switching]);
                break;
        }
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
        coneCollider.SetColider(BlinkEffect.Blink);
    }

    private void UpdateStats()
    {
        // ----------------- Head Movement -----------------
        float cameraDeltaX = playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value;
        float cameraDeltaY = playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value;
        recentHorizontalCameraMovement = currentHorizontalCameraMovement;
        recentVerticalCameraMovement = currentVerticalCameraMovement;
        currentHorizontalCameraMovement = cameraDeltaX;
        currentVerticalCameraMovement = cameraDeltaY;
        if(currentVerticalCameraMovement == recentVerticalCameraMovement && currentHorizontalCameraMovement == recentHorizontalCameraMovement) isCameraMovement = 0;
        else isCameraMovement = 1;
        
        float mouseDeltaX = Mathf.Abs(InputSystem.MouseDeltaX);
        float mouseDeltaY = Mathf.Abs(InputSystem.MouseDeltaY);
        recentHorizontalMouseMovement = currentHorizontalMouseMovement;
        recentVerticalMouseMovement = currentVerticalMouseMovement;
        currentHorizontalMouseMovement = mouseDeltaX;
        currentVerticalMouseMovement = mouseDeltaY;
        if(currentVerticalMouseMovement == recentVerticalMouseMovement) deltaVerticalMouseMovement = 0;
        else deltaVerticalMouseMovement = Mathf.Abs(currentVerticalMouseMovement - recentVerticalMouseMovement);
        if(currentHorizontalMouseMovement == recentHorizontalMouseMovement) deltaHorizontalMouseMovement = 0;
        else deltaHorizontalMouseMovement = Mathf.Abs(currentHorizontalMouseMovement - recentHorizontalMouseMovement);
        
        PlayerConstant.HeadMovementCAT += (deltaHorizontalMouseMovement + deltaVerticalMouseMovement) * isCameraMovement;
        PlayerConstant.HeadMovementLAT += (deltaHorizontalMouseMovement + deltaVerticalMouseMovement) * isCameraMovement;
        // ----------------- Head Movement -----------------

        // ----------------- Look Value -----------------
        if (PlayerConstant.isEyeOpen == false) return;
        else
        {
            float eulerY = mainCamera.transform.eulerAngles.y;
            float eulerX = mainCamera.transform.eulerAngles.x;

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
        if      (PlayerConstant.fearGauge >= 80) cameraNoise.m_FrequencyGain = 4f;
        else if (PlayerConstant.fearGauge >= 60) cameraNoise.m_FrequencyGain = 3f;
        else if (PlayerConstant.fearGauge >= 40) cameraNoise.m_FrequencyGain = 2f;
        else if (PlayerConstant.fearGauge < 40)  cameraNoise.m_FrequencyGain = 1f;

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

            playerCamera.m_Lens.Dutch = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
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
        psxPostProcessEffect._PixelationFactor = 0.25f + (-PlayerConstant.fearGauge * 0.0015f);
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
        AudioManager.instance.SetPosition(AudioManager.instance.fearHal, transform.position);
        AudioManager.instance.SetPosition(AudioManager.instance.stressHal, transform.position);
        AudioManager.instance.SetPosition(AudioManager.instance.headMove, playerPillowSoundPosition.position);
        // 위치 조정

        // --------머리 움직임 소리
        if (deltaHorizontalMouseMovement > 0f || deltaVerticalMouseMovement > 0f || PlayerConstant.isMovingState)  
        {
            if (PlayerConstant.isRightState || PlayerConstant.isLeftState) AudioManager.instance.SetParameter(AudioManager.instance.headMove, "Lowpass", 1.6f);
            else AudioManager.instance.SetParameter(AudioManager.instance.headMove, "Lowpass", 4.5f);

            if(AudioManager.instance.GetVolume(AudioManager.instance.headMove) < 1.0f) 
            {
                if (headMoveSFXCoroutine != null) StopCoroutine(headMoveSFXCoroutine);
                headMoveSFXCoroutine = StartCoroutine(headMoveSFXSet(true));
            }
        }
        else 
        {
            if(AudioManager.instance.GetVolume(AudioManager.instance.headMove) > 0.0f) 
            {
                if (headMoveSFXCoroutine != null) StopCoroutine(headMoveSFXCoroutine);
                headMoveSFXCoroutine = StartCoroutine(headMoveSFXSet(false));
            }
        }

        IEnumerator headMoveSFXSet(bool _Up)
        {
            float volume = AudioManager.instance.GetVolume(AudioManager.instance.headMove);

            if(_Up)
            {
                AudioManager.instance.ResumeSound(AudioManager.instance.headMove);
                while(volume < 1.0f)
                {
                    volume += 0.1f;
                    volume = Mathf.Clamp(volume, 0.0f, 1.0f);
                    AudioManager.instance.VolumeControl(AudioManager.instance.headMove, volume);
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
                    AudioManager.instance.VolumeControl(AudioManager.instance.headMove, volume);
                    yield return new WaitForSeconds(0.1f);
                }
                AudioManager.instance.PauseSound(AudioManager.instance.headMove);
                headMoveSFXCoroutine = null;
            }
        }   
        // --------머리 움직임 소리

        // --------------------숨소리
        breathTime += Time.deltaTime * (cameraNoise.m_FrequencyGain * 0.25f);
        if (breathTime >= 0.5f && exhaleCheck == false) 
        { 
            breathTime = 0.0f; 
            AudioManager.instance.PlayOneShot(AudioManager.instance.inhale, transform.position);
            exhaleCheck = true;
        }
        if (breathTime >= 0.5f && exhaleCheck == true)
        {
            breathTime = 0.0f;
            AudioManager.instance.PlayOneShot(AudioManager.instance.exhale, transform.position);
            exhaleCheck = false;
        }
        // --------------------숨소리

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

        AudioManager.instance.VolumeControl(AudioManager.instance.fearHal, currentFearSFXVolume);
        AudioManager.instance.VolumeControl(AudioManager.instance.stressHal, currentStressSFXVolume);
        // -------------------------------------게이지 효과음
    }

    private void SetPlayerState()
    {   
        if (PlayerConstant.isParalysis)
        {
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 5f;
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 5f;
        }
        else
        {
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_MaxSpeed = 500f;
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed = 500f;
        }
    }

    private void StopPlayer()
    {
        if (PlayerConstant.isPlayerStop == true)
        {
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "";
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "";
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisValue = 0;
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisValue = 0;
            if (PlayerConstant.isEyeOpen) playerEyeControl.ChangeEyeState(PlayerEyeStateTypes.Close);

            // UI manager로 옮기기
            if (PlayerConstant.isEyeOpen == false) 
            {
                uiCanvas.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            
        }
        if (PlayerConstant.isPlayerStop == false)
        {
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_InputAxisName = "Mouse Y";
            playerCamera.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.m_InputAxisName = "Mouse X";

            // UI manager로 옮기기
            uiCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnApplicationQuit()
    {
        BlinkEffect.Blink = 0.001f;
    }
}

