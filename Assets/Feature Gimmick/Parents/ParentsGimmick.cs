using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParentsGimmick : Gimmick, IMarkovGimmick
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
    
    private int moveChance = 0;                     // ������ Ȯ��
    
    [Range(0, 100)]
    private int moveProbability = 0;                // �ʱⰪ 0. �ִ� 100. moveChance���� ũ�� ������
    
    [Range(0, 100)]
    private int stateTransitionProbability = 35;    // �ʱⰪ 35. ���� ���ų� �������� �� ���� ������ -5.
    
    private MarkovChain chain = new MarkovChain();
    public MarkovState CurrState { get; set; } = null;
    
    private List<MarkovState> statesWithoutNear = new List<MarkovState>();
    public MarkovState Wait { get; set; }       = new MarkovState("Wait");
    public MarkovState Watch { get; set; }      = new MarkovState("Watch");
    public MarkovState Cautious { get; set; }   = null;
    public MarkovState Danger { get; set; }     = new MarkovState("Danger");
    public MarkovState Near { get; set; }       = new MarkovState("Near");

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

        Wait.OnStateAction += StartMarkovStateCoroutine;
        Watch.OnStateAction += StartMarkovStateCoroutine;
        Danger.OnStateAction += StartMarkovStateCoroutine;
        Near.OnStateAction += StartMarkovStateCoroutine;
        
        chain.InsertTransition(Wait,
            new List<MarkovTransition>()
            {
                // TODO: Ȯ�� ����
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
        ChangeMarkovState(Watch);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void UpdateProbability()
    {
        moveProbability = PlayerConstant.noiseStage * 10;
        probability = 0 < moveProbability ? 100 : 0;
        if (Mathf.Approximately(probability, 0) && !CurrState.Equals(Wait)) ChangeMarkovState(Wait);

        //if (currState.Equals(watch) && Ư�� �ൿ) ChangeMarkovState(near); 
        
        // �ӽ� �� �ݿ�
        tmpDecision = tmpValue;
        tmpValue = 0;
    }

    public override void Initialize()
    {
        if (hand.activeSelf) hand.SetActive(false);
        if (!CurrState.Equals(Wait)) ChangeMarkovState(Wait);
    }

    public void ChangeRandomMarkovState()
    {
        var randomState = statesWithoutNear[Random.Range(0, statesWithoutNear.Count)];
        ChangeMarkovState(randomState);
    }
    
    public void ChangeMarkovState(MarkovState next)
    {
        CurrState = next;
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
            case MarkovGimmickData.MarkovGimmickType.Danger:
                ChangeMarkovState(Danger);
                break;
            default:
                Debug.Log("Invalid MarkovGimmickData.MarkovGimmickType type");
                break;
        }
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
            case var _ when state.Equals(Wait):
                if (hand.activeSelf) hand.SetActive(false);
                PlayAnimationWithoutDuplication(Wait.Name);
                Deactivate();
                break;
            case var _ when state.Equals(Watch):
                if (hand.activeSelf) hand.SetActive(false);
                PlayRandomChildAnimation(Watch.Name, 4);
                break;
            case var _ when state.Equals(Danger):
                if (hand.activeSelf) hand.SetActive(false);
                PlayAnimationWithoutDuplication(Danger.Name);
                break;
            case var _ when state.Equals(Near):
                if (hand.activeSelf) hand.SetActive(false);
                
                GimmickManager.Instance.DeactivateGimmicks(this);
                PlayAnimationWithoutDuplication(Danger.Name);
                
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
                StartCoroutine(GameManager.Instance.player.LookAt(dadHead, 0.5f)); // TODO: Ư�� ������Ʈ ���
                
                PlayerConstant.isParalysis = true;
                yield return new WaitForSeconds(0.5f); // ���
                
                breathSound.ToggleBreath(); // �� ����
                yield return new WaitForSeconds(2.5f); // ���
                
                UIManager.Instance.SetGameOverScreen(name);
                UIManager.Instance.ActiveOrDeActiveDText(true); // D text Ȱ��ȭ
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.parentsD, this.transform.position); // �÷��̾� ���� ������ ���� ���°� �ƴ϶�� ������ ���� ���� (�Ҹ� �ȵ鸮��)
                GameManager.Instance.player.DirectionControlNoSound(PlayerDirectionStateTypes.Middle);
                yield return new WaitForSeconds(2.5f); // ���
                
                UIManager.Instance.ActiveOrDeActiveDText(false); // D text ��Ȱ��ȭ
                PlayerConstant.isParalysis = false;
                PlayerConstant.isRedemption = true;
                PlayAnimationWithoutDuplication(Near.Name);
                if (!hand.activeSelf) hand.SetActive(true); // �� Ȱ��ȭ
                StartCoroutine(GameManager.Instance.player.LookAt(dadHead, 0.5f)); // TODO: Ư�� ������Ʈ ���
                yield return new WaitForSeconds(3f); // ��� 
                
                UIManager.Instance.ActiveOrDeActiveNText(true); // n text Ȱ��ȭ
                AudioManager.Instance.PlayOneShot(AudioManager.Instance.parentsN, this.transform.position);
                yield return new WaitForSeconds(2.5f); // ���
                
                GameManager.Instance.SetState(GameState.GameOver); // ���� ���� ���·� ���� (�غ� ���·� �ʱ�ȭ)
                breathSound.ToggleBreath(); // �� ���� ����
                yield return new WaitForSeconds(1f); // ���
                
                PlayerConstant.isRedemption = false;
                UIManager.Instance.ActiveOrDeActiveNText(false); // n text ��Ȱ��ȭ
                
                ChangeMarkovState(Wait); // ����� ��� ���·� �����
                yield break;
        }

        var markovTransitions = chain[state];

        yield return new WaitUntil(() => tmpDecision >= markovTransitions[0].ThresholdRange.y);
        
        if (stateTransitionProbability <= Random.Range(0, 50))                      // true�� ���� ���� false�� �� ���� ����
        {
            CurrState = chain.TransitionNextState(CurrState, tmpDecision);
        }
        else
        {
            CurrState = chain.TransitionNextState(CurrState);
        }
        
        Debug.Log("Next State: " + CurrState.Name + " Active Count : " + CurrState.ActiveCount);
    }
    
    private void PlayAnimationWithoutDuplication(string animName)
    {
        // �� �ִϸ��̼��� �� �ִϸ��̼��̶� ���� ���� ���� �ִϸ��̼� ���
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
