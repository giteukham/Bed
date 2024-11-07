using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSoundGimmick : Gimmick
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
        //개 짖다가
        AudioManager.Instance.PlaySound(AudioManager.Instance.dogBark, transform.position);

        yield return new WaitForSeconds(21);

        //소리 끝나면 바로 개 비명 지름(맞는 소리 넣으면 뭔가 이상할듯)
        AudioManager.Instance.PlaySound(AudioManager.Instance.dogWhine, transform.position);

        yield return new WaitForSeconds(10);
        //스트레스 데미지 주고 끝냄
        Deactivate();
    }

    public override void Initialize(){}
}
