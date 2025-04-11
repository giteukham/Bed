using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeMarkovState(wait);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeMarkovState(watch);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeMarkovState(danger);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeMarkovState(near);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        GameManager.Instance.OnGameOverEvent += ReachedGameOver;

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
        animator.Play(state.Name);
        
        switch (state)
        {
            case var _ when state.Equals(wait):
                if(hand.activeSelf)       hand.SetActive(false);
                Deactivate();
                break;
            case var _ when state.Equals(watch):
                if(hand.activeSelf)       hand.SetActive(false);
                PlayRandomChildAnimation(watch.Name, 4);
                break;
            case var _ when state.Equals(danger):
                if(hand.activeSelf)       hand.SetActive(false);
                break;
            case var _ when state.Equals(near):
                if(!hand.activeSelf)      hand.SetActive(true);
                GameManager.Instance.SetState(GameState.GameOver);
                yield return new WaitForSeconds(3.5f);
                ChangeMarkovState(wait);
                yield break;
            default:
                break;
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

    private void PlayRandomChildAnimation(string stateName, int ranCount)
    {
        animator.SetTrigger($"{stateName}{Random.Range(1, ranCount)}");
    }
    
    /// <summary>
    /// 게임 오버 시 Dad 쳐다 보기
    /// </summary>
    /// <param name="player"></param>
    private void ReachedGameOver(Player player)
    {
        StartCoroutine(player.LookAt(dad.transform.position));
    }
}
