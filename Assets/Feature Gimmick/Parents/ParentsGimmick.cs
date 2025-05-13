using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using Bed.Collider;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParentsGimmick : MarkovGimmick
{
    #region Override Variables
    public string GimmickName { get; protected set; } = "Parents";
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
    public GameObject hand, dadHead, momHead;
    public Transform dadPosition, momHeadPosition, dadWalkSoundPosition, dadWalkStartPosition, dadWalkEndPosition;
    private bool isDadAngry = false;
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
    
    protected override bool IsOn { get; set; } = false;
    
    private List<MarkovState> statesWithoutNear = new List<MarkovState>();
    public override MarkovState Wait { get; set; }       = new MarkovState("Wait");
    public override MarkovState Watch { get; set; }      = new MarkovState("Watch");
    public override MarkovState Cautious { get; set; }   = null;
    public override MarkovState Danger { get; set; }     = new MarkovState("Danger");
    public override MarkovState Near { get; set; }       = new MarkovState("Near");

    private int conditionCountToNear = 3;
    [SerializeField] private BreathSound breathSound;
    
    [Header("설정")]
    
    [SerializeField]
    [Tooltip("특정 상황에 변화하는 상태 확률값")]
    private List<StateProbabilityData> stateProbabilities = new();

    [SerializeField]
    [Tooltip("눈 쳐다볼 때 감소하는 상태 확률. Threshold는 안 씀.")]
    private List<StateProbabilityData> stateProbabilitiesLookEye = new();
    
    private Coroutine dadWalkSoundSetPositionCoroutine = null, checkHeadCollisionCoroutine = null;
    private Tween moveTween;
    #endregion
    
    private void Update()
    {
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
        statesWithoutNear.Add(Danger);

        Wait.OnStateAction += ActiveStateCoroutine;
        Watch.OnStateAction += ActiveStateCoroutine;
        Danger.OnStateAction += ActiveStateCoroutine;
        Near.OnStateAction += ActiveStateCoroutine;
        
        chain.InsertTransition(Wait,
            new List<MarkovTransition>()
            {
                // TODO: 확률 조정
                new MarkovTransition { Target = Watch, ThresholdRange   = new Vector2(0, 10)},
                new MarkovTransition { Target = Danger, ThresholdRange  = new Vector2(11, 20)},
                new MarkovTransition { Target = Near, ThresholdRange    = new Vector2(21, 30)},
            });
        
        chain.InsertTransition(Watch,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = Wait, ThresholdRange    = new Vector2(0, 10)},
                new MarkovTransition { Target = Danger, ThresholdRange  = new Vector2(11, 20)},
                new MarkovTransition { Target = Near, ThresholdRange    = new Vector2(21, 30)},
            });
        
        chain.InsertTransition(Danger,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = Wait, ThresholdRange    = new Vector2(0, 10)},
                new MarkovTransition { Target = Watch, ThresholdRange   = new Vector2(11, 20)},
                new MarkovTransition { Target = Near, ThresholdRange    = new Vector2(21, 30)},
            });
        
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
        moveProbability = PlayerConstant.noiseStage * 10;
        probability = 0 < moveProbability ? 100 : 0;
        if (Mathf.Approximately(probability, 0) && !CurrState.Equals(Wait)) ChangeMarkovState(Wait);

        // 특정 상황에 상태 확률을 decision으로 지정
        stateTransitionDecisionValue = stateTransitionProbability;

        if (GameManager.Instance.isDemo == false && IsOn == true)
        {
            ChangeStateProbability();
            ChangeStateProbabilitySeeingParents(Watch);
            Debug.Log("Parents State Probability " + stateTransitionProbability);
        }
    }

    public override void ChangeRandomMarkovState()
    {
        var randomState = statesWithoutNear[Random.Range(0, statesWithoutNear.Count)];
        ChangeMarkovState(randomState);
    }
    
    public override void ChangeMarkovState(MarkovState next)
    {
        CurrState = next;
        if (checkHeadCollisionCoroutine != null) 
        {
            StopCoroutine(checkHeadCollisionCoroutine);
            checkHeadCollisionCoroutine = null;
        }
        CurrState.ActiveCount++;
        CurrState.Active();
    }
    
    public override void ChangeMarkovState(MarkovGimmickData.MarkovGimmickType type)
    {
        switch (type)
        {
            case MarkovGimmickData.MarkovGimmickType.Wait:
                ChangeMarkovState(Wait);
                break;
            case MarkovGimmickData.MarkovGimmickType.Watch:
                ChangeMarkovState(Watch);
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
                AudioManager.Instance.StopSound(AudioManager.Instance.momBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                AudioManager.Instance.StopSound(AudioManager.Instance.dadBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                if (hand.activeSelf) hand.SetActive(false);
                PlayAnimationWithoutDuplication(state.Name);
                if (Door.GetAngle() > 0) Door.Close(0, 0.5f);
                Deactivate();
                break;
                
            case var _ when state.Equals(Watch):
                if (hand.activeSelf) hand.SetActive(false);
                AudioManager.Instance.StopSound(AudioManager.Instance.dadBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                PlayRandomChildAnimation(state.Name, 3);
                Door.Open(20, 0.6f);
                checkHeadCollisionCoroutine ??= StartCoroutine(CheckHeadCollision(momHead, (isCollided) =>
                {
                    ChangeMarkovState(Danger);
                }));
                break;

            case var _ when state.Equals(Danger):
                if(Danger.ActiveCount >= 4) ChangeMarkovState(Near);

                AudioManager.Instance.StopSound(AudioManager.Instance.momBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                AudioManager.Instance.StopSound(AudioManager.Instance.dadBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                if (hand.activeSelf) hand.SetActive(false);
                
                PlayAnimationWithoutDuplication(Wait.Name);
                if (Door.GetAngle() > 0) Door.Close(0, 0.4f);
                yield return new WaitForSeconds(0.6f); 

                dadWalkSoundPosition.position = dadWalkStartPosition.position;

                float duration = 5f / (isDadAngry ? 1.5f : 1f);
                AudioManager.Instance.PlaySound(AudioManager.Instance.dadWalk, dadWalkSoundPosition.position);

                if(Danger.ActiveCount >= 3) 
                { 
                    isDadAngry = true; 
                    AudioManager.Instance.SetParameter(AudioManager.Instance.dadWalk, "DadAnger", 1f); 
                }
                else 
                { 
                    isDadAngry = false; 
                    AudioManager.Instance.SetParameter(AudioManager.Instance.dadWalk, "DadAnger", 0f); 
                }
                if (moveTween != null && moveTween.IsActive()) moveTween.Kill();

                dadWalkSoundSetPositionCoroutine = StartCoroutine(dadWalkSoundSetPosition());
                moveTween = dadWalkSoundPosition.DOMove(dadWalkEndPosition.position, duration)
                                        .SetEase(Ease.InOutSine)
                                        .OnComplete(() =>
                                        {
                                            if(dadWalkSoundSetPositionCoroutine != null) StopCoroutine(dadWalkSoundSetPositionCoroutine);
                                            AudioManager.Instance.StopSound(AudioManager.Instance.dadWalk, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                                            dadWalkSoundPosition.position = dadWalkStartPosition.position;
                                            Door.Set(80, 0.3f);
                                        });
                yield return new DOTweenCYInstruction.WaitForCompletion(moveTween);
                yield return new WaitForSeconds(0.4f);
                PlayAnimationWithoutDuplication(Danger.Name);
                checkHeadCollisionCoroutine ??= StartCoroutine(CheckHeadCollision(dadHead, (isCollided) =>
                {
                    ChangeMarkovState(Near);
                }));
                break;

            case var _ when state.Equals(Near):
                GameManager.Instance.StopDemoCoroutine();
                AudioManager.Instance.StopSound(AudioManager.Instance.momBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                AudioManager.Instance.StopSound(AudioManager.Instance.dadBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                if (hand.activeSelf) hand.SetActive(false);
                
                GimmickManager.Instance.DeactivateGimmicks(this);
                if(GetPlayAnimationName() != "Danger")
                {
                    PlayAnimationWithoutDuplication(Wait.Name);
                    if (Door.GetAngle() > 0) Door.Close(0, 0.4f);
                    yield return new WaitForSeconds(0.6f); 
                    isDadAngry = true; 
                    dadWalkSoundPosition.position = dadWalkStartPosition.position;
                    AudioManager.Instance.PlaySound(AudioManager.Instance.dadWalk, dadWalkSoundPosition.position);
                    AudioManager.Instance.SetParameter(AudioManager.Instance.dadWalk, "DadAnger", 1f); 
                    if (moveTween != null && moveTween.IsActive()) moveTween.Kill();

                    dadWalkSoundSetPositionCoroutine = StartCoroutine(dadWalkSoundSetPosition());
                    moveTween = dadWalkSoundPosition.DOMove(dadWalkEndPosition.position, 3f)
                                            .SetEase(Ease.InOutSine)
                                            .OnComplete(() =>
                                            {
                                                if(dadWalkSoundSetPositionCoroutine != null) StopCoroutine(dadWalkSoundSetPositionCoroutine);
                                                AudioManager.Instance.StopSound(AudioManager.Instance.dadWalk, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                                                dadWalkSoundPosition.position = dadWalkStartPosition.position;
                                                Door.Set(110f, 0.3f);
                                            });
                    yield return new DOTweenCYInstruction.WaitForCompletion(moveTween);
                    yield return new WaitForSeconds(0.4f);
                    PlayAnimationWithoutDuplication(Danger.Name);
                }
                
                TimeManager.Instance.isGameOver = true;
                var timer = 0f;
                var sequence = DOTween.Sequence();
                sequence.Append(DOTween.To(() => timer, x => timer = x, 10f, 10f))
                    .OnUpdate(() =>
                    {
                        if (!PlayerConstant.isRightState) sequence.Complete();
                    });
                
                yield return new DOTweenCYInstruction.WaitForCompletion(sequence);

                if (PlayerConstant.isRightState)
                {
                    GameManager.Instance.player.DirectionControl(PlayerDirectionStateTypes.Middle);
                }
                yield return new WaitUntil(() => PlayerConstant.isMiddleState);
                StartCoroutine(GameManager.Instance.player.LookAt(dadHead, 0.2f)); // TODO: Ư�� ������Ʈ ���
                GameManager.Instance.player.ForceOpenEye();
                PlayerConstant.isParalysis = true;
                yield return new WaitForSeconds(0.2f); // 대기
                
                breathSound.ToggleBreath(); // 숨 참음
                yield return new WaitForSeconds(2.5f); // 대기
                AudioManager.Instance.StopSound(AudioManager.Instance.dadBreath, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                
                UIManager.Instance.ControlDText(true, "Parents"); // D text 활성화
                PlayerConstant.isPillowSound = false;
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.parentsD, this.transform.position); // 플레이어 몸이 정면을 보는 상태가 아니라면 정면을 보게 돌림 (소리 안들리게)
                GameManager.Instance.player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
                PlayAnimationWithoutDuplication(Near.Name);
                if (!hand.activeSelf) hand.SetActive(true); // �� Ȱ��ȭ
                yield return new WaitForSeconds(2.5f); // ���
                
                UIManager.Instance.ControlDText(false, "Parents"); // D text 비활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.dadStrangle, this.transform.position);
                PlayerConstant.isParalysis = false;
                PlayerConstant.isRedemption = true;
                PlayerConstant.isPillowSound = true;

                StartCoroutine(GameManager.Instance.player.StayLookAt(dadHead, 3f)); // TODO: Ư�� ������Ʈ ���
                yield return new WaitForSeconds(3f); // ��� 
                
                PlayerConstant.isPillowSound = false;
                UIManager.Instance.ControlNText(true, "Parents"); // n text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.parentsN, this.transform.position);
                yield return new WaitForSeconds(1.5f); // 대기
                
                GameManager.Instance.SetState(GameState.GameOver); // 게임 오버 상태로 변경 (준비 상태로 초기화)
                
                yield return new WaitForSeconds(1f); // 대기
                PlayerConstant.isPillowSound = true;
                breathSound.ToggleBreath(); // 숨 참음 해제
                ChangeMarkovState(Wait); // 기믹은 대기 상태로 변경
                isDadAngry = false; 
                chain.InitStateCount();
                PlayerConstant.isRedemption = false;
                TimeManager.Instance.isGameOver = false;
                UIManager.Instance.ControlNText(false, "Parents"); // n text 비활성화
                yield break;
        }

        yield return new WaitUntil(() => IsOn == true);
        yield return new WaitForSeconds(TimeManager.Instance.CycleInterval);
        if (stateTransitionProbability <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            CurrState = chain.TransitionNextState(CurrState, Mathf.RoundToInt(stateTransitionDecisionValue));
        }
        else
        {
            CurrState = chain.TransitionNextState(CurrState);
        }
        
        Debug.Log("Next State: " + CurrState.Name + " Active Count : " + CurrState.ActiveCount);
        yield break;
        
        IEnumerator CheckHeadCollision(GameObject targetObject, Action<bool> callback)
        {
            bool isCollided = false;
            while (!isCollided)
            {
                if (BlinkEffect.Blink <= 0.85f && 
                    ConeCollider.TriggeredObject != null &&
                    ConeCollider.TriggeredObject.Equals(targetObject))
                {
                    callback(!isCollided);
                    Debug.Log("Collision Detected with " + targetObject.name);
                    isCollided = true;
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
            
            if (BlinkEffect.Blink <= 0.65f && 
                (PlayerConstant.isRightState || PlayerConstant.isRightLook || PlayerConstant.isRightFrontLook))
            {
                probabilityValue *= 2f;
            }
            else if (PlayerConstant.isRightState ||
                PlayerConstant.isRightLook ||
                PlayerConstant.isRightFrontLook ||
                BlinkEffect.Blink <= 0.65f)
            {
                probabilityValue *= 1.5f;
            }
            
            // Noise Level이 높으면 상태 확률 증가
            if (IsLoud(data.noiseThreshold))
            {
                StateTransitionProbability += probabilityValue;
            }
            // Noise Level이 낮으면 상태 확률 감소
            else if (!IsLoud(data.noiseThreshold))
            {
                StateTransitionProbability -= probabilityValue;
            }
        }
    }

    bool IsLoud(int noise) => PlayerConstant.noiseLevel >= noise;
    
    /// <summary>
    /// 이웃을 보고 있으면 매개변수 상태 별로 값이 다르게 다음 상태 확률을 증감하는 코루틴
    /// </summary>
    /// <param name="state"></param>
    private void ChangeStateProbabilitySeeingParents(MarkovState state)
    {
        if (!CurrState.Equals(state)) return;
        
        foreach (var data in stateProbabilitiesLookEye)
        {
            if (data.type != state.Type) continue;

            // 부모를 보고 있으면 상태 확률 감소
            if (ConeCollider.TriggeredObject
                && (ConeCollider.TriggeredObject.Equals(dadHead) || ConeCollider.TriggeredObject.Equals(momHead)))
            {
                StateTransitionProbability -= data.probabilityChangeValue;
            }
            // 부모를 안 보고 있으면 상태 확률 증가
            else if ((ConeCollider.TriggeredObject 
                      && (!ConeCollider.TriggeredObject.Equals(dadHead) || !ConeCollider.TriggeredObject.Equals(momHead))) 
                     || !ConeCollider.TriggeredObject)
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

    private string GetPlayAnimationName()
    {
        var currentState = animator.GetCurrentAnimatorStateInfo(0);
        return currentState.IsName("Wait") ? "Wait" : currentState.IsName("Watch") ? "Watch" : currentState.IsName("Danger") ? "Danger" : "Near";
    }

    private void PlayRandomChildAnimation(string stateName, int ranCount)
    {
        animator.SetTrigger($"{stateName}{Random.Range(0, ranCount) + 1}");
    }

    private void MomBreathSoundPlay()
    {
        if (!AudioManager.Instance.DuplicateCheck(AudioManager.Instance.momBreath)) 
            AudioManager.Instance.PlaySound(AudioManager.Instance.momBreath, momHeadPosition.position);
        AudioManager.Instance.SetPosition(AudioManager.Instance.momBreath, momHeadPosition.position);
    }

    private void DadBreathSoundPlay()
    {
        if (isDadAngry)
        {
            if (!AudioManager.Instance.DuplicateCheck(AudioManager.Instance.dadBreath)) 
            AudioManager.Instance.PlaySound(AudioManager.Instance.dadBreath, dadHead.transform.position);
            AudioManager.Instance.SetPosition(AudioManager.Instance.dadBreath, dadHead.transform.position);
        }
    }

    // private void StrangleSoundPlay()
    // {
    //     AudioManager.Instance.PlayOneShot(AudioManager.Instance.dadStrangle, this.transform.position);
    // }

    private IEnumerator dadWalkSoundSetPosition()
    {
        while (true)
        {
            AudioManager.Instance.SetPosition(AudioManager.Instance.dadWalk, dadWalkSoundPosition.position);
            yield return null;
        }
    }
    
    public override void Initialize()
    {
        if (hand.activeSelf) hand.SetActive(false);
        if (!CurrState.Equals(Wait)) ChangeMarkovState(Wait);
    }
}