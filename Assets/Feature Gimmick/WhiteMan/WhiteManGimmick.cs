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

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
        //neck.LookAt(new Vector3(player.position.x, neck.position.y, player.position.z));
        neck.LookAt(player.position);

    }

    public override void Activate()
    {
        SettingVariables();
        //StartCoroutine(MainCode());
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
        //그냥 직진함
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

        //목 원상복귀(0, -172, 0으로 적용되는 버그 있음)
        //neck.rotation = Quaternion.Euler(0, 0, 0);
        //neck.rotation = Quaternion.identity;

        //잠시 대기
        yield return new WaitForSeconds(2);

        timeLimit = 0;
        while (timeLimit <= 3)
        {
            yield return null;
            transform.Translate(Vector3.forward * Time.deltaTime * 0.5f);
        }

        //웃는 소리 재생
        yield return new WaitForSeconds(2);

        timeLimit = 0;
        while (timeLimit <= 3)
        {
            yield return null;
            neck.LookAt(new Vector3(point.position.x, neck.position.y, point.position.z));
        }

        print("끝");
    }
}
