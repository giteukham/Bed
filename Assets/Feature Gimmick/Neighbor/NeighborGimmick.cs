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
    
    private MarkovState wait        = new MarkovState {Name = "Wait"};
    private MarkovState watch       = new MarkovState {Name = "Watch"};
    private MarkovState cautious    = new MarkovState {Name = "Cautious"};
    private MarkovState danger      = new MarkovState {Name = "Danger"};
    private MarkovState near        = new MarkovState {Name = "Near"};
    
    private Coroutine markovCoroutine;
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
        
        chain.InsertTransition(wait,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = watch, Threshold = 30 },
                new MarkovTransition { Target = cautious, Threshold = 60 },
                new MarkovTransition { Target = danger, Threshold = 80 },
                new MarkovTransition { Target = near, Threshold = 95 }
            }
        );
        
        chain.InsertTransition(watch, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, Threshold = 10 },
                new MarkovTransition { Target = cautious, Threshold = 45 },
                new MarkovTransition { Target = danger, Threshold = 70 },
                new MarkovTransition { Target = near, Threshold = 95 }
            }
        );
        
        chain.InsertTransition(cautious, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, Threshold = 10 },
                new MarkovTransition { Target = watch, Threshold = 15 },
                new MarkovTransition { Target = danger, Threshold = 40 },
                new MarkovTransition { Target = near, Threshold = 75 }
            }
        );
        
        chain.InsertTransition(danger, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, Threshold = 5 },
                new MarkovTransition { Target = watch, Threshold = 10 },
                new MarkovTransition { Target = cautious, Threshold = 15 },
                new MarkovTransition { Target = near, Threshold = 50 }
            }
        );
    }

    public override void Activate()
    {
        base.Activate();
        wait.OnStateAction += ActiveStateCoroutine;
        watch.OnStateAction += ActiveStateCoroutine;
        cautious.OnStateAction += ActiveStateCoroutine;
        danger.OnStateAction += ActiveStateCoroutine;
        near.OnStateAction += ActiveStateCoroutine;
        
        currState = watch;
        currState.Active();
    }

    public override void Deactivate()
    {
        base.Deactivate();

        wait.OnStateAction -= ActiveStateCoroutine;
        watch.OnStateAction -= ActiveStateCoroutine;
        cautious.OnStateAction -= ActiveStateCoroutine;
        danger.OnStateAction -= ActiveStateCoroutine;
        near.OnStateAction -= ActiveStateCoroutine;
        
        currState = watch;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        // moveChance = Random.Range(1, 100);
        moveProbability = PlayerConstant.noiseStage * -10;
        
        // moveChance가 moveProbability보다 작으면 실행 안 하고 크면 실행
        probability = 0 < moveProbability ? 100 : 0;      
    }
    
    private void ActiveStateCoroutine(MarkovState state)
    {
        if (markovCoroutine != null) StopCoroutine(markovCoroutine);
        tmpDecision = 0;
        markovCoroutine = StartCoroutine(ActiveMarkovState(state));
    }

    private IEnumerator ActiveMarkovState(MarkovState state)
    {
        if (!currState.Equals(state))
        {
            animator.Play(state.Name);
        }

        switch (state)
        {
            case var _ when state.Equals(wait):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                break;
            case var _ when state.Equals(watch):
                if(!houseLight.activeSelf)houseLight.SetActive(true);
                if(hand.activeSelf)       hand.SetActive(false);
                break;
            case var _ when state.Equals(cautious):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                break;
            case var _ when state.Equals(danger):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                break;
            case var _ when state.Equals(near):
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(!hand.activeSelf)      hand.SetActive(true);
                Deactivate();
                yield break;
            default:
                break;
        }

        // if (state.Equals(near))
        // {
        //     Deactivate();
        //     yield break;
        // }

        var markovTransitions = chain[state];

        yield return new WaitUntil(() => tmpDecision >= markovTransitions[0].Threshold);
        
        if (stateTransitionProbability <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            currState = chain.TransitionNextState(currState, tmpDecision);
        }
        else
        {
            currState = chain.TransitionCurrentState(currState);
        }
        
        Debug.Log("Next State: " + currState.Name);
    }

    private void RustleSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.pantRustle, this.transform.position);
    }

    public override void Initialize(){}
}
