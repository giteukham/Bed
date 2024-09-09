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
    private Transform point;

    private Vector3 dir;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
        //dir = -(player.position - transform.position);
        //dir = transform.position - player.position;
        //dir = -((player.position - transform.position).normalized);
        //transform.LookAt(new Vector3(dir.x, transform.position.y, dir.z));
        //point.Translate(new Vector3(dir.x, transform.position.y, dir.z));
        //point.position = new Vector3(dir.x, transform.position.y, dir.z);
        //transform.LookAt(point);
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

        //yield return new WaitForSeconds(2);
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

        //허리 x축 -90도로 꺾고, 머리 x축 -90도, 머리 y축 360도로 지속적인 회전
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            waist.localRotation = Quaternion.Slerp(waist.localRotation, Quaternion.Euler(-90, 0, 0), Time.deltaTime);
            neck.LookAt(player.position);

            //몸통이 플레이어 반대쪽으로 바라보게 하는 기능이 의도대로 작동하지 않고 있음
            dir = -(player.position - transform.position);
            transform.LookAt(new Vector3(dir.x, transform.position.y, dir.z));
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(dir), Time.deltaTime);
            //neck.localRotation = Quaternion.Slerp(neck.localRotation, Quaternion.Euler(-90, 180, 0), Time.deltaTime);
        }

        //waist.localRotation = Quaternion.Euler(-90, 0, 0);
        //neck.localRotation = Quaternion.Euler(-90, neck.localRotation.y, 0);

        print("끝");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, dir);

        Gizmos.DrawSphere(transform.position + dir, 0.01f);
    }
}
