using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using AbstractGimmick;
using Bed.Collider;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using UnityEngine.Playables;
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
    private int statePassByNextValue = 35;    // 초기값 35. 눈을 감거나 오른족을 안 보고 잇으면 -5.
    
    private int stateTransitionProbability = 0;     // 상태가 변화할 확률 변화 값
    public int StateTransitionProbability
    {
        get => stateTransitionProbability;
        set => stateTransitionProbability = Mathf.Clamp(value, 0, 100);
    }
    
    private int stateTransitionDecisionValue = 0;   // 상태 변화 확률 최종 값

    public int StateTransitionDecisionValue
    {
        get => stateTransitionDecisionValue;
        set => stateTransitionDecisionValue = Mathf.Clamp(value, 0, 100);
    }
    private Coroutine stateProbabilityChangeCoroutine = null;
    
    private MarkovChain chain = new MarkovChain();
    private Coroutine markovCoroutine = null;
    
    public MarkovState CurrState { get; set; } = null;
    public MarkovGimmickData.MarkovGimmickType CurrGimmickType { get; set; }

    private List<MarkovState> statesWithoutNear = new List<MarkovState>();
    public MarkovState Wait { get; set; }       = new MarkovState("Wait");
    public MarkovState Watch { get; set; }      = new MarkovState("Watch");
    public MarkovState Cautious { get; set; }   = new MarkovState("Cautious");
    public MarkovState Danger { get; set; }     = new MarkovState("Danger");
    public MarkovState Near { get; set; }       = new MarkovState("Near");
    
    [SerializeField] private BreathSound breathSound;
    
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
        ChangeMarkovState(Watch);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void UpdateProbability()
    {
        moveProbability = PlayerConstant.noiseStage * -10;
        probability = 0 < moveProbability ? 100 : 0;
        if (Mathf.Approximately(probability, 0) && !CurrState.Equals(Wait)) ChangeMarkovState(Wait);
        
        // 특정 상황에 상태 확률을 decision으로 지정
        stateTransitionDecisionValue = stateTransitionProbability;
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

        if (!GameManager.Instance.isDemo)
        {
            stateProbabilityChangeCoroutine = StartCoroutine(ChangeNeighborStateProbability());
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
                
                PlayerConstant.isParalysis = true; // 조작이 불가능한 상태로 변경
                yield return new WaitForSeconds(0.2f);  // 대기
                
                breathSound.ToggleBreath(); // 숨 참음
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.SetGameOverScreen(name); 
                UIManager.Instance.ActiveOrDeActiveDText(true); // D text 활성화
                PlayerConstant.isPillowSound = false;
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.neighborD, this.transform.position);
                GameManager.Instance.player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
                PlayAnimationWithoutDuplication(Near.Name);
                if(!hand.activeSelf) hand.SetActive(true); // 손 활성화
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.ActiveOrDeActiveDText(false); // D text 비활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.gag, this.transform.position);
                PlayerConstant.isParalysis = false; // 조작 가능하게 변경
                PlayerConstant.isRedemption = true; // 몸을 못돌리는 상태로 변경
                PlayerConstant.isPillowSound = true;
                
                StartCoroutine(GameManager.Instance.player.StayLookAt(neighborHead, 3f)); // TODO: 특정 오브젝트 대상
                yield return new WaitForSeconds(3f); // 대기 
                
                PlayerConstant.isPillowSound = false;
                UIManager.Instance.ActiveOrDeActiveNText(true); // n text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.neighborN, this.transform.position);
                yield return new WaitForSeconds(1.5f); // 대기
                
                GameManager.Instance.SetState(GameState.GameOver); // 게임 오버 상태로 변경 (준비 상태로 초기화)

                yield return new WaitForSeconds(1f); // 대기
                PlayerConstant.isPillowSound = true;
                breathSound.ToggleBreath(); // 숨 참음 해제
                ChangeMarkovState(Wait); // 기믹은 대기 상태로 변경
                PlayerConstant.isRedemption = false; // 몸 돌릴수 있게
                TimeManager.Instance.isGameOver = false;
                UIManager.Instance.ActiveOrDeActiveNText(false); // n text 비활성화
                yield break;
        }
        
        var markovTransitions = chain[state];
        yield return new WaitUntil(() => stateTransitionDecisionValue >= markovTransitions[0].ThresholdRange.y);
        if (statePassByNextValue <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            CurrState = chain.TransitionNextState(CurrState, stateTransitionDecisionValue);
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
                        GameManager.Instance.StopDemoCoroutine();
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
                    GameManager.Instance.StopDemoCoroutine();
                    GimmickManager.Instance.ChangeAllMarkovGimmickState(MarkovGimmickData.MarkovGimmickType.Wait);

                    yield break;
                }
        
                yield return null;
            }
        }
        
        IEnumerator ChangeNeighborStateProbability()
        {
            if (state.Equals(Wait) || state.Equals(Near)) yield break;
            
            var stateParams = new Dictionary<MarkovState, (int noiseThreshold, int probabilityChangeValue)>
            {
                { Watch, (30, 2) },
                { Cautious, (40, 3) },
                { Danger, (50, 4) }
            };

            while (true)
            {
                if (stateParams.TryGetValue(state, out var param))
                {
                    if (IsLoud(param.noiseThreshold) ||
                        (ConeCollider.TriggeredObject != null && ConeCollider.TriggeredObject.Equals(neighborHead)))
                    {
                        StateTransitionProbability -= param.probabilityChangeValue;
                    }
                    else if (!IsLoud(param.noiseThreshold) || !ConeCollider.TriggeredObject.Equals(neighborHead) ||
                             !PlayerConstant.isEyeOpen)
                    {
                        StateTransitionProbability += param.probabilityChangeValue;
                    }
                }
                Debug.Log("State Probability " + stateTransitionProbability);

                // 확률 값이 증가하는 시간
                yield return new WaitForSeconds(TimeManager.Instance.CycleInterval);
            }
        }

        bool IsLoud(int noise) => PlayerConstant.noiseLevel >= noise;
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
