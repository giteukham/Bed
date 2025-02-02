using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
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
    #endregion

    #region Main Camera
    [Header("Main Camera")]
    [SerializeField] private Camera mainCamera;
    #endregion

    #region Reference Components
    [Header("Reference Components")]
    [SerializeField] private Player player;
    #endregion
    
    #region Objects related Components
    [Header("Objects related Components")]
    [SerializeField] private GameObject mother;
    private Animator motherAnimator;
    #endregion

    #region Quit Settings
    [Header("Quit Settings")]
    [SerializeField] private bool isBlinkInit;
    #endregion
    
    #region Managers
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private TutorialManager tutorialManager;
    #endregion
    
    private GameState currentState;
    
    void Awake()
    {
        motherAnimator = mother.GetComponent<Animator>();
        //InputSystem.Instance.OnMouseClickEvent += () => PlayerConstant.isPlayerStop = false;

        //예전 마지막 플레이 시간 가져옴
        DateTime ago = DateTime.ParseExact(SaveManager.Instance.LoadLastPlayedTime(), "yyyyMMddHHmm", CultureInfo.InvariantCulture);
        //현재 플레이 시간 가져옴
        DateTime now = DateTime.Now;

        //시간 비교
        TimeSpan timeDifference = now - ago;
        Debug.Log(timeDifference.TotalHours);

        //게임 실행시 마지막 접속이후 48시간 넘었는지 확인하고 저장
        if (timeDifference.TotalHours > 48.0d)
        {
            SaveManager.Instance.SaveIsTwoDaysLater(true);
        }
        else
        {
            SaveManager.Instance.SaveIsTwoDaysLater(false);
        }

        //첫게임 플레이인지 여부 || 48시간 이후 접속 여부
        if (SaveManager.Instance.LoadIsFirstPlay() || SaveManager.Instance.LoadIsTwoDaysLater())
        {
            //튜토리얼 실행 코드 작성
            
            //첫게임 플레이 여부 false
            SaveManager.Instance.SaveIsFirstPlay(false);
        }
        else
        {

        }
    }

    void Start()
    {
        SetState(GameState.Preparation);
        
        #if UNITY_EDITOR
            if (debugStatsText.activeSelf) debugStatsText.SetActive(false);
            if (debugTimeText.activeSelf) debugTimeText.SetActive(false);
            if (debugColiderImage.activeSelf) debugColiderImage.SetActive(false);
        #endif
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.GetMouseButtonDown(1) ||
            (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1)) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (PlayerConstant.isPlayerStop == true) PlayerConstant.isPlayerStop = false;
            else if (PlayerConstant.isPlayerStop == false) PlayerConstant.isPlayerStop = true;
        }

        #if UNITY_EDITOR
            DebugFunctions();
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
        Door.SetNoSound(0, 0);
        PlayerConstant.isShock = false;
        tutorialManager.ReadyTutorial();
    }
    
    private void GamePlay()
    {
        Debug.Log("GamePlay !!");
        StartCoroutine(GamePlayCoroutine());
        // Door.SetNoSound(50, 0.5f);
        // BedRoomLightSwitch.SwitchAction(false);
        // timeManagerObject.SetActive(true);
    }
    
    IEnumerator GamePlayCoroutine()
    {
        tutorialManager.IsNotOpen(true);
        tutorialManager.StopDoorKnock();
        yield return new WaitForSeconds(1.5f);
        Door.Set(30, 0.15f);
        yield return new WaitForSeconds(0.2f);
        mother.SetActive(true);

        float randomNum = UnityEngine.Random.Range(3f, 6f);
        yield return new WaitForSeconds(randomNum);

        BedRoomLightSwitch.SwitchAction(false);
        yield return new WaitForSeconds(0.25f);
        motherAnimator.SetTrigger("Leave");
        yield return new WaitForSeconds(0.1f);
        Door.Set(0, 0.4f);
        yield return new WaitForSeconds(0.25f);
        mother.SetActive(false);
        //LivingRoomLightSwitch.SwitchAction(false);
        yield return new WaitForSeconds(1f);
        timeManager.gameObject.SetActive(true);
    }
    
    private void GameOver()
    {
        Debug.Log("GameOver !!");
        player.EyeControl(PlayerEyeStateTypes.Close);
        PlayerConstant.isShock = true;
        Invoke(nameof(DelayTurnToMiddle), 0.1f);
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
            else if (currentState == GameState.GameOver) SetState(GameState.Preparation);
        }
        
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            if(BedRoomLightSwitch.isOn) BedRoomLightSwitch.SwitchAction(false);
            else BedRoomLightSwitch.SwitchAction(true);
        }

        if (Input.GetKeyDown(KeyCode.BackQuote) && !Input.GetKey(KeyCode.LeftShift))
        {
            debugStatsText.SetActive(!debugStatsText.activeSelf);
            debugTimeText.SetActive(!debugTimeText.activeSelf);
        } 

        if (Input.GetKeyDown(KeyCode.BackQuote) && Input.GetKey(KeyCode.LeftShift)) debugColiderImage.SetActive(!debugColiderImage.activeSelf);

        if (Input.GetKeyDown(KeyCode.F) && !Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, 10);

        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, -10);

        if (Input.GetKeyDown(KeyCode.S) && !Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, 10);

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, -10);

        int hour = TimeManager.playTimeToMin >= 60 ? 11 + TimeManager.playTimeToMin / 60 - 12 : 11 + TimeManager.playTimeToMin / 60;
        int minute = TimeManager.playTimeToMin % 60;

        debugTimeText.GetComponent<TMP_Text>().text = $"<size=150%>{hour}h : {minute}m</size>";

        debugStatsText.GetComponent<TMP_Text>().text = 
                $"<size=130%><b>Play Time: <color=#ff808f></b>{TimeManager.playTimeToMin}/480</color></size>\n" +
                $"<size=120%><b>Camera Horizontal Value: <color=#80ffff></b>{mainCamera.transform.eulerAngles.y}</color></size>\n" +
                $"<size=120%><b>Camera Vertical Value: <color=#80ffff></b>{mainCamera.transform.eulerAngles.x}</color></size>\n" +
                $"<size=120%><b>Stress Gauge: <color=#80ffff></b>{PlayerConstant.stressGauge} / 100</color></size>\n" +
                $"<size=120%><b>Fear Gauge: <color=#80ffff></b>{PlayerConstant.fearGauge} / 100</color></size>\n" +
                $"isParalysis: <color=#80ffff>{PlayerConstant.isParalysis}</color>\n" +
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

        #endif
    }
    
    protected override void OnApplicationQuit()
    {
        if (isBlinkInit) BlinkEffect.Blink = 1f;
    }

    public void GameEnd()
    {
        SaveManager.Instance.SaveLastPlayedTime(DateTime.Now.ToString("yyyyMMddHHmm"));

        //에디터에서는 코드 동작 안함
        Debug.Log("게임 끝");
        Application.Quit();
    }
}
