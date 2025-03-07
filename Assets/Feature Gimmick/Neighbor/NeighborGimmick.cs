using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;

public class NeighborGimmick : Gimmick
{
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
    #endregion
    enum NeighborState
    {
        Wait,
        Watch,
        Cautious,
        Danger,
        Near
    }
    // private Dictionary<NeighborState, Dictionary<NeighborState, ProbabilityData>> baseTransitionMatrix;
    
    // public class ProbabilityData
    // {
    //     public int BaseProbability;  // 기본 확률
    //     public int Modifier;    // 추가 확률

    //     public ProbabilityData(int baseChance, int modifier = 0)
    //     {
    //         BaseProbability = baseChance;
    //         Modifier = modifier;
    //     }

    //     public void UpdateProbability()
    //     {
            
    //     }
        
    //     public int GetTotalProbability()
    //     {
    //         return Mathf.Clamp(BaseProbability + Modifier, 0, 100);
    //     }
    // }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        // baseTransitionMatrix = new Dictionary<NeighborState, Dictionary<NeighborState, ProbabilityData>>();
        // baseTransitionMatrix[NeighborState.Wait] = new Dictionary<NeighborState, ProbabilityData>
        // {
        //     { NeighborState.Wait, new ProbabilityData(20) },
        //     { NeighborState.Watch, new ProbabilityData(50) },
        //     { NeighborState.Cautious, new ProbabilityData(20) },
        //     { NeighborState.Danger, new ProbabilityData(10) },
        //     { NeighborState.Near, new ProbabilityData(0) }
        // };
        // baseTransitionMatrix[NeighborState.Watch] = new Dictionary<NeighborState, ProbabilityData>
        // {
        //     { NeighborState.Wait,  new ProbabilityData(20) },
        //     { NeighborState.Watch, new ProbabilityData(30) },
        //     { NeighborState.Cautious, new ProbabilityData(40) },
        //     { NeighborState.Danger, new ProbabilityData(10) },
        //     { NeighborState.Near, new ProbabilityData(0) }
        // };
        // baseTransitionMatrix[NeighborState.Cautious] = new Dictionary<NeighborState, ProbabilityData>
        // {
        //     { NeighborState.Wait, new ProbabilityData(20) },
        //     { NeighborState.Watch, new ProbabilityData(25) },
        //     { NeighborState.Cautious, new ProbabilityData(35) },
        //     { NeighborState.Danger, new ProbabilityData(15) },
        //     { NeighborState.Near, new ProbabilityData(5) }
        // };
        // baseTransitionMatrix[NeighborState.Danger] = new Dictionary<NeighborState, ProbabilityData>
        // {
        //     { NeighborState.Wait, new ProbabilityData(20) },
        //     { NeighborState.Watch, new ProbabilityData(5) },
        //     { NeighborState.Cautious, new ProbabilityData(30) },
        //     { NeighborState.Danger, new ProbabilityData(5) },
        //     { NeighborState.Near, new ProbabilityData(40) }
        // };

        gameObject.SetActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        probability = 100;
    }

    private IEnumerator MainCode()
    {
        yield return new WaitForSeconds(3f);

        Deactivate(); 
    }

    private void RustleSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.pantRustle, this.transform.position);
    }

    public override void Initialize(){}
}
