using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteManGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Human;
    public override float Probability { get; set; } = 100;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Transform waist;
    
    [SerializeField]
    private Transform neck;

    [SerializeField]
    private Transform head;

    [SerializeField]
    private Transform leftArm;

    [SerializeField]
    private Transform rightArm;

    [SerializeField]
    private Transform point;

    //기믹에서 플레이어 방향의 반대방향을 저장할 변수
    private Vector3 dir;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDetected == false)
        {
            timeLimit += Time.deltaTime;
        }
        dir = -(player.position - transform.position);
        //transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        //transform.LookAt(new Vector3(dir.x, 0, dir.z) + transform.position);
    }

    public override void Activate()
    {
        SettingVariables();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.humanGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        //문 넘어서 살짝 앞으로 직진함
        while (timeLimit <= 5)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.5f);
        }

        yield return new WaitForSeconds(2);
        //소리 난뒤 갑자기 벽 쪽으로 고개 확 돌림
        neck.LookAt(new Vector3(point.position.x, neck.position.y, point.position.z));
        
        //잠시 대기
        yield return new WaitForSeconds(2);

        timeLimit = 0;
        //몸을 벽으로 향할때 까지 3초동안 회전
        while (timeLimit <= 3)
        {
            yield return null;
            transform.rotation = Quaternion.Slerp(transform.rotation, neck.rotation, Time.deltaTime);
            neck.LookAt(new Vector3(point.position.x, neck.position.y, point.position.z));
        }

        //목 원상복귀(localRotation 써줘야 버그 안남)
        neck.localRotation = Quaternion.identity;

        //잠시 대기
        yield return new WaitForSeconds(2);

        timeLimit = 0;
        //벽으로 직진
        while (timeLimit <= 3)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.5f);
            print("2번째 이동");
        }

        //웃는 소리 재생(너무 뻔함)
        yield return new WaitForSeconds(2);
        
        timeLimit = 0;
        //플레이어 쪽으로 목 돌림
        while (timeLimit <= 3)
        {
            yield return null;
            neck.LookAt(player.position);
        }

        print(neck.localRotation.eulerAngles);

        //허리 x축 -90도로 꺾고, 머리 x축 -90도
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            //허리 서서히 뒤로 꺾음
            waist.localRotation = Quaternion.Slerp(waist.localRotation, Quaternion.Euler(-90, 0, 0), Time.deltaTime);
            //얼굴은 플레이어를 향해 고정
            neck.LookAt(player.position);

            //몸통은 플레이어 반대방향을 바라보게 함
            dir = -(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), Time.deltaTime);
        }

        //허리, 목, 몸통 로테이션 조정
        waist.localRotation = Quaternion.Euler(-90, 0, 0);
        neck.localRotation = Quaternion.Slerp(neck.localRotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));

        //머리 z축 계속회전, 양팔 넓게 벌림
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            head.Rotate(new Vector3(0, 0, 720 * Time.deltaTime));
            leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, Quaternion.Euler(-10, 0, 0), Time.deltaTime);
            rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, Quaternion.Euler(-10, 0, 0), Time.deltaTime);
        }

        //돌진

        print("끝");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, dir);

        Gizmos.DrawSphere(transform.position + dir, 0.03f);
    }
}
