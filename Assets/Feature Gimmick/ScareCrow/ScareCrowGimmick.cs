using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;

public class ScareCrowGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [field: SerializeField] public override float probability { get; set; }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion

    #region Variables
    private float rotationX = 0;
    private bool isMinus = false;
    private Rigidbody rig;
    private BoxCollider boxColl;
    private Vector3 startPosition;
    private int soundNum = 0;
    #endregion

    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
        boxColl = GetComponent<BoxCollider>();
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
        //처음에 일어섬
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime);
        }

        boxColl.isTrigger = false;
        yield return new WaitForSeconds(3);

        //위로 상승
        rig.AddForce(new Vector3(0, 1, 4));

        yield return new WaitForSeconds(5);

        //갑자기 넘어짐
        rig.velocity = Vector3.zero;
        rig.useGravity = true;
        rig.AddForce(new Vector3(3, 0, -4));

        yield return new WaitForSeconds(3);

        //다시 천천히 일어섬
        rig.useGravity = false;
        timeLimit = 0;
        while (timeLimit <= 10)
        {
            yield return null;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 0.5f);
        }

        //다시 넘어짐
        rig.velocity = Vector3.zero;
        rig.useGravity = true;
        rig.AddForce(new Vector3(-5, 0, 7));

        yield return new WaitForSeconds(3);

        //까마귀 소리 내고 끝냄
        AudioManager.Instance.PlaySound(AudioManager.Instance.crows, transform.position);
        yield return new WaitForSeconds(9);

        //공포 데미지 상승
        Deactivate();

        /*timeLimit = 0;
        while (timeLimit <= 200)
        {
            yield return null;
            Wobble();
        }*/
    }

    //현재 사용 안함
    private void Wobble()
    {
        if (rotationX <= -12f)
        {
            isMinus = true;
        }
        else if (rotationX >= 12f)
        {
            isMinus = false;
        }

        if (isMinus == true)
        {
            rotationX += Time.deltaTime * 10;
        }
        else
        {
            rotationX -= Time.deltaTime * 10;
        }

        transform.localRotation = Quaternion.Euler(rotationX, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //부모 콜라이더에서만 충돌할때 소리나게
        if (collision.contacts[0].thisCollider.CompareTag("Gimmick"))
        {
            soundNum++;
            if (soundNum == 1)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.woodDrop1, transform.position);
            }
            else
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.woodDrop2, transform.position);
            }
        }
    }

    public override void Initialize()
    {
        rig.useGravity = false;
        boxColl.isTrigger = true;
        soundNum = 0;
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
        transform.position = startPosition;
    }
}
