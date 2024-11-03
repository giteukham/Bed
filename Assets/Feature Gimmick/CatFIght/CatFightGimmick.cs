using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatFightGimmick : Gimmick
{
    [field: SerializeField] public override GimmickType Type { get; protected set; }
    public override float Probability { get; set; } = 100;

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
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.catFight, transform.position);
        yield return new WaitForSeconds(30);
        //스트레스 데미지
        Deactivate();
    }

    public override void Initialize(){}
}
