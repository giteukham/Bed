using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    private Animator animator;
    private Rigidbody rig;
    private Vector3 startPoint;
    private Vector3 startRotation;
    private bool isRun = false;
    private float beforeSpeed = 0;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        isRun = false;
        startPoint = transform.position;
        startRotation = transform.rotation.eulerAngles;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDetected == false || isRun == true)
        {
            timeLimit += Time.deltaTime;
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

    public override void Initialize()
    {
        //초기 위치로 이동, 회전
        animator.speed = 1;
        rig.useGravity = false;
        rig.velocity = Vector3.zero;
        transform.position = startPoint;
        transform.rotation = Quaternion.Euler(startRotation);
        isRun = false;
        isDetected = false;
    }

    private IEnumerator MainCode()
    {
        //처음에 기어옴
        animator.speed = 0.7f;
        animator.SetTrigger("Crawl");
        print("크라울");
        timeLimit = 0;
        while (timeLimit <= 5f)
        {
            yield return null;

            if (isDetected == false)
            {
                animator.speed = 0.7f;
                transform.Translate(Vector3.forward * Time.deltaTime * 0.2f);
            }
            else
            {
                animator.speed = 0;
            }
        }

        //살짝 느리게
        animator.speed = 0.3f;
        //animator.SetTrigger("Crawl");
        timeLimit = 0;
        while (timeLimit <= 3f)
        {
            yield return null;

            if (isDetected == false)
            {
                animator.speed = 0.3f;
                transform.Translate(Vector3.forward * Time.deltaTime * 0.15f);
            }
            else
            {
                animator.speed = 0;
            }
        }

        //멈춤(애니메이션 스피드 0으로 하고 이 때 좀비 소리 재생)
        animator.speed = 0;
        //몇초간 대기
        //yield return new WaitForSeconds(8);

        isRun = true;
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

        //몇초후에 데미지 주고 기믹 종료
        yield return new WaitForSeconds(3);
        Deactivate();
    }

    private void SettingCrawl(float animationSpeed, float crawlSpeed)
    {
        animator.speed = 0.7f;
        transform.Translate(Vector3.forward * Time.deltaTime * 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.speed = 0;
            rig.useGravity = false;
            rig.velocity = Vector3.zero;

            animator.speed = 1;
            animator.SetTrigger("Biting");
        }
    }
}
