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
    private Transform clockKey;

    [SerializeField]
    private Transform movePoints;

    [SerializeField]
    private Transform[] pointsArray;

    //기믹에서 플레이어 방향의 반대방향을 저장할 변수
    private Vector3 dir;

    private float rotationY;
    private float rotationZ;
    private float currentY;

    private void Awake()
    {
        pointsArray = movePoints.GetComponentsInChildren<Transform>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //플레이어가 보지 않을때만 움직임(생각 더 해볼것)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("스페이스바");
            transform.LookAt(pointsArray[1].position);
        }
        timeLimit += Time.deltaTime;
        clockKey.Rotate(0, 0, 100 * Time.deltaTime);
        //dir은 항상 기믹 기준 플레이어 반대 방향을 가리킴
        //dir = -(player.position - transform.position);
    }

    public override void Activate()
    {
        SettingVariables();
        //포인트 오브젝트 월드 오브젝트로 바꿈
        movePoints.SetParent(null);
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.humanGimmick = null;
        //기믹 상태 원래위치로 변경하고 로테이션 원래대로
        waist.localRotation = Quaternion.identity;
        neck.localRotation = Quaternion.identity;
        head.localRotation = Quaternion.identity;
        rightArm.localRotation = Quaternion.Euler(80, 10, 16);
        leftArm.localRotation = Quaternion.Euler(80, -10, -16);

        transform.position = pointsArray[0].position;
        transform.rotation = Quaternion.Euler(0, 180, 0);

        //포인트 오브젝트 로컬 오브젝트로 바꿈
        movePoints.SetParent(transform);

        gimmickManager.gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        //첫번째 포인트 바라봄
        transform.LookAt(pointsArray[1].position);

        //노크소리가 들리고 잠시 후 문이 열림
        AudioManager.instance.PlaySound(AudioManager.instance.knock, transform.position);

        yield return new WaitForSeconds(3);

        currentY = transform.eulerAngles.y;
        //문 넘어서 살짝 앞으로 직진함
        AudioManager.instance.PlaySound(AudioManager.instance.toyWalk, transform.position);
        timeLimit = 0;
        while (timeLimit <= 3)
        {
            yield return null;
            //transform.Translate(Vector3.forward * Time.deltaTime * 0.5f);
            transform.position = Vector3.MoveTowards(transform.position, pointsArray[1].position, 0.5f * Time.deltaTime);
            PenguinMove();
            print("실행중");
        }
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        AudioManager.instance.StopSound(AudioManager.instance.toyWalk, FMOD.Studio.STOP_MODE.IMMEDIATE);

        yield return new WaitForSeconds(2);
        //소리 난뒤 갑자기 두번째 포인트 쪽으로 고개 확 돌림
        AudioManager.instance.PlaySound(AudioManager.instance.neckSnap, transform.position);
        neck.LookAt(new Vector3(pointsArray[2].position.x, neck.position.y, pointsArray[2].position.z));
        
        //잠시 대기
        yield return new WaitForSeconds(2);

        //몸을 두번째 포인트로 향할 때 까지 3초 동안 회전
        AudioManager.instance.PlaySound(AudioManager.instance.cogWheell, transform.position);
        timeLimit = 0;
        while (timeLimit <= 3)
        {
            yield return null;
            transform.rotation = Quaternion.Slerp(transform.rotation, neck.rotation, Time.deltaTime);
            neck.LookAt(new Vector3(pointsArray[2].position.x, neck.position.y, pointsArray[2].position.z));
        }
        AudioManager.instance.StopSound(AudioManager.instance.cogWheell, FMOD.Studio.STOP_MODE.IMMEDIATE);

        //목 원상복귀
        neck.localRotation = Quaternion.identity;

        //잠시 대기
        yield return new WaitForSeconds(2);

        currentY = transform.eulerAngles.y;
        //두번째 포인트로 직진
        AudioManager.instance.PlaySound(AudioManager.instance.toyWalk, transform.position);
        timeLimit = 0;
        while (timeLimit <= 5.5f)
        {
            yield return null;
            //transform.Translate(Vector3.forward * Time.deltaTime * 0.5f);
            transform.position = Vector3.MoveTowards(transform.position, pointsArray[2].position, 0.5f * Time.deltaTime);
            PenguinMove();
        }
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        AudioManager.instance.StopSound(AudioManager.instance.toyWalk, FMOD.Studio.STOP_MODE.IMMEDIATE);

        //웃는 소리 재생(너무 뻔함)
        yield return new WaitForSeconds(2);
        
        //목을 천천히 우측으로 돌림
        timeLimit = 0;
        while (timeLimit <= 3)
        {
            yield return null;
            neck.localRotation = Quaternion.Slerp(neck.localRotation, Quaternion.Euler(0, 90, 0), Time.deltaTime);
        }
        //목 갑자기 플레이어 쪽으로 확 꺾음
        AudioManager.instance.PlaySound(AudioManager.instance.neckSnap, transform.position);
        neck.LookAt(player.position);
        yield return new WaitForSeconds(2);

        AudioManager.instance.PlaySound(AudioManager.instance.cogWheell, transform.position);
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            //허리 x축 -90도로 꺾음
            waist.localRotation = Quaternion.Slerp(waist.localRotation, Quaternion.Euler(-90, 0, 0), Time.deltaTime);
            //얼굴은 플레이어를 향해 고정
            neck.LookAt(player.position);

            //몸통은 플레이어 반대방향을 바라보게 함
            dir = -(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z)), Time.deltaTime);
        }

        //머리 z축 천천히 회전, 양팔 넓게 벌림
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            head.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
            leftArm.localRotation = Quaternion.Slerp(leftArm.localRotation, Quaternion.Euler(-10, 0, 0), Time.deltaTime);
            rightArm.localRotation = Quaternion.Slerp(rightArm.localRotation, Quaternion.Euler(-10, 0, 0), Time.deltaTime);
        }

        //새총 당기듯이 뒤로 살짝 감
        timeLimit = 0;
        while(timeLimit <= 3)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.1f);
        }
        AudioManager.instance.StopSound(AudioManager.instance.cogWheell, FMOD.Studio.STOP_MODE.IMMEDIATE);

        yield return new WaitForSeconds(4);

        //플레이어를 향해 돌진
        timeLimit = 0;
        while (timeLimit <= 0.1f)
        {
            yield return null;
            //transform.Translate(Vector3.back * Time.deltaTime * 30f);
            transform.position = Vector3.MoveTowards(transform.position, pointsArray[3].position, 30 * Time.deltaTime);
        }
        waist.localRotation = Quaternion.Euler(-120, 0, 0);
        neck.LookAt(player.position);
        head.LookAt(player.position);

        //이때 플레이어가 눈을 뜬 상태라면 데미지 줌
        if (true)
        {
            //데미지 코드와 공포영화에서 놀래킬 때 쓰는 소리
        }

        //소리 끝날때까지 대기
        yield return new WaitForSeconds(2);

        //기믹 종료
        Deactivate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, dir);

        Gizmos.DrawSphere(transform.position + dir, 0.03f);
    }

    private void PenguinMove()
    {
        float max = currentY + 10;
        float min = currentY - 10;

        //시간에 따라 회전값 생성
        rotationY = Mathf.PingPong(Time.time * 25, max - min) + min;
        rotationZ = -(Mathf.PingPong(Time.time * 15, 12) - 6);
        //현재 Z 회전값에 회전 추가
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotationY, rotationZ);
    }
}
