using System;
using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Bed.Collider;
using PSXShaderKit;
using System.Collections;
using DG.Tweening;

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
    private float currentStressSFXVolume, currentHeadMoveSFXVolume;
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

        // Camera Noise
        //cameraNoise = playerCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        // Sound Play
        AudioManager.Instance.PlaySound(AudioManager.Instance.stressHal, transform.position);

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
        while (true)
        {
            float shakeIntensity = 0f;

            if      (PlayerConstant.stressLevel >= 80) shakeIntensity = 1.3f;
            else if (PlayerConstant.stressLevel >= 65) shakeIntensity = 0.75f;
            else if (PlayerConstant.stressLevel >= 50) shakeIntensity = 0.35f;
            else if (PlayerConstant.stressLevel >= 25) shakeIntensity = 0.1f;
            else if (PlayerConstant.stressLevel < 25)  shakeIntensity = 0f;

            playerVirtualCamera.m_Lens.Dutch = UnityEngine.Random.Range(-shakeIntensity, shakeIntensity);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ChromaticAberrationEffect()
    {
        while (true)
        {
            chromaticAberration.intensity.value = PlayerConstant.stressLevel * 0.01f;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.12f));
            chromaticAberration.intensity.value = PlayerConstant.stressLevel * 0.01f / UnityEngine.Random.Range(2f, 4f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.03f, 0.1f));
        }
    }

    private void UpdatePostProcessing()
    {
        StartCoroutine(ChromaticAberrationEffect());
        grain.intensity.value = PlayerConstant.stressLevel * 0.01f;

        psxPostProcessEffect._PixelationFactor = Mathf.Lerp(pixelationFactor, pixelationFactor * 0.4f, PlayerConstant.stressLevel / 100f);
        colorGrading.saturation.value = -PlayerConstant.stressLevel;

        depthOfField.focusDistance.overrideState = BlinkEffect.Blink > 0.3f;
        if      (BlinkEffect.Blink > 0.8f) depthOfField.kernelSize.value = KernelSize.VeryLarge;
        else if (BlinkEffect.Blink > 0.7f) depthOfField.kernelSize.value = KernelSize.Large;
        else if (BlinkEffect.Blink > 0.57f) depthOfField.kernelSize.value = KernelSize.Medium;
        else if (BlinkEffect.Blink > 0.45f) depthOfField.kernelSize.value = KernelSize.Small;
    }


    private void UpdateSFX()
    {
        // 위치 조정
        AudioManager.Instance.SetPosition(AudioManager.Instance.stressHal, transform.position);
        
        // -------------------------------------머리 움직임 효과음
        pillowSound.PlaySound();
        
        // -------------------------------------게이지 효과음
        float targetStressSFXVolume;

        if (PlayerConstant.stressLevel < 25) targetStressSFXVolume = 0.0f;
        else targetStressSFXVolume = Mathf.Clamp((PlayerConstant.stressLevel - 25) / 75f, 0f, 1f);

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
    
    public IEnumerator LookAt(Vector3 targetPos)
    {
        var verticalAngle = Mathf.Atan2(targetPos.y - transform.localPosition.y, targetPos.x - transform.localPosition.x) * Mathf.Rad2Deg - 180;
        var horizontalAngle = Mathf.Atan2(targetPos.x - transform.localPosition.x, targetPos.z - transform.localPosition.z) * Mathf.Rad2Deg + 180;
        verticalAngle = Mathf.Clamp(verticalAngle, povCamera.m_VerticalAxis.m_MinValue, povCamera.m_VerticalAxis.m_MaxValue);
        horizontalAngle = Mathf.Clamp(horizontalAngle, povCamera.m_HorizontalAxis.m_MinValue, povCamera.m_HorizontalAxis.m_MaxValue);
        
        var baseSpeed = 10f;
        var horizontalDistance = Mathf.Abs(horizontalAngle - povCamera.m_HorizontalAxis.Value);
        var verticalDistance = Mathf.Abs(verticalAngle - povCamera.m_VerticalAxis.Value);
        var maxDistance = Mathf.Max(horizontalDistance, verticalDistance);
        
        // 동일한 시간에 도달하도록 속도 계산
        var horizontalSpeed = baseSpeed * (horizontalDistance / maxDistance) * Mathf.Sign(horizontalAngle - povCamera.m_HorizontalAxis.Value);
        var verticalSpeed = baseSpeed * (verticalDistance / maxDistance) * Mathf.Sign(verticalAngle - povCamera.m_VerticalAxis.Value);
        
        var isComplete = new[] { false, false };
        
        while (!(isComplete[0] && isComplete[1]))
        {
            ProcessAxis(0, ref povCamera.m_HorizontalAxis, horizontalAngle, horizontalSpeed);
            ProcessAxis(1, ref povCamera.m_VerticalAxis, verticalAngle, verticalSpeed);
            
            yield return null;
        }

        void ProcessAxis(int index, ref AxisState axis, float targetValue, float speed)
        {
            if (isComplete[index]) return;
            
            var currentValue = axis.Value;
            var nextValue = currentValue + Time.deltaTime * speed;
            
            // 목표값 초과 여부 확인
            var overshot = (speed > 0 && nextValue > targetValue) || (speed < 0 && nextValue < targetValue);
            
            if (overshot)
            {
                axis.Value = targetValue;
                isComplete[index] = true;
            }
            else
            {
                axis.Value = nextValue;
            }
        }
    }
}

