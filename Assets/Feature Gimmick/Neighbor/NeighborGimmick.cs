using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using AbstractGimmick;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

public class NeighborGimmick : Gimmick
{
    enum NeighborState
    {
        Wait,
        Watch,
        Cautious,
        Danger,
        Near
    }
    
    #region Override Variables
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
    [SerializeField] private bool zeroPahse, onePhase, twoPhase, threePhase, fourPhase = false;
    private Animator animator;
    
    private int moveChance = 0;                     // 움직일 확률
    
    [Range(0, 100)]
    private int moveProbability = 0;                // 초기값 0. 최댓값 100. moveChance보다 크면 움직임
    
    private int initialStateProbability = 0;        // 처음 기믹이 실행될 때 정해지는 값
    
    [Range(0, 100)]
    private int stateTransitionProbability = 35;    // 초기값 35. 눈을 감거나 오른족을 안 보고 잇으면 -5.
    
    private MarkovChain chain = new MarkovChain();
    private MarkovState currState;
    
    private MarkovState wait        = new MarkovState {name = "Wait"};
    private MarkovState watch       = new MarkovState {name = "Watch"};
    private MarkovState cautious    = new MarkovState {name = "Cautious"};
    private MarkovState danger      = new MarkovState {name = "Danger"};
    private MarkovState near        = new MarkovState {name = "Near"};
    #endregion

    private int tmpValue = 0;
    private int tmpDecision = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            tmpValue += 5;
            Debug.Log(tmpValue);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            tmpValue -= 5;
            Debug.Log(tmpValue);
        }

        if (Input.GetMouseButtonDown(0))
        {
            tmpDecision = tmpValue;
            tmpValue = 0;
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(true);
        
        wait.OnStateAction += ActiveStateCoroutine;
        watch.OnStateAction += ActiveStateCoroutine;
        cautious.OnStateAction += ActiveStateCoroutine;
        danger.OnStateAction += ActiveStateCoroutine;
        near.OnStateAction += ActiveStateCoroutine;
        
        chain.InsertTransition(wait,                        // 현 상태
            new[] { watch, cautious, danger, near },  // 다음 상태
            new[] { 30, 60, 80, 95 }          // 각 상태의 확률
        );
        
        chain.InsertTransition(watch, 
            new[] { wait, cautious, danger, near }, 
            new[] { 10, 45, 70, 95 }
        );
        
        chain.InsertTransition(cautious, 
            new[] { wait, watch, danger, near }, 
            new[] { 10, 15, 40, 75 }
        );
        
        chain.InsertTransition(danger, 
            new[] { wait, watch, cautious, near }, 
            new[] { 5, 10, 15, 50 }
        );
        
        currState = watch;
        currState.Active();
    }

    public override void Activate()
    {
        base.Activate();
    }

    public override void Deactivate()
    {
        base.Deactivate();

        wait.OnStateAction -= ActiveStateCoroutine;
        watch.OnStateAction -= ActiveStateCoroutine;
        cautious.OnStateAction -= ActiveStateCoroutine;
        danger.OnStateAction -= ActiveStateCoroutine;
        near.OnStateAction -= ActiveStateCoroutine;
        
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        moveChance = Random.Range(1, 100);
        moveProbability = PlayerConstant.noiseStage * -10;
        
        // moveChance가 moveProbability보다 작으면 실행 안 하고 크면 실행
        probability = moveChance <= moveProbability ? 100 : 0;      
    }
    
    private void ActiveStateCoroutine(MarkovState state)
    {
        StartCoroutine(ActiveMarkovState(state));
    }

    private IEnumerator ActiveMarkovState(MarkovState state)
    {
        if (!currState.Equals(state))
        {
            animator.Play(state.name);
        }

        if (state.Equals(near))
        {
            Deactivate();
            yield break;
        }

        var stateProbabilities = chain[state];

        if (stateTransitionProbability <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            yield return new WaitUntil(() => tmpDecision >= stateProbabilities[0]);
            currState = chain.GetNextState(currState, tmpDecision);
        }
        else
        {
            currState = chain.GetCurrentState(currState);
        }
        Debug.Log("Next State: " + currState.name);
    }

    private void RustleSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.pantRustle, this.transform.position);
    }

    public override void Initialize(){}
}
