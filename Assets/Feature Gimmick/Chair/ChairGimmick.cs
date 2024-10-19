using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

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
        SettingVariables();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.objectGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        yield return null;
        //의자바퀴소리
        //약간 좌측으로 이동

        yield return new WaitForSeconds(4);

        //갑자기 벽쪽으로 던져짐
    }
}
