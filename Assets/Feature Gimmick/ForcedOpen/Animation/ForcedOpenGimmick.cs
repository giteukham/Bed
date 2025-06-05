using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using DG.Tweening;

public class ForcedOpenGimmick : Gimmick
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
    }

    private IEnumerator MainCode()
    {
        yield return new WaitUntil(() => !PlayerConstant.isEyeOpen);
        PlayerConstant.isEyeParalysis = true;
        animator.Play("Opening");
        UIManager.Instance.ForceCloseSettingsScreen();  
        PlayerConstant.canOpenMenu = false;
        yield return new WaitForSeconds(0.05f);
        DOVirtual.Float(1f, 0.9f, 0.12f, value => {
            GameManager.Instance.player.ForceSetEyeValue(value);
        });
        yield return new WaitForSeconds(0.25f);
        DOVirtual.Float(0.9f, 0.1f, 0.6f, value => {
            GameManager.Instance.player.ForceSetEyeValue(value);
        });
        yield return new WaitForSeconds(0.6f);
        DOVirtual.Float(0.1f, 0.2f, 0.1f, value => {
            GameManager.Instance.player.ForceSetEyeValue(value);
        });
        yield return new WaitForSeconds(0.2f);
        Deactivate();
    }

    public override void Initialize() 
    {
        PlayerConstant.isEyeParalysis = false;
        PlayerConstant.canOpenMenu = true;
    }
}
