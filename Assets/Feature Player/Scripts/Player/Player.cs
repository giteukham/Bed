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
    [Header("State Machine")]
    [SerializeField] private StateMachine playerDirectionStateMachine;
    [SerializeField] private StateMachine playerEyeStateMachine;
    
    #region Player Components
    [Header("Player Camera")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    
    private PlayerAnimation playerAnimation;
    
    [Header("Input System")]
    [SerializeField] private InputSystem inputSystem;
    
    [Header("Cone Colider")]
    [SerializeField] private ConeCollider coneCollider;
    #endregion

    #region Post Processing
    private CinemachinePostProcessing postProcessing;
    private ColorGrading colorGrading;
    private Grain grain;
    private ChromaticAberration chromaticAberration;
    private DepthOfField depthOfField;
    private PSXPostProcessEffect psxPostProcessEffect;
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
    
    private CinemachineBasicMultiChannelPerlin cameraNoise;
    private float currentVolume;
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
        AudioManager.instance.PlaySound(AudioManager.instance.fearWhisper, transform.position);
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
        // ----------------- Look Value -----------------
    }

    private void UpdateCamera()
    {
        if (PlayerConstant.fearGauge < 40) cameraNoise.m_FrequencyGain = 0.5f;
        else if (PlayerConstant.fearGauge >= 40 && PlayerConstant.fearGauge < 70) cameraNoise.m_FrequencyGain = 1f;
        else if(PlayerConstant.fearGauge >= 70) cameraNoise.m_FrequencyGain = 1.5f;
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
        depthOfField.focusDistance.value = 2.4f - (BlinkEffect.Blink * 2.4f);
    }

    IEnumerator ChromaticAberrationEffect()
    {
        while (true)
        {
            chromaticAberration.intensity.value = PlayerConstant.stressGauge * 0.01f;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
            chromaticAberration.intensity.value = PlayerConstant.stressGauge * 0.01f / UnityEngine.Random.Range(2f, 4f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
        }
    }

    private void UpdateSFX()
    {
        float targetVolume;

        if (PlayerConstant.fearGauge < 40) targetVolume = 0.0f;
        else targetVolume = Mathf.Clamp((PlayerConstant.fearGauge - 40) / 60f, 0f, 1f);

        if (currentVolume > targetVolume)
        {
            currentVolume -= 0.1f * Time.deltaTime;
            currentVolume = Mathf.Max(currentVolume, targetVolume);
        }

        if (currentVolume < targetVolume)
        {
            currentVolume += 0.1f * Time.deltaTime;
            currentVolume = Mathf.Min(currentVolume, targetVolume);
        }

        AudioManager.instance.SetPosition(AudioManager.instance.fearWhisper, transform.position);

        AudioManager.instance.VolumeControl(AudioManager.instance.fearWhisper, currentVolume);
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

    private void OnApplicationQuit()
    {
        BlinkEffect.Blink = 0.001f;
    }

    private static Coroutine fearGaugeCoroutine;

    // public static void SetFearGuage(int value)
    // {
    //     float targetValue = PlayerConstant.fearGauge + value;

    //     if (fearGaugeCoroutine != null)
    //     {
    //         StopCoroutine(fearGaugeCoroutine);
    //     }

    //     fearGaugeCoroutine = StartCoroutine(ChangeFearGauge(targetValue));
    // }

    public static IEnumerator ChangeFearGauge(int targetValue)
    {
        float duration = 2f;
        float elapsed = 0f;
        float startValue = PlayerConstant.fearGauge;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            PlayerConstant.fearGauge = (int)Mathf.Lerp(startValue, targetValue, elapsed / duration);
            yield return null; 
        }

        PlayerConstant.fearGauge = targetValue;
        yield return null;
    }
}

