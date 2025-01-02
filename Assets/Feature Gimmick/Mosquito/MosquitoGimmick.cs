using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MosquitoGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [SerializeField] private float _probability;
    public override float probability 
    { 
        get => _probability; 
        set => _probability = Mathf.Clamp(value, 0, 100); 
    }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    private Vector3 startPosition;
    [SerializeField] private Transform playerPosition;
    #endregion

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
        //소리재생
        AudioManager.Instance.PlaySound(AudioManager.Instance.mosquito, transform.position);

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

    public override void Initialize()
    {
        transform.position = startPosition;
    }
}
