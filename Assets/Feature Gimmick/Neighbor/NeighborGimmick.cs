using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using AYellowpaper.SerializedCollections;
using Bed.Collider;
using DG.Tweening;
using Unity.VisualScripting;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class NeighborGimmick : Gimmick, IMarkovGimmick
{
    #region Override Variables
    public string GimmickName { get; protected set; } = "Neighbor";
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [SerializeField] private float _probability;
    public override float probability 
    { 
        get => _probability; 
        set => _probability = Mathf.Clamp(value, 0, 100); 
    }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    public GameObject hand, houseLight;
    public GameObject neighbor;
    public GameObject neighborHead;
    private Animator animator;
    
    [Range(0, 100)]
    private int moveProbability = 0;                // 초기값 0. 최댓값 100. moveChance보다 크면 움직임
    
    [Range(0, 100)]
    private int statePassByNextValue = 35;          // 초기값 35. 눈을 감거나 오른족을 안 보고 잇으면 -5.
    
    private float stateTransitionProbability = 0;     // 상태가 변화할 확률 변화 값
    public float StateTransitionProbability
    {
        get => stateTransitionProbability;
        set => stateTransitionProbability = Mathf.Clamp(value, 0, 100);
    }
    
    private float stateTransitionDecisionValue = 0;   // 상태 변화 확률 최종 값

    public float StateTransitionDecisionValue
    {
        get => stateTransitionDecisionValue;
        set => stateTransitionDecisionValue = Mathf.Clamp(value, 0, 100);
    }
    private Coroutine stateProbabilityChangeCoroutine = null;
    
    private MarkovChain chain = new MarkovChain();
    private Coroutine markovCoroutine = null;
    
    public MarkovState CurrState { get; set; } = null;
    public MarkovGimmickData.MarkovGimmickType CurrGimmickType { get; set; }
    public bool IsOn { get; set; } = false;

    private List<MarkovState> statesWithoutNear = new List<MarkovState>();
    public MarkovState Wait { get; set; }       = new MarkovState("Wait");
    public MarkovState Watch { get; set; }      = new MarkovState("Watch");
    public MarkovState Cautious { get; set; }   = new MarkovState("Cautious");
    public MarkovState Danger { get; set; }     = new MarkovState("Danger");
    public MarkovState Near { get; set; }       = new MarkovState("Near");
    
    [SerializeField] private BreathSound breathSound;
    
    [Header("설정")]
    
    [SerializeField]
    [Tooltip("특정 상황에 변화하는 상태 확률값")]
    private List<StateProbabilityData> stateProbabilities = new();

    [SerializeField]
    [Tooltip("눈 쳐다볼 때 감소하는 상태 확률. Threshold는 안 씀.")]
    private List<StateProbabilityData> stateProbabilitiesLookEye = new();
    
    private Coroutine eyeCloseCheckCoroutine = null;
    private Coroutine cautiousCheckCoroutine = null;
    #endregion
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            stateTransitionProbability += 15;
            Debug.Log(stateTransitionProbability);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            stateTransitionProbability -= 15;
            Debug.Log(stateTransitionProbability);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && probability == 100)
        {
            ChangeMarkovState(Wait);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && probability == 100)
        {
            ChangeMarkovState(Watch);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && probability == 100)
        {
            ChangeMarkovState(Danger);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && probability == 100)
        {
            ChangeMarkovState(Near);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        statesWithoutNear.Add(Wait);
        statesWithoutNear.Add(Watch);
        statesWithoutNear.Add(Cautious);
        statesWithoutNear.Add(Danger);
        
        Wait.OnStateAction += ActiveStateCoroutine;
        Watch.OnStateAction += ActiveStateCoroutine;
        Cautious.OnStateAction += ActiveStateCoroutine;
        Danger.OnStateAction += ActiveStateCoroutine;
        Near.OnStateAction += ActiveStateCoroutine;

        chain.InsertTransition(Wait,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = Watch, ThresholdRange       = new Vector2(0, 30) },
                new MarkovTransition { Target = Cautious, ThresholdRange    = new Vector2 (31, 60) },
                new MarkovTransition { Target = Danger, ThresholdRange      = new Vector2 (61, 80) },
                new MarkovTransition { Target = Near, ThresholdRange        = new Vector2 (81, 100) }
            }
        );
        
        chain.InsertTransition(Watch, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = Wait, ThresholdRange        = new Vector2(0, 10) },
                new MarkovTransition { Target = Cautious, ThresholdRange    = new Vector2(11, 45) },
                new MarkovTransition { Target = Danger, ThresholdRange      = new Vector2(46, 70) },
                new MarkovTransition { Target = Near, ThresholdRange        = new Vector2(71, 100) }
            }
        );
        
        chain.InsertTransition(Cautious, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = Wait, ThresholdRange        = new Vector2(0, 10) },
                new MarkovTransition { Target = Watch, ThresholdRange       = new Vector2(11, 15) },
                new MarkovTransition { Target = Danger, ThresholdRange      = new Vector2(16, 40) },
                new MarkovTransition { Target = Near, ThresholdRange        = new Vector2(41, 100) }
            }
        );
        
        chain.InsertTransition(Danger, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = Wait, ThresholdRange        = new Vector2(0, 5) },
                new MarkovTransition { Target = Watch, ThresholdRange       = new Vector2(6, 10) },
                new MarkovTransition { Target = Cautious, ThresholdRange    = new Vector2(11, 15) },
                new MarkovTransition { Target = Near, ThresholdRange        = new Vector2(16, 100) }
            }
        );

        ChangeMarkovState(Wait);
    }

    public override void Activate()
    {
        base.Activate();
        IsOn = true;
        ChangeMarkovState(Watch);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        IsOn = false;
    }

    public override void UpdateProbability()
    {
        moveProbability = PlayerConstant.noiseStage * -10;
        probability = 0 < moveProbability ? 100 : 0;
        if (Mathf.Approximately(probability, 0) && !CurrState.Equals(Wait)) ChangeMarkovState(Wait);
        
        // 특정 상황에 상태 확률을 decision으로 지정
        stateTransitionDecisionValue = stateTransitionProbability;

        if (GameManager.Instance.isDemo == false && IsOn == true)
        {
            ChangeStateProbability();
            ChangeStateProbabilitySeeingNeighbor(Watch);
            Debug.Log("Neighbor State Probability " + stateTransitionProbability);
        }
    }

    public void ChangeRandomMarkovState()
    {
        ChangeMarkovState(statesWithoutNear[Random.Range(0, statesWithoutNear.Count)]);
    }

    public void ChangeMarkovState(MarkovState next)
    {
        CurrState = next;
        
        if(eyeCloseCheckCoroutine != null && !next.Equals(Danger))
        {
            StopCoroutine(eyeCloseCheckCoroutine);
            eyeCloseCheckCoroutine = null;
        }
        
        if(cautiousCheckCoroutine != null && !next.Equals(Cautious))
        {
            StopCoroutine(cautiousCheckCoroutine);
            cautiousCheckCoroutine = null;
        }
        CurrState.Active();
    }

    public void ChangeMarkovState(MarkovGimmickData.MarkovGimmickType type)
    {
        switch (type)
        {
            case MarkovGimmickData.MarkovGimmickType.Wait:
                ChangeMarkovState(Wait);
                break;
            case MarkovGimmickData.MarkovGimmickType.Watch:
                ChangeMarkovState(Watch);
                break;
            case MarkovGimmickData.MarkovGimmickType.Cautious:
                ChangeMarkovState(Cautious);
                break;
            case MarkovGimmickData.MarkovGimmickType.Danger:
                ChangeMarkovState(Danger);
                break;
            default:
                break;
        }

        CurrGimmickType = type;
    }
    
    private void ActiveStateCoroutine(MarkovState state)
    {
        if (markovCoroutine != null) StopCoroutine(markovCoroutine);
        stateTransitionProbability = 0;
        stateTransitionDecisionValue = 0;
        markovCoroutine = StartCoroutine(ActiveMarkovState(state));
    }

    private IEnumerator ActiveMarkovState(MarkovState state)
    {
        CurrState = state;
        
        if (stateProbabilityChangeCoroutine != null)
        {
            StopCoroutine(stateProbabilityChangeCoroutine);
            stateProbabilityChangeCoroutine = null;
        }
        
        switch (state)
        {
            case var _ when state.Equals(Wait):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                PlayAnimationWithoutDuplication(state.Name);
                Deactivate();
                break;

            case var _ when state.Equals(Watch):
                if(!houseLight.activeSelf)houseLight.SetActive(true);
                if(hand.activeSelf)       hand.SetActive(false);
                PlayAnimationWithoutDuplication(state.Name);
                break;

            case var _ when state.Equals(Cautious):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                PlayAnimationWithoutDuplication(state.Name);

                cautiousCheckCoroutine ??= StartCoroutine(CheckNeighborHeadCollision());
                break;

            case var _ when state.Equals(Danger):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                PlayAnimationWithoutDuplication(state.Name);

                eyeCloseCheckCoroutine ??= StartCoroutine(ChangeNearWhenEyeClose());
                break;

            case var _ when state.Equals(Near):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(!hand.activeSelf)      hand.SetActive(false);
                
                GimmickManager.Instance.DeactivateGimmicks(this);
                PlayAnimationWithoutDuplication(Danger.Name);
                
                TimeManager.Instance.isGameOver = true;
                var nearTimer = 0f;
                var sequence = DOTween.Sequence();
                sequence.Append(DOTween.To(() => nearTimer, x => nearTimer = x, 10f, 10f))
                    .OnUpdate(() =>
                    {
                        if (!PlayerConstant.isLeftState) sequence.Complete();
                    });
                
                yield return new DOTweenCYInstruction.WaitForCompletion(sequence);
                
                if (PlayerConstant.isLeftState)
                {
                    GameManager.Instance.player.DirectionControl(PlayerDirectionStateTypes.Middle);
                }
                yield return new WaitUntil(() => PlayerConstant.isMiddleState);
                StartCoroutine(GameManager.Instance.player.LookAt(neighborHead, 0.2f)); // TODO: 특정 오브젝트 대상
                GameManager.Instance.player.ForceOpenEye();
                PlayerConstant.isParalysis = true; // 조작이 불가능한 상태로 변경
                yield return new WaitForSeconds(0.2f);  // 대기
                
                breathSound.ToggleBreath(); // 숨 참음
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.ControlDText(true, "Neighbor"); // D text 활성화
                PlayerConstant.isPillowSound = false;
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.neighborD, this.transform.position);
                GameManager.Instance.player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
                PlayAnimationWithoutDuplication(Near.Name);
                if(!hand.activeSelf) hand.SetActive(true); // 손 활성화
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.ControlDText(false, "Neighbor"); // D text 비활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.gag, this.transform.position);
                PlayerConstant.isParalysis = false; // 조작 가능하게 변경
                PlayerConstant.isRedemption = true; // 몸을 못돌리는 상태로 변경
                PlayerConstant.isPillowSound = true;
                
                StartCoroutine(GameManager.Instance.player.StayLookAt(neighborHead, 3f)); // TODO: 특정 오브젝트 대상
                yield return new WaitForSeconds(3f); // 대기 
                
                PlayerConstant.isPillowSound = false;
                UIManager.Instance.ControlNText(true, "Neighbor"); // n text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.neighborN, this.transform.position);
                yield return new WaitForSeconds(1.5f); // 대기
                
                GameManager.Instance.SetState(GameState.GameOver); // 게임 오버 상태로 변경 (준비 상태로 초기화)

                yield return new WaitForSeconds(1f); // 대기
                PlayerConstant.isPillowSound = true;
                breathSound.ToggleBreath(); // 숨 참음 해제
                ChangeMarkovState(Wait); // 기믹은 대기 상태로 변경
                PlayerConstant.isRedemption = false; // 몸 돌릴수 있게
                TimeManager.Instance.isGameOver = false;
                UIManager.Instance.ControlNText(false, "Neighbor"); // n text 비활성화
                yield break;
        }

        yield return new WaitUntil(() => IsOn == true);
        yield return new WaitForSeconds(TimeManager.Instance.CycleInterval);
        if (statePassByNextValue <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            CurrState = chain.TransitionNextState(CurrState, Mathf.RoundToInt(stateTransitionDecisionValue));
        }
        else
        {
            CurrState = chain.TransitionNextState(CurrState);
        }
        
        Debug.Log("Next : " + CurrState.Name);
        yield break;
        
        //////////////////////////////////////////////////////////////////////////////////////
        IEnumerator ChangeNearWhenEyeClose()
        {
            var timer = 0f;
            
            while (true)
            {
                if (PlayerConstant.isEyeOpen == false)
                {
                    timer += Time.deltaTime;
                    
                    if (timer >= 0.5f)
                    {
                        GameManager.Instance.player.ForceOpenEye();
                        // GameManager.Instance.StopDemoCoroutine();
                        ChangeMarkovState(Near);
                        eyeCloseCheckCoroutine = null;
                        yield break;
                    }
                }
                else
                {
                    timer = 0f;
                }
                
                yield return null;
            }
        }
        
        IEnumerator CheckNeighborHeadCollision()
        {
            while (true)
            {
                if (BlinkEffect.Blink <= 0.85f && 
                    ConeCollider.TriggeredObject != null &&
                    ConeCollider.TriggeredObject.Equals(neighborHead))
                {
                    // GameManager.Instance.StopDemoCoroutine();
                    GimmickManager.Instance.ChangeAllMarkovGimmickState(MarkovGimmickData.MarkovGimmickType.Wait);

                    yield break;
                }
        
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// 현재 상태에 따라 특정 상황이 되면 다음 상태로 갈 확률이 증가하거나 감소하는 코루틴
    /// </summary>
    private void ChangeStateProbability()
    {
        if (CurrState.Equals(Wait) || CurrState.Equals(Near)) return;
        
        foreach (var data in stateProbabilities)
        {
            if (data.type != CurrState.Type) continue;

            var probabilityValue = (float) data.probabilityChangeValue;
            
            // 눈을 감고 있으면서 몸이 왼쪽으로 돌아가 있거나 왼쪽이나 왼쪽 앞을 쳐다보고 있으면 확률 2배 증가
            if (!PlayerConstant.isEyeOpen &&
                (PlayerConstant.isLeftState || PlayerConstant.isLeftLook || PlayerConstant.isLeftFrontLook))
            {
                probabilityValue *= 2f;
            }
            // 몸이 왼쪽으로 돌아가 있거나 왼쪽이나 왼쪽 앞을 쳐다보거나 눈을 감고 있으면 확률 1.5배 증가
            else if (PlayerConstant.isLeftState ||
                PlayerConstant.isLeftLook ||
                PlayerConstant.isLeftFrontLook ||
                !PlayerConstant.isEyeOpen)
            {
                probabilityValue *= 1.5f;
            }
            
            // Noise Level이 높으면 상태 확률 감소
            if (IsLoud(data.noiseThreshold))
            {
                StateTransitionProbability -= probabilityValue;
            }
            // Noise Level이 낮으면 상태 확률 증가
            else if (!IsLoud(data.noiseThreshold))
            {
                StateTransitionProbability += probabilityValue;
            }
        }
    }

    bool IsLoud(int noise) => PlayerConstant.noiseLevel >= noise;
    
    /// <summary>
    /// 이웃을 보고 있으면 매개변수 상태 별로 값이 다르게 다음 상태 확률을 증감하는 코루틴
    /// </summary>
    /// <param name="state"></param>
    private void ChangeStateProbabilitySeeingNeighbor(MarkovState state)
    {
        if (!CurrState.Equals(state)) return;
        
        foreach (var data in stateProbabilitiesLookEye)
        {
            if (data.type != state.Type) continue;

            // 이웃을 보고 있으면 상태 확률 감소
            if (ConeCollider.TriggeredObject&& ConeCollider.TriggeredObject.Equals(neighborHead))
            {
                StateTransitionProbability -= data.probabilityChangeValue;
            }
            // 이웃을 안 보고 있으면 상태 확률 증가
            else if ((ConeCollider.TriggeredObject&& !ConeCollider.TriggeredObject.Equals(neighborHead)) || !ConeCollider.TriggeredObject)
            {
                StateTransitionProbability += data.probabilityChangeValue;
            }
        }
    }

    private void PlayAnimationWithoutDuplication(string animName)
    {
        // 현 애니메이션이 전 애니메이션이랑 같지 않을 때만 애니메이션 재생
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            animator.SetTrigger(animName);
        }
    }

    private void WindowOpenCloseSoundPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.windowOpenClose, this.transform.position);
    }
    private void RustleSoundPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.pantRustle, this.transform.position);
    }
    // private void GagSoundPlay()
    // {
    //     AudioManager.Instance.PlayOneShot(AudioManager.Instance.gag, this.transform.position);
    // }
    private void HornyBreathSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.hornyBreath, this.transform.position);
    }

    public override void Initialize()
    {
        if(houseLight.activeSelf) houseLight.SetActive(false);
        if(hand.activeSelf)       hand.SetActive(false);
        if (!CurrState.Equals(Wait)) ChangeMarkovState(Wait);
    }
}
