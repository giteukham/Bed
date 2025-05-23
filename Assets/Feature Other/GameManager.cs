using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using AbstractGimmick;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum GameState
{
    Preparation,
    GamePlay,
    GameOver
}

public class GameManager : MonoSingleton<GameManager>
{
    #region Debug Variables
    [Header("Debug Variables")]
    [SerializeField] private GameObject debugStatsText;
    [SerializeField] private GameObject debugTimeText;
    [SerializeField] private GameObject debugColiderImage;
    [SerializeField] private GameObject debugGimmickText;
    #endregion

    #region Main Camera
    [Header("Main Camera")]
    [SerializeField] private Camera mainCamera;
    #endregion

    #region Reference Components
    [Header("Reference Components")]
    public Player player;
    #endregion

    #region Reference Components for Debug
    [Header("Reference Components for Debug")]
    public GameObject parentsGimmick;
    public GameObject neighborGimmick;
    private MarkovGimmick parentsGimmickScript;
    private MarkovGimmick neighborGimmickScript;
    #endregion
    
    #region Objects related Components
    [Header("Objects related Components")]
    [SerializeField] private GameObject dad;
    private Animator motherAnimator;
    #endregion

    #region Quit Settings
    [Header("Quit Settings")]
    public bool isBlinkInit;
    #endregion
    
    #region Managers
    [Header("Managers")]
    [SerializeField] private TimeManager timeManager;
    #endregion

    #region Intro Scene Related Values
    [Header("Intro Scene Related Values")]
    [SerializeField, Tooltip("준비 확인 시간")]
    private float ready_CheckTime;

    [SerializeField, Tooltip("문 열리기 전 지연 시간")]
    private float door_Open_BeforeDelayTime;

    [SerializeField, Tooltip("엄마 등장 전 지연 시간")]
    private float mother_Appear_BeforeDelayTime;

    [SerializeField, Tooltip("엄마 퇴장 전 지연 시간")]
    private float mother_Leave_BeforeDelayTime;

    [SerializeField, Tooltip("문 닫히기 전 지연 시간")]
    private float door_Close_BeforeDelayTime;

    [SerializeField, Tooltip("엄마 비활성화 전 지연 시간")]
    private float mother_Deactivate_BeforeDelayTime;

    [SerializeField, Tooltip("게임 시작 전 지연 시간")]
    private float game_Start_BeforeDelayTime;
    #endregion

    [Space(10f)]
    [SerializeField] private Door door;
    private GameState currentState;
    public bool isTutorialEnable;
    
    private float lastLeftClickTime = -1f;
    private float lastRightClickTime = -1f;

    [Tooltip("동시 클릭 허용 시간")]
    public float bothClickToleranceTime;
    
    [Header("데모")]
    // 데모 씬 전용. 데모 씬은 반드시 이름이 Demo여야 함.

    public Gimmick testGimmick;
    
    private void OnValidate()
    // 값이 0이면 기본 값으로 설정
    {
        if (ready_CheckTime <= 0) ready_CheckTime = 2.5f;                             
        if (door_Open_BeforeDelayTime <= 0) door_Open_BeforeDelayTime = 2f;             
        if (mother_Appear_BeforeDelayTime <= 0) mother_Appear_BeforeDelayTime = 0.4f; 
        if (mother_Leave_BeforeDelayTime <= 0) mother_Leave_BeforeDelayTime = 0.45f;
        if (door_Close_BeforeDelayTime <= 0) door_Close_BeforeDelayTime = 0.2f;
        if (mother_Deactivate_BeforeDelayTime <= 0) mother_Deactivate_BeforeDelayTime = 0.1f;
        if (game_Start_BeforeDelayTime <= 0) game_Start_BeforeDelayTime = 1f;
        if (bothClickToleranceTime <= 0) bothClickToleranceTime = 0.1f;
    }

    void Awake()
    {
        motherAnimator = dad.GetComponent<Animator>();
        //InputSystem.Instance.OnMouseClickEvent += () => PlayerConstant.isPlayerStop = false;

        // if (SceneManager.GetActiveScene().name.Equals("Demo")) isDemo = true;

        //예전 마지막 플레이 시간 가져옴
        DateTime ago = DateTime.ParseExact(SaveManager.Instance.LoadLastPlayedTime(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        //현재 플레이 시간 가져옴
        DateTime now = DateTime.Now;

        //시간 비교
        TimeSpan timeDifference = now - ago;
        
        // isTutorialEnable = timeDifference.TotalMinute >= 1f;
        isTutorialEnable = timeDifference.TotalHours >= 48.0d;
    }

    void Start()
    {
        SetState(GameState.Preparation);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        #if UNITY_EDITOR
            if (debugStatsText.activeSelf) 
            {
                debugStatsText.SetActive(false);
                debugGimmickText.SetActive(true);
            }
            if (debugTimeText.activeSelf) debugTimeText.SetActive(false);
            if (debugColiderImage.activeSelf) debugColiderImage.SetActive(false);
            parentsGimmickScript = parentsGimmick.GetComponent<MarkovGimmick>();
            neighborGimmickScript = neighborGimmick.GetComponent<MarkovGimmick>();
        #endif
    }

    private void Update()
    {
        if (UIManager.Instance.isRightClikHeld == false && IsBothMouseClicked() ||  Input.GetKeyDown(KeyCode.Escape))
        {
            if (PlayerConstant.isPlayerStop == true)
            {
                PlayerConstant.isPlayerStop = false;
                //UIManager.Instance.ActivateUICanvas(false);
            }
            else if (PlayerConstant.isPlayerStop == false)
            {
                PlayerConstant.isPlayerStop = true;
                //UIManager.Instance.ActivateUICanvas(true);
            }
        }
        #if UNITY_EDITOR
            DebugFunctions();

            if (Input.GetKeyDown(KeyCode.Q) && !Input.GetKey(KeyCode.LeftShift))
            {
                testGimmick.gameObject.SetActive(true);
                testGimmick.Activate();
            }
            if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftShift))
            {
                testGimmick.Deactivate();
            }
        #endif
    }

    public void SetState(GameState state)
    {
        currentState = state;
        UpdateState();
    }
    
    private void UpdateState()
    {
        switch (currentState)
        {
            case GameState.Preparation:
                Prepartion();
                break;
            case GameState.GamePlay:
                GamePlay();
                break;
            case GameState.GameOver:
                GameOver();
                break;
        }
    }
    
    private void Prepartion()
    {
        BedRoomLightSwitch.SwitchActionNoSound(true);
        LivingRoomLightSwitch.SwitchAction(true);
        player.EyeControl(PlayerEyeStateTypes.Close);
        timeManager.gameObject.SetActive(false);
        GimmickManager.Instance.InitGimmicks();
        PlayerLevelController.Instance.Initialize();
        Door.SetNoSound(0, 0);
        PlayerConstant.isShock = false;
        TutorialManager.Instance.isEyeOpenTutorialActivate = false;
        TutorialManager.Instance.isBlinkTutorialActivate = false;

        UIManager.Instance.DeutActivate(true);

        if (isTutorialEnable) TutorialManager.Instance.EyeOpenTutorialStart();
        StartCoroutine(ReadyCheckCoroutine());
    }

    private IEnumerator ReadyCheckCoroutine()
    {
        if (!isBlinkInit) yield return new WaitForSeconds(0.25f);
        float time = 0f;

        while (true)
        {
            if (BlinkEffect.Blink <= 0.93f)
            {
                UIManager.Instance.DeutActivate(false);
                if (TutorialManager.Instance.CheckCockroachActive()) door.StartDoorKnock();
            } 

            if (PlayerConstant.isLeftState && PlayerConstant.isEyeOpen && !isTutorialEnable)
            {
                time += Time.deltaTime;
                if (time >= ready_CheckTime)
                {
                    SetState(GameState.GamePlay);
                    yield break;
                }
            }
            else time = 0f;

            yield return null;
        }
    }
    
    private void GamePlay()
    {
        Debug.Log("GamePlay !!");
        StartCoroutine(GamePlayCoroutine());
    }
    
    IEnumerator GamePlayCoroutine()
    {
        door.StopDoorKnock();
        yield return new WaitForSeconds(door_Open_BeforeDelayTime);
        Door.Set(30, 0.35f);
        yield return new WaitForSeconds(mother_Appear_BeforeDelayTime);
        dad.SetActive(true);

        float randomNum = UnityEngine.Random.Range(5f, 8f);
        yield return new WaitForSeconds(randomNum);

        BedRoomLightSwitch.SwitchAction(false);
        yield return new WaitForSeconds(mother_Leave_BeforeDelayTime);
        motherAnimator.SetTrigger("Leave");
        yield return new WaitForSeconds(door_Close_BeforeDelayTime);
        Door.Set(0, 0.4f);
        yield return new WaitForSeconds(mother_Deactivate_BeforeDelayTime);
        dad.SetActive(false);
        //LivingRoomLightSwitch.SwitchAction(false);
        yield return new WaitForSeconds(game_Start_BeforeDelayTime);
        timeManager.gameObject.SetActive(true);
        PlayerLevelController.Instance.OnGameStart();
        PlayerConstant.ResetAllStats();
    }
    
    private void GameOver()
    {
        Debug.Log("GameOver !!");
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        player.EyeControl(PlayerEyeStateTypes.Close);
        PlayerConstant.isShock = true;
        Invoke(nameof(DelayTurnToMiddle), 0.1f);
        yield return new WaitForSeconds(1f);
        SetState(GameState.Preparation);
    }

    public bool IsBothMouseClicked()
    {
        bool isLeftClick = Input.GetMouseButtonDown(0);
        bool isRightClick = Input.GetMouseButtonDown(1);

        if (isLeftClick)
        {
            if (Time.time - lastRightClickTime <= bothClickToleranceTime)
            {
                return true;
            }
            lastLeftClickTime = Time.time;
        }

        if (isRightClick)
        {
            if (Time.time - lastLeftClickTime <= bothClickToleranceTime)
            {
                return true;
            }
            lastRightClickTime = Time.time;
        }
        return false;
    }

    private void DelayTurnToMiddle()
    {
        player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
    }

    private void DebugFunctions()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (currentState == GameState.Preparation) SetState(GameState.GamePlay);
            else if (currentState == GameState.GamePlay) SetState(GameState.GameOver);
        }
        
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            if(BedRoomLightSwitch.isOn) BedRoomLightSwitch.SwitchAction(false);
            else BedRoomLightSwitch.SwitchAction(true);
        }

        if (Input.GetKeyDown(KeyCode.BackQuote) && !Input.GetKey(KeyCode.LeftShift))
        {
            debugStatsText.SetActive(!debugStatsText.activeSelf);
            debugGimmickText.SetActive(!debugGimmickText.activeSelf);
            debugTimeText.SetActive(!debugTimeText.activeSelf);
        } 

        if (Input.GetKeyDown(KeyCode.BackQuote) && Input.GetKey(KeyCode.LeftShift)) debugColiderImage.SetActive(!debugColiderImage.activeSelf);

        if (Input.GetKeyDown(KeyCode.N) && !Input.GetKey(KeyCode.LeftShift)) PlayerLevelController.Instance.OnNoiseChanged.Invoke(10);

        if (Input.GetKeyDown(KeyCode.N) && Input.GetKey(KeyCode.LeftShift)) PlayerLevelController.Instance.OnNoiseChanged.Invoke(-10);

        if (Input.GetKeyDown(KeyCode.S) && !Input.GetKey(KeyCode.LeftShift)) PlayerLevelController.Instance.OnStressChanged.Invoke(10);

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))PlayerLevelController.Instance.OnStressChanged.Invoke(-10);

        int hour = TimeManager.playTimeToMin >= 60 ? 11 + TimeManager.playTimeToMin / 60 - 12 : 11 + TimeManager.playTimeToMin / 60;
        int minute = TimeManager.playTimeToMin % 60;

        debugTimeText.GetComponent<TMP_Text>().text = $"<size=150%>{hour}h : {minute}m</size>";

        debugStatsText.GetComponent<TMP_Text>().text = 
                $"<size=130%><b>Play Time: <color=#ff808f></b>{TimeManager.playTimeToMin}/480</color></size>\n" +
                $"<size=120%><b>Camera Horizontal Value: <color=#80ffff></b>{mainCamera.transform.eulerAngles.y}</color></size>\n" +
                $"<size=120%><b>Camera Vertical Value: <color=#80ffff></b>{mainCamera.transform.eulerAngles.x}</color></size>\n" +
                $"<size=120%><b>Stress Gauge: <color=#80ffff></b>{PlayerConstant.stressLevel} / 100</color></size>\n" +
                $"<size=120%><b>Noise Stage: <color=#80ffff></b>{PlayerConstant.noiseStage}</color></size>\n" +
                $"<size=120%><b>Noise Level: <color=#80ffff></b>{PlayerConstant.noiseLevel} / 100</color></size>\n" +
                $"<size=120%><b>is Paralysis: <color=#80ffff></b>{PlayerConstant.isParalysis}</color></size>\n" +
                $"haedMoveSpeed: <color=#80ffff>{PlayerConstant.headMoveSpeed}</color>\n" +
                $"EyeClosedCAT: <color=yellow>{PlayerConstant.EyeClosedCAT}</color>\n" +
                $"EyeClosedLAT: <color=yellow>{PlayerConstant.EyeClosedLAT}</color>\n" +
                $"EyeBlinkCAT: <color=yellow>{PlayerConstant.EyeBlinkCAT}</color>\n" +
                $"EyeBlinkLAT: <color=yellow>{PlayerConstant.EyeBlinkLAT}</color>\n" +
                $"HeadMovementCAT: <color=yellow>{PlayerConstant.HeadMovementCAT}</color>\n" +
                $"HeadMovementLAT: <color=yellow>{PlayerConstant.HeadMovementLAT}</color>\n" +
                $"BodyMovementCAT: <color=yellow>{PlayerConstant.BodyMovementCAT}</color>\n" +
                $"BodyMovementLAT: <color=yellow>{PlayerConstant.BodyMovementLAT}</color>\n" +
                $"LeftStateCAT: <color=yellow>{PlayerConstant.LeftStateCAT}</color>\n" +
                $"LeftStateLAT: <color=yellow>{PlayerConstant.LeftStateLAT}</color>\n" +
                $"RightStateCAT: <color=yellow>{PlayerConstant.RightStateCAT}</color>\n" +
                $"RightStateLAT: <color=yellow>{PlayerConstant.RightStateLAT}</color>\n" +
                $"MiddleStateCAT: <color=yellow>{PlayerConstant.MiddleStateCAT}</color>\n" +
                $"MiddleStateLAT: <color=yellow>{PlayerConstant.MiddleStateLAT}</color>\n" +
                $"LeftLookCAT: <color=yellow>{PlayerConstant.LeftLookCAT}</color>\n" +
                $"LeftLookLAT: <color=yellow>{PlayerConstant.LeftLookLAT}</color>\n" +
                $"LeftFrontLookCAT: <color=yellow>{PlayerConstant.LeftFrontLookCAT}</color>\n" +
                $"LeftFrontLookLAT: <color=yellow>{PlayerConstant.LeftFrontLookLAT}</color>\n" +
                $"FrontLookCAT: <color=yellow>{PlayerConstant.FrontLookCAT}</color>\n" +
                $"FrontLookLAT: <color=yellow>{PlayerConstant.FrontLookLAT}</color>\n" +
                $"RightFrontLookCAT: <color=yellow>{PlayerConstant.RightFrontLookCAT}</color>\n" +
                $"RightFrontLookLAT: <color=yellow>{PlayerConstant.RightFrontLookLAT}</color>\n" +
                $"RightLookCAT: <color=yellow>{PlayerConstant.RightLookCAT}</color>\n" +
                $"RightLookLAT: <color=yellow>{PlayerConstant.RightLookLAT}</color>\n" +
                $"UpLookCAT: <color=yellow>{PlayerConstant.UpLookCAT}</color>\n" +
                $"UpLookLAT: <color=yellow>{PlayerConstant.UpLookLAT}</color>\n" +
                $"DownLookCAT: <color=yellow>{PlayerConstant.DownLookCAT}</color>\n" +
                $"DownLookLAT: <color=yellow>{PlayerConstant.DownLookLAT}</color>\n";
        debugGimmickText.GetComponent<TMP_Text>().text = 
                $"<size=130%><b>Neigbor State<color=#ff808f>\n{neighborGimmickScript.CurrState.Name}</color></size>\n\n" +
                $"<size=130%><b>Parents State<color=#ff808f>\n{parentsGimmickScript.CurrState.Name}</color></size>";
        #endif
    }
    
    private void OnApplicationQuit()
    {
        if (isBlinkInit) BlinkEffect.Blink = 1f;
        else BlinkEffect.Blink = 0.001f;
        
        SaveManager.Instance.SaveLastPlayedTime(DateTime.Now.ToString("yyyyMMddHHmmss"));
    }
}
