using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using AbstractGimmick;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.Playables;
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
    public GameObject neighborHead;
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
    private MarkovState cautious    = new MarkovState("Cautious");
    private MarkovState danger      = new MarkovState("Danger");
    private MarkovState near        = new MarkovState("Near");
    
    private Coroutine markovCoroutine;
    [SerializeField] private BreathSound breathSound;
    #endregion

    private int tmpValue = 0;
    private int tmpDecision = 0;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            tmpValue += 15;
            Debug.Log(tmpValue);
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            tmpValue -= 15;
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
        switch (state)
        {
            case var _ when state.Equals(wait):
            case var _ when state.Equals(watch):
            case var _ when state.Equals(cautious):
            case var _ when state.Equals(danger):
                PlayAnimationWithoutStateDuplication(state);
                break;
                
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
                if(!hand.activeSelf)      hand.SetActive(false);
                
                GimmickManager.Instance.DeactivateGimmicks(this);
                PlayAnimationWithoutStateDuplication(danger);
                
                TimeManager.Instance.isGameOver = true;
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

                yield return new WaitUntil(() => PlayerConstant.isMiddleState);
                StartCoroutine(GameManager.Instance.player.LookAt(neighborHead, 0.1f)); // TODO: 특정 오브젝트 대상

                PlayerConstant.isParalysis = true; // 조작이 불가능한 상태로 변경
                yield return new WaitForSeconds(0.2f);  // 대기
                
                breathSound.ToggleBreath(); // 숨 참음
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.SetGameOverScreen(name); 
                UIManager.Instance.ActiveOrDeActiveDText(true); // D text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.neighborD, this.transform.position);
                GameManager.Instance.player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
                yield return new WaitForSeconds(2.5f); // 대기
                
                UIManager.Instance.ActiveOrDeActiveDText(false); // D text 비활성화
                PlayerConstant.isParalysis = false; // 조작 가능하게 변경
                PlayerConstant.isRedemption = true; // 몸을 못돌리는 상태로 변경
                PlayAnimationWithoutStateDuplication(near);
                StartCoroutine(GameManager.Instance.player.LookAt(neighborHead, 0.1f)); // TODO: 특정 오브젝트 대상
                if(!hand.activeSelf) hand.SetActive(true); // 손 활성화
                
                yield return new WaitForSeconds(3f); // 대기 
                
                UIManager.Instance.ActiveOrDeActiveNText(true); // n text 활성화
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.neighborN, this.transform.position);
                yield return new WaitForSeconds(1.5f); // 대기
                
                GameManager.Instance.SetState(GameState.GameOver); // 게임 오버 상태로 변경 (준비 상태로 초기화)

                yield return new WaitForSeconds(1f); // 대기
                breathSound.ToggleBreath(); // 숨 참음 해제
                ChangeMarkovState(wait); // 기믹은 대기 상태로 변경
                PlayerConstant.isRedemption = false; // 몸 돌릴수 있게
                TimeManager.Instance.isGameOver = false;
                UIManager.Instance.ActiveOrDeActiveNText(false); // n text 비활성화
                yield break;
        }

        var markovTransitions = chain[state];

        yield return new WaitUntil(() => tmpDecision >= markovTransitions[0].ThresholdRange.y);
        
        if (stateTransitionProbability <= Random.Range(0, 50))                      // true면 다음 상태 false면 현 상태 유지
        {
            currState = chain.TransitionNextState(currState, tmpDecision);
            Debug.Log("Next State: " + currState.Name);
        }
        else
        {
            currState = chain.TransitionNextState(currState);
            Debug.Log("Next State: X");
        }
    }

    private void PlayAnimationWithoutStateDuplication(MarkovState state)
    {
        // 현 상태가 전 상태랑 같지 않을 때만 애니메이션 재생
        if (!currState.Equals(state))
        {
            animator.SetTrigger(state.Name);
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
    private void GagSoundPlay()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.gag, this.transform.position);
    }
    private void HornyBreathSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.hornyBreath, this.transform.position);
    }

    public override void Initialize()
    {
        if(houseLight.activeSelf) houseLight.SetActive(false);
        if(hand.activeSelf)       hand.SetActive(false);
        if (!currState.Equals(wait)) ChangeMarkovState(wait);
    }
}
