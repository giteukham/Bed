using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Unreal;
    public override float Probability { get; set; } = 100;

    private Animator animator;
    private Rigidbody rig;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
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
        //초기 위치로 이동, 회전
        rig.useGravity = false;
        rig.velocity = Vector3.zero;

        gimmickManager.LowerProbability(this);
        gimmickManager.unrealGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        //처음에 기어옴
        animator.speed = 0.7f;
        animator.SetTrigger("Crawl");
        timeLimit = 0;
        while (timeLimit <= 5f)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.2f);
        }

        //살짝 느리게
        animator.speed = 0.3f;
        //animator.SetTrigger("Crawl");
        timeLimit = 0;
        while (timeLimit <= 3f)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.15f);
        }

        //멈춤(애니메이션 스피드 0으로 하고 이 때 좀비 소리 재생)
        animator.speed = 0;
        //몇초간 대기
        //yield return new WaitForSeconds(8);

        //빠르게 기어와서 플레이어 머리 바로 위까지 옴
        animator.speed = 5f;
        //animator.SetTrigger("Crawl");
        timeLimit = 0;
        while (timeLimit <= 0.9f)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 4f);
        }

        //그대로 떨어지면서 몸을 플레이어 향해서 돌림
        animator.speed = 2;
        rig.useGravity = true;
        timeLimit = 0;
        while (timeLimit <= 1f)
        {
            yield return null;
            print("돌리는중");
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * 3f);
        }

        yield return new WaitForSeconds(0.16f);
        animator.speed = 0;
        rig.useGravity = false;
        rig.velocity = Vector3.zero;

        //플레이어 위에 착지 후 물어뜯는 모션(화면 붉어지는 효과 있으면 좋을듯)
        //현재 물어뜯는 모션 잠깐 갔다가 갑자기 기어가는 모션이 다시 실행되는 오류 있음 : animator.speed와는 연관없음
        animator.speed = 1;
        animator.SetTrigger("Biting");
    }

    private void SettingCrawl(float animationSpeed, float crawlSpeed)
    {
        animator.speed = 0.7f;
        transform.Translate(Vector3.forward * Time.deltaTime * 0.2f);
    }
}
