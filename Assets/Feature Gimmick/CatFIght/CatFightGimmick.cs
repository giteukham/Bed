using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatFightGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    // 기믹 개인 변수
    #endregion
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit = Time.deltaTime;
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
        AudioManager.Instance.PlaySound(AudioManager.Instance.catFight, transform.position);
        yield return new WaitForSeconds(30);
        //스트레스 데미지
        Deactivate();
    }

    public override void Initialize(){}
}
