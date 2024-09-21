using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;

public class ScareCrowGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private float rotationX = 0;
    private bool isMinus = false;

    private void Awake()
    {
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
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
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
        //까마귀 소리 점점 더 시끄럽게 나는 걸로, 몇초 지나면 그냥 흔들거리게만
        yield return null;
        timeLimit = 0;
        while (timeLimit <= 5)
        {
            yield return null;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime);
        }

        yield return new WaitForSeconds(3);

        print(transform.localRotation.eulerAngles.x);
        timeLimit = 0;
        while (timeLimit <= 200)
        {
            yield return null;
            Wobble();
        }
    }

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
}
