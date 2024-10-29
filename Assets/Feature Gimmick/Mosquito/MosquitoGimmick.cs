using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private Vector3 startPosition;
    [SerializeField]
    private Transform playerPosition;

    private int randomNum = 0;
    private float[] randomValue = new float[3];

    private Vector3 point = Vector3.zero;

    private void Awake()
    {
        startPosition = transform.position;
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
        transform.position = startPosition;

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
        //소리재생
        AudioManager.instance.PlaySound(AudioManager.instance.mosquito, transform.position);

        yield return new WaitForSeconds(1);

        //랜덤 위치로 이동
        timeLimit = 0;
        point = RandomPosition(3f);
        while (timeLimit <= 3)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, point, 0.5f * Time.deltaTime);
        }

        //랜덤 위치로 이동
        timeLimit = 0;
        point = RandomPosition(3f);
        while (timeLimit <= 3)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, point, 0.5f * Time.deltaTime);
        }

        //플레이어쪽으로 이동
        timeLimit = 0;
        point = RandomPosition(0.1f);
        while (timeLimit <= 3)
        {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, playerPosition.position + point, 2f * Time.deltaTime);
        }

        //스트레스 데미지 주고 끝냄

        Deactivate();
    }

    private Vector3 RandomPosition(float num)
    {
        randomNum = Random.Range(0, 2);

        for (int i = 0; i < 3; i++)
        {
            if (randomNum == 0)
            {
                randomValue[i] = num * 1;
            }
            else
            {
                randomValue[i] = num * -1;
            }
        }

        return new Vector3(randomValue[0], randomValue[1], randomValue[2]);
    }
}
