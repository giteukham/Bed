using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;

public class RapistGimmick : Gimmick
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    private void Update() 
    {
        timeLimit += Time.deltaTime;

        if (zeroPahse)
        {
            animator.Play("Phase 0");
        }
        if (onePhase)
        {
            animator.Play("Phase 1");
            if (houseLight.activeSelf == false)
                houseLight.SetActive(true);
        }

        if (twoPhase)
        {
            animator.Play("Phase 2");
        }

        if (threePhase)
        {
            animator.Play("Phase 3");
        }

        if (fourPhase)
        {
            animator.Play("Phase 4");
        }
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
        if (zeroPahse == false) 
            zeroPahse = true;

        yield return new WaitForSeconds(3f);
        //1페이즈
        zeroPahse = false;
        if (onePhase == false) 
            onePhase = true;

        //2페이즈
        yield return new WaitForSeconds(3f);
        onePhase = false;
        if (twoPhase == false)
            twoPhase = true;
        if (houseLight.activeSelf == true)
            houseLight.SetActive(false);
        AudioManager.Instance.PlaySound(AudioManager.Instance.hornyBreath, this.transform.position);
        AudioManager.Instance.PlaySound(AudioManager.Instance.pantRustle, this.transform.position);

        //3페이즈
        yield return new WaitForSeconds(3f);
        twoPhase = false;
        if (threePhase == false)
            threePhase = true;
        AudioManager.Instance.PlaySound(AudioManager.Instance.windowOpenClose, this.transform.position);


        // 4페이즈 / 이땐 정면 고정
        yield return new WaitForSeconds(3f);
        threePhase = false;
        if (fourPhase == false)
            fourPhase = true;
        hand.SetActive(true);
        AudioManager.Instance.PlaySound(AudioManager.Instance.rapist4Phase, this.transform.position);
        
        yield return new WaitForSeconds(3f);
        hand.SetActive(false);
        fourPhase = false;

        Deactivate(); 
    }

    private void RustleSoundPlay()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.pantRustle, this.transform.position);
    }

    public override void Initialize(){}
}
