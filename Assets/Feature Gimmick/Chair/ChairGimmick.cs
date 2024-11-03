using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; } = 100;
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    // 기믹 개인 변수
    #endregion
    private void Awake()
    {
        //의자기믹은 처음부터 사라지지 않고 계속 있는걸로
        //gameObject.SetActive(false);
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
        //의자바퀴소리
        AudioManager.instance.PlaySound(AudioManager.instance.chair1, transform.position);
        //약간 좌측으로 이동
        timeLimit = 0;
        while (timeLimit <= 1.3f)
        {
            yield return null;
            transform.Translate(Vector3.left * Time.deltaTime * 0.2f);
        }

        yield return new WaitForSeconds(1.1f);

        //의자바퀴소리
        AudioManager.instance.PlaySound(AudioManager.instance.chair2, transform.position);
        //약간 좌측으로 이동
        timeLimit = 0;
        while (timeLimit <= 3f)
        {
            yield return null;
            transform.Translate(Vector3.left * Time.deltaTime * 0.3f);
        }

        yield return new WaitForSeconds(4);

        //의자바퀴소리
        AudioManager.instance.PlaySound(AudioManager.instance.chair1, transform.position);
        //의자 돌아감
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 4.3f, 0), Time.deltaTime);
        }

        //의자 기믹은 단 한번만 나오게 함
        probability = 0;

        Deactivate();
    }

    public override void Initialize(){ }
}
