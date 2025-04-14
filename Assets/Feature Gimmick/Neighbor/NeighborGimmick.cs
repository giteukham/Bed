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
    private Animator animator;
    
    [Tooltip("다음 상태로 전환하는데 걸리는 시간")]
    [SerializeField]
    private float nextStateDelay = 2f;
    
    private int moveChance = 0;                     // 움직일 확률
    
    [Range(0, 100)]
    private int moveProbability = 0;                // 초기값 0. 최댓값 100. moveChance보다 크면 움직임
    
    [Range(0, 100)]
    private int stateTransitionProbability = 35;    // 초기값 35. 눈을 감거나 오른족을 안 보고 잇으면 -5.
    
    private MarkovChain chain = new MarkovChain();
    private MarkovState currState;
    
    private MarkovState wait        = new MarkovState("Wait");
    private MarkovState watch       = new MarkovState("Watch");
    private MarkovState cautious    = new MarkovState("Cautious");
    private MarkovState danger      = new MarkovState("Danger");
    private MarkovState near        = new MarkovState("Near");
    
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
        
        wait.OnStateAction += ActiveStateCoroutine;
        watch.OnStateAction += ActiveStateCoroutine;
        cautious.OnStateAction += ActiveStateCoroutine;
        danger.OnStateAction += ActiveStateCoroutine;
        near.OnStateAction += ActiveStateCoroutine;

        chain.InsertTransition(wait,
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = watch, ThresholdRange       = new Vector2(0, 30) },
                new MarkovTransition { Target = cautious, ThresholdRange    = new Vector2 (31, 60) },
                new MarkovTransition { Target = danger, ThresholdRange      = new Vector2 (61, 80) },
                new MarkovTransition { Target = near, ThresholdRange        = new Vector2 (81, 100) }
            }
        );
        
        chain.InsertTransition(watch, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, ThresholdRange        = new Vector2(0, 10) },
                new MarkovTransition { Target = cautious, ThresholdRange    = new Vector2(11, 45) },
                new MarkovTransition { Target = danger, ThresholdRange      = new Vector2(46, 70) },
                new MarkovTransition { Target = near, ThresholdRange        = new Vector2(71, 100) }
            }
        );
        
        chain.InsertTransition(cautious, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, ThresholdRange        = new Vector2(0, 10) },
                new MarkovTransition { Target = watch, ThresholdRange       = new Vector2(11, 15) },
                new MarkovTransition { Target = danger, ThresholdRange      = new Vector2(16, 40) },
                new MarkovTransition { Target = near, ThresholdRange        = new Vector2(41, 100) }
            }
        );
        
        chain.InsertTransition(danger, 
            new List<MarkovTransition>()
            {
                new MarkovTransition { Target = wait, ThresholdRange        = new Vector2(0, 5) },
                new MarkovTransition { Target = watch, ThresholdRange       = new Vector2(6, 10) },
                new MarkovTransition { Target = cautious, ThresholdRange    = new Vector2(11, 15) },
                new MarkovTransition { Target = near, ThresholdRange        = new Vector2(16, 100) }
            }
        );

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
        moveProbability = PlayerConstant.noiseStage * -10;
        probability = 0 < moveProbability ? 100 : 0;
        if (Mathf.Approximately(probability, 0) && !currState.Equals(wait)) ChangeMarkovState(wait);
        // 임시 값 반영
        tmpDecision = tmpValue;
        tmpValue = 0;
    }

    private void ChangeMarkovState(MarkovState next)
    {
        currState = next;
        currState.Active();
    }
    
    private void ActiveStateCoroutine(MarkovState state)
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
                if(houseLight.activeSelf) houseLight.SetActive(false);
                if(hand.activeSelf)       hand.SetActive(false);
                Deactivate();
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
                GameManager.Instance.SetState(GameState.GameOver);
                yield return new WaitForSeconds(3.5f);
                ChangeMarkovState(wait);
                yield break;
            default:
                break;
        }

        var markovTransitions = chain[state];

        yield return new WaitUntil(() => tmpDecision >= markovTransitions[0].ThresholdRange.y);
        yield return new WaitForSeconds(nextStateDelay);
        
        if (stateTransitionProbability <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            currState = chain.TransitionNextState(currState, tmpDecision);
        }
        else
        {
            currState = chain.TransitionNextState(currState);
        }
        
        Debug.Log("Next State: " + currState.Name);
    }

    private void WindowOpenCloseSoundPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.windowOpenClose, this.transform.position);
    }
    private void RustleSoundPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.pantRustle, this.transform.position);
    }
    private void GagSoundPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.gag, this.transform.position);
    }
    private void HornyBreathSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.hornyBreath, this.transform.position);
    }

    /// <summary>
    /// 게임 오버 시 이웃 쳐다보기
    /// </summary>
    /// <param name="player"></param>
    private void ReachedGameOver(Player player)
    {
        // StartCoroutine(player.LookAt(neighbor.transform.position));
    }

    public override void Initialize()
    {
        if(houseLight.activeSelf) houseLight.SetActive(false);
        if(hand.activeSelf)       hand.SetActive(false);
        if (!currState.Equals(wait)) ChangeMarkovState(wait);
    }
}
