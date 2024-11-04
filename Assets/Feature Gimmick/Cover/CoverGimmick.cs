using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AbstractGimmick;

public class CoverGimcik : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    public Animator animator;
    #endregion

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
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
        yield return new WaitForSeconds(4);
        AudioManager.instance.PlaySound(AudioManager.instance.handCover, this.transform.position);
        animator.Play("CoverEye");

        yield return new WaitForSeconds(0.16f);
        AudioManager.instance.PlaySound(AudioManager.instance.roughBreath, this.transform.position);

        yield return new WaitForSeconds(2.68f);
        AudioManager.instance.PlaySound(AudioManager.instance.handCoverOff, this.transform.position);

        yield return new WaitForSeconds(0.3f);
        animator.Play("CoverOffEye");

        yield return new WaitForSeconds(0.15f);
        Deactivate();
    }

    public override void Initialize() {}
}