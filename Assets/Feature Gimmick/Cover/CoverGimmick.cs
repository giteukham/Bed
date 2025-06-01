using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class CoverGimcik : Gimmick
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
    public Animator animator;
    #endregion

    private void Awake()
    {
    }

    private void Update()
    {
        
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
        probability = ((PlayerConstant.EyeBlinkLAT * 8) + (PlayerConstant.EyeBlinkCAT * 2)) * (PlayerConstant.isEyeOpen ? 1 : 0);
    }

    private IEnumerator MainCode()
    {
        yield return new WaitForSeconds(1);
        AudioManager.Instance.PlayForce(AudioKeys.HandCover, this.transform.position);
        animator.Play("CoverEye");

        yield return new WaitForSeconds(0.16f);

        AudioManager.Instance.PlayForce(AudioKeys.RoughBreath, this.transform.position);

        yield return new WaitForSeconds(2.68f);
        AudioManager.Instance.PlayForce(AudioKeys.HandCoverOff, this.transform.position);

        yield return new WaitForSeconds(0.3f);
        animator.Play("CoverOffEye");

        yield return new WaitForSeconds(0.15f);
        Deactivate();
    }

    public override void Initialize() {}
}