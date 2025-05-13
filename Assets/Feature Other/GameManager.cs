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
using VFolders.Libs;
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
    private bool isTutorialEnable;
    public bool tutorialTestEnable;
    
    private float lastLeftClickTime = -1f;
    private float lastRightClickTime = -1f;

    [Tooltip("동시 클릭 허용 시간")]
    public float bothClickToleranceTime;
    
    [Header("데모")]
    // 데모 씬 전용. 데모 씬은 반드시 이름이 Demo여야 함.
    
    [SerializeField]
    public bool isDemo = false;

    public bool isDemoActive = false;
    private Coroutine demoCoroutine = null;
    
    [Tooltip("데모 씬에서 총 기믹 시간. 기본값 300")]
    private int totalTime = 0;
    
    [FormerlySerializedAs("nonMarkovGimmickActiveTime")]
    [Space]
    [SerializeField]
    [Tooltip("맨 처음에 기믹이 활성화 되는 시간. 기본값 20")]
    private int randomGimmickActiveTime = 20;
    
    [FormerlySerializedAs("nonMarkovGimmickInterval")]
    [SerializeField]
    [Tooltip("이웃, 부모 기믹이 아닌 기믹들이 출현할 때 \n5, 4, 3, 2초씩 출현하는데 각 초 유지 시간 간격")]
    private Vector2Int[] randomGimmickInterval = new Vector2Int[4]
    {
        new Vector2Int(20, 40),
        new Vector2Int(40, 50),
        new Vector2Int(50, 58),
        new Vector2Int(58, 65)
    };
    
    [Space]
    [SerializeField]
    [Tooltip("이웃, 부모 기믹이 출현하는 시간. 기본값 30")]
    private int markovGimmickActiveTime = 30;
    
    [SerializeField]
    private List<MarkovGimmickData> demoGimmicks = new();

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
        DateTime ago = DateTime.ParseExact(SaveManager.Instance.LoadLastPlayedTime(), "yyyyMMddHHmm", CultureInfo.InvariantCulture);
        //현재 플레이 시간 가져옴
        DateTime now = DateTime.Now;

        //시간 비교
        TimeSpan timeDifference = now - ago;

        isTutorialEnable = timeDifference.TotalHours >= 48.0d;

        //48시간 이후 접속 여부
        if (isTutorialEnable == true)
        {
            //튜토리얼 실행 코드 작성
        }
        else
        {

        }
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

        if (tutorialTestEnable) TutorialManager.Instance.EyeOpenTutorialStart();
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

            if (PlayerConstant.isLeftState && PlayerConstant.isEyeOpen && !tutorialTestEnable)
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
        if (isDemo) demoCoroutine = StartCoroutine(DemoCoroutine());
    }
    
    private void GameOver()
    {
        Debug.Log("GameOver !!");
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        // 테스트용
        tutorialTestEnable = true;

        player.EyeControl(PlayerEyeStateTypes.Close);
        PlayerConstant.isShock = true;
        Invoke(nameof(DelayTurnToMiddle), 0.1f);
        yield return new WaitForSeconds(1f);
        SetState(GameState.Preparation);
    }
    
    public void StopDemoCoroutine()
    {
        if (isDemo == false) return;
        
        if (demoCoroutine != null)
        {
            isDemoActive = false;
            StopCoroutine(demoCoroutine);
            demoCoroutine = null;
        }
    }

    IEnumerator DemoCoroutine()
    {
        var activeSecTimesTmp = new List<int>();
        
        foreach (var demoGimmick in demoGimmicks)
        {
            if (demoGimmick.activeSecTime < markovGimmickActiveTime)
            {
                Debug.LogWarning("Markov Gimmick Active Time이 Demo Gimmick의 첫 번째 활성화 시간보다 작습니다.");
                yield break;
            }
        }

        totalTime = demoGimmicks[demoGimmicks.Count - 1].activeSecTime + 5;
        
        MarkovGimmick markovGimmick = null, exMarkovGimmick = null;
        Gimmick currGimmick = null, exGimmick = null;

        var demoTimer = 0f;
        
        isDemoActive = true;
        StartCoroutine(StartMarkovGimmicks());
        StartCoroutine(StartRandomGimmicks());
        StartCoroutine(StartStressLevelAdd());
        
        yield return new WaitWhile(() =>
        {
            if (demoTimer >= totalTime || isDemoActive == false) return false;        
            demoTimer += Time.deltaTime;
            
            return true;
        });

        isDemoActive = false;

        if (demoTimer >= totalTime)
        {
            if (PlayerConstant.isLeftState || PlayerConstant.isLeftFrontLook)
            {
                currGimmick = GimmickManager.Instance.GetGimmick("Neighbor");
                exGimmick = GimmickManager.Instance.GetGimmick("Parents");
                Debug.Log("PlayerConstant.isLeftState - " + currGimmick.name );
            }
            else if (PlayerConstant.isRightState || PlayerConstant.isRightFrontLook|| PlayerConstant.isEyeOpen == false)
            {
                currGimmick = GimmickManager.Instance.GetGimmick("Parents");
                exGimmick = GimmickManager.Instance.GetGimmick("Neighbor");
                Debug.Log("PlayerConstant.isRightState || PlayerConstant.isEyeOpen == false - " + currGimmick.name );
            }
            else
            {
                var ran = Random.Range(0, 1);
                var randomGimmick = ran == 0 ? "Neighbor" : "Parents";
                currGimmick = GimmickManager.Instance.GetGimmick(randomGimmick);
                exGimmick = GimmickManager.Instance.GetGimmick(randomGimmick == "Neighbor" ? "Parents" : "Neighbor");
                Debug.Log("Random - " + currGimmick.name );
            }
            
            markovGimmick = currGimmick as MarkovGimmick;
            exMarkovGimmick = exGimmick as MarkovGimmick;
            exMarkovGimmick?.ChangeMarkovState(MarkovGimmickData.MarkovGimmickType.Wait);
            markovGimmick?.ChangeMarkovState(MarkovGimmickData.MarkovGimmickType.Near);
            // if(markovGimmick?.CurrGimmickType != MarkovGimmickData.MarkovGimmickType.Near)
            //     markovGimmick?.ChangeMarkovState(markovGimmick?.Near);
        }
        else
        {
            var neighborGimmick = GimmickManager.Instance.GetGimmick("Neighbor") as MarkovGimmick;
            var parentsGimmick = GimmickManager.Instance.GetGimmick("Parents") as MarkovGimmick;

            if (neighborGimmick.CurrGimmickType == MarkovGimmickData.MarkovGimmickType.Near)
                parentsGimmick?.ChangeMarkovState(MarkovGimmickData.MarkovGimmickType.Wait);
            else if (parentsGimmick.CurrGimmickType == MarkovGimmickData.MarkovGimmickType.Near)
                neighborGimmick?.ChangeMarkovState(MarkovGimmickData.MarkovGimmickType.Wait);
        }
        
        
        // 기믹 활성화 시간 초기화
        for (int i = 0; i < demoGimmicks.Count; i++)
        {
            demoGimmicks[i].activeSecTime = activeSecTimesTmp[i];
        }
        yield break;

        ////////////////////////////////////////////////////////////////////////////////////////////
        IEnumerator StartMarkovGimmicks()
        {
            yield return new WaitForSeconds(markovGimmickActiveTime);
            
            var markovGimmickTimer = 0f;
            var idx = 0;

            while (true)
            {
                // 인스펙터에서 정한 시간이 되면 정해진 상태로 변환
                if (demoGimmicks[idx].activeSecTime == (int) demoTimer)
                {
                    if (PlayerConstant.isLeftState || PlayerConstant.isLeftFrontLook)
                    {
                        currGimmick = GimmickManager.Instance.GetGimmick("Neighbor");
                        exGimmick = GimmickManager.Instance.GetGimmick("Parents");
                        // Debug.Log("PlayerConstant.isLeftState - " + currGimmick.name );
                    }
                    else if (PlayerConstant.isRightState || PlayerConstant.isRightFrontLook|| PlayerConstant.isEyeOpen == false)
                    {
                        currGimmick = GimmickManager.Instance.GetGimmick("Parents");
                        exGimmick = GimmickManager.Instance.GetGimmick("Neighbor");
                        // Debug.Log("PlayerConstant.isRightState || PlayerConstant.isEyeOpen == false - " + currGimmick.name );
                    }
                    else
                    {
                        var ran = Random.Range(0, 1);
                        var randomGimmick = ran == 0 ? "Neighbor" : "Parents";
                        currGimmick = GimmickManager.Instance.GetGimmick(randomGimmick);
                        exGimmick = GimmickManager.Instance.GetGimmick(randomGimmick == "Neighbor" ? "Parents" : "Neighbor");
                        // Debug.Log("Random - " + currGimmick.name );
                    }

                    markovGimmick = currGimmick as MarkovGimmick;
                    exMarkovGimmick = exGimmick as MarkovGimmick;
                    GimmickManager.Instance.ExclusionGimmick(currGimmick);
                    markovGimmick?.ChangeMarkovState(demoGimmicks[idx].type);
                    exMarkovGimmick?.ChangeMarkovState(MarkovGimmickData.MarkovGimmickType.Wait);
                    activeSecTimesTmp.Add(demoGimmicks[idx].activeSecTime);
                    demoGimmicks[idx].activeSecTime = 0;
                    idx = idx < demoGimmicks.Count - 1 ? idx + 1 : idx;
                }
                if (demoTimer >= totalTime || isDemoActive == false) yield break;

                yield return null;
            }
        }

        IEnumerator StartRandomGimmicks()
        {
            // 1분 되면 랜덤 기믹 활성화
            yield return new WaitForSeconds(randomGimmickActiveTime);

            var randomGimmickIntervalIdx = 0;
            var initInterval = 5;
            while (true)
            {
                if (demoTimer >= randomGimmickInterval[randomGimmickIntervalIdx].y)
                {
                    randomGimmickIntervalIdx = 
                        randomGimmickIntervalIdx < randomGimmickInterval.Length - 1 ? 
                            randomGimmickIntervalIdx + 1 : randomGimmickIntervalIdx;
                    initInterval--;
                }
                
                if (demoTimer >= randomGimmickInterval[randomGimmickIntervalIdx].x 
                    && demoTimer < randomGimmickInterval[randomGimmickIntervalIdx].y)
                {
                    yield return new WaitForSeconds(initInterval);
                    GimmickManager.Instance.PickDemoGimmick();
                }
                
                if (demoTimer >= totalTime || isDemoActive == false) yield break;

                yield return null;
            }
        }

        IEnumerator StartStressLevelAdd()
        {
            int startTime = randomGimmickActiveTime + 30;
            float increasingDuration = totalTime - startTime;
            float accumulated = 0f;
            int previousLevel = 0;

            yield return new WaitForSeconds(startTime);
            float timer = 0f;
            while (timer < increasingDuration)
            {
                float delta = Time.deltaTime;
                timer += delta;

                accumulated += 100f / increasingDuration * delta;
                int level = Mathf.FloorToInt(accumulated);

                if (level > previousLevel)
                {
                    int diff = level - previousLevel;
                    previousLevel = level;
                    PlayerLevelController.Instance.OnStressChanged.Invoke(diff);
                }
        
                if (demoTimer >= totalTime || isDemoActive == false) yield break;

                yield return null;
            }
        }
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
    
    protected override void OnApplicationQuit()
    {
        if (isBlinkInit) BlinkEffect.Blink = 1f;
        else BlinkEffect.Blink = 0f;
    }

    public void GameEnd()
    {
        SaveManager.Instance.SaveLastPlayedTime(DateTime.Now.ToString("yyyyMMddHHmm"));

        //에디터에서는 코드 동작 안함
        Debug.Log("게임 끝");
        Application.Quit();
    }
}
