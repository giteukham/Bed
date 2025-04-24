using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParentsGimmick : Gimmick
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
    public GameObject hand;
    public GameObject dad;
    public GameObject dadHead;
    private Animator animator;
    
    private int moveChance = 0;                     // 움직일 확률
    
    [Range(0, 100)]
    private int moveProbability = 0;                // 초기값 0. 최댓값 100. moveChance보다 크면 움직임
    
    [Range(0, 100)]
    private int stateTransitionProbability = 35;    // 초기값 35. 눈을 감거나 오른족을 안 보고 잇으면 -5.
    
    private MarkovChain chain = new MarkovChain();
    private MarkovState currState;
    
    private MarkovState wait        = new MarkovState("Wait");
    private MarkovState watch       = new MarkovState("Watch");
    private MarkovState danger      = new MarkovState("Danger");
    private MarkovState near        = new MarkovState("Near");

    private int conditionCountToNear = 3;
    [SerializeField] private BreathSound breathSound;
    
    private Coroutine markovCoroutine;
    #endregion
    
    private int tmpValue = 0;
    private int tmpDecision = 0;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            tmpValue += 5;
            Debug.Log(tmpValue);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            tmpValue -= 5;
            Debug.Log(tmpValue);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && probability == 100)
        {
            ChangeMarkovState(wait);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && probability == 100)
        {
            ChangeMarkovState(watch);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && probability == 100)
        {
            ChangeMarkovState(danger);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && probability == 100)
        {
            ChangeMarkovState(near);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        wait.OnStateAction += StartMarkovStateCoroutine;
        watch.OnStateAction += StartMarkovStateCoroutine;
        danger.OnStateAction += StartMarkovStateCoroutine;
        near.OnStateAction += StartMarkovStateCoroutine;
        
        chain.InsertTransition(wait,
            new List<MarkovTransition>()
            {
                // TODO: 확률 조정
                new MarkovTransition { Target = watch, ThresholdRange   = new Vector2(0, 10)},
                new MarkovTransition { Target = danger, ThresholdRange  = new Vector2(11, 20)},
                new MarkovTransition { Target = near, ThresholdRange    = new Vector2(21, 30)},
            });
        
        chain.InsertTransition(watch,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, ThresholdRange    = new Vector2(0, 10)},
                new MarkovTransition { Target = danger, ThresholdRange  = new Vector2(11, 20)},
                new MarkovTransition { Target = near, ThresholdRange    = new Vector2(21, 30)},
            });
        
        chain.InsertTransition(danger,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, ThresholdRange    = new Vector2(0, 10)},
                new MarkovTransition { Target = watch, ThresholdRange   = new Vector2(11, 20)},
                new MarkovTransition { Target = near, ThresholdRange    = new Vector2(21, 30)},
            });
        
        ChangeMarkovState(wait);
    }

    public override void Activate()
    {
        base.Activate();
        ChangeMarkovState(watch);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void UpdateProbability()
    {
        moveProbability = PlayerConstant.noiseStage * 10;
        probability = 0 < moveProbability ? 100 : 0;
        if (Mathf.Approximately(probability, 0) && !currState.Equals(wait)) ChangeMarkovState(wait);

        //if (currState.Equals(watch) && 특정 행동) ChangeMarkovState(near); 
        
        // 임시 값 반영
        tmpDecision = tmpValue;
        tmpValue = 0;
    }

    public override void Initialize()
    {
        if (hand.activeSelf) hand.SetActive(false);
        if (!currState.Equals(wait)) ChangeMarkovState(wait);
    }
    
    private void ChangeMarkovState(MarkovState next)
    {
        currState = next;
        currState.Active();
    }
    
    private void StartMarkovStateCoroutine(MarkovState state)
    {
        if (markovCoroutine != null) StopCoroutine(markovCoroutine);
        tmpDecision = 0;
        markovCoroutine = StartCoroutine(ActiveMarkovState(state));
    }

    private IEnumerator ActiveMarkovState(MarkovState state)
    {
        switch (state)
        {
            case var _ when state.Equals(wait):
                if (hand.activeSelf) hand.SetActive(false);
                PlayAnimationWithoutDuplication(wait.Name);
                Deactivate();
                break;
            case var _ when state.Equals(watch):
                if (hand.activeSelf) hand.SetActive(false);
                PlayRandomChildAnimation(watch.Name, 4);
                break;
            case var _ when state.Equals(danger):
                if (hand.activeSelf) hand.SetActive(false);
                PlayAnimationWithoutDuplication(danger.Name);
                break;
            case var _ when state.Equals(near):
                if (hand.activeSelf) hand.SetActive(false);
                
                GimmickManager.Instance.DeactivateGimmicks(this);
                PlayAnimationWithoutDuplication(danger.Name);
                
                // PauseTime();
                var timer = 0f;
                var sequence = DOTween.Sequence();
                sequence.Append(DOTween.To(() => timer, x => timer = x, 10f, 10f))
                    .OnUpdate(() =>
                    {
                        if (PlayerConstant.isMiddleState) sequence.Kill();
                    })
                    .OnComplete(() =>
                    {
                        GameManager.Instance.player.DirectionControl(PlayerDirectionStateTypes.Middle);
                    });
                
                yield return new DOTweenCYInstruction.WaitForCompletion(sequence);

                yield return new WaitUntil(() => (PlayerConstant.isMiddleState));
                StartCoroutine(GameManager.Instance.player.LookAt(dadHead, 0.5f)); // TODO: 특정 오브젝트 대상
                
                PlayerConstant.isParalysis = true;
                yield return new WaitForSeconds(0.5f); // 대기
                
                breathSound.ToggleBreath(); // 숨 참음
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.SetGameOverScreen(name);
                UIManager.Instance.ActiveOrDeActiveDText(true); // D text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.parentsD, this.transform.position); // 플레이어 몸이 정면을 보는 상태가 아니라면 정면을 보게 돌림 (소리 안들리게)
                GameManager.Instance.player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.ActiveOrDeActiveDText(false); // D text 비활성화
                PlayerConstant.isParalysis = false;
                PlayerConstant.isRedemption = true;
                PlayAnimationWithoutDuplication(near.Name);
                if (!hand.activeSelf) hand.SetActive(true); // 손 활성화
                StartCoroutine(GameManager.Instance.player.LookAt(dadHead, 0.5f)); // TODO: 특정 오브젝트 대상
                yield return new WaitForSeconds(3f); // 대기 
                
                UIManager.Instance.ActiveOrDeActiveNText(true); // n text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.parentsN, this.transform.position);
                yield return new WaitForSeconds(2.5f); // 대기
                
                GameManager.Instance.SetState(GameState.GameOver); // 게임 오버 상태로 변경 (준비 상태로 초기화)
                breathSound.ToggleBreath(); // 숨 참음 해제
                yield return new WaitForSeconds(1f); // 대기
                
                PlayerConstant.isRedemption = false;
                UIManager.Instance.ActiveOrDeActiveNText(false); // n text 비활성화
                
                ChangeMarkovState(wait); // 기믹은 대기 상태로 변경경
                yield break;
        }

        var markovTransitions = chain[state];

        yield return new WaitUntil(() => tmpDecision >= markovTransitions[0].ThresholdRange.y);
        
        if (stateTransitionProbability <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            currState = chain.TransitionNextState(currState, tmpDecision);
        }
        else
        {
            currState = chain.TransitionNextState(currState);
        }
        
        Debug.Log("Next State: " + currState.Name + " Active Count : " + currState.ActiveCount);
    }
    
    private void PlayAnimationWithoutDuplication(string animName)
    {
        // 현 애니메이션이 전 애니메이션이랑 같지 않을 때만 애니메이션 재생
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            animator.SetTrigger(animName);
        }
    }

    private void PlayRandomChildAnimation(string stateName, int ranCount)
    {
        animator.SetTrigger($"{stateName}{Random.Range(1, ranCount)}");
    }
}
