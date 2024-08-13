using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class CoverGimcikTest : MonoBehaviour, IGimmick
{
    //�ڽ��� ��� ����Ʈ�� �����ִ��� �ʱⰪ �ٷ� ����
    public ListGroup myGroup { get; set; } = ListGroup.Unreal;

    public int percent { get; set; } = 100;

    public ExPlayer Player { get; set; }

    public Animator animator;

    //��� ������ �� �� ������ ������
    public void OnEnd()
    {
        print("OnEnd");
        if (gameObject.activeSelf == true)
            this.gameObject.SetActive(false);
    }

    //����� ó�� ������ ��
    public void OnStart()
    {
        //print("OnStart");
        OnUpdate();
    }

    //����� �����ϰ� �ִ� ���߿�
    public void OnUpdate()
    {
        //print("OnUpdate");
        if (gameObject.activeSelf == false)
            this.gameObject.SetActive(true);
    }

    private void OnEnable() 
    {
        StartCoroutine(TestCode());
    }

    public void PercentRedefine(bool mouseMove, bool eyeBlink)
    {
        print("�ۼ�Ʈ ��������");

        if (mouseMove == true)
        {
            percent += 5;
        }
        else
        {
            percent -= 5;
        }

        if (percent > 100)
        {
            percent = 100;
        }
        else if (percent < 1)
        {
            percent = 1;
        }
    }

    private IEnumerator TestCode()
    {
        print("CoverGimcikTest Start !");

        yield return new WaitForSeconds(4);
        AudioManager.instance.PlayOneShot(AudioManager.instance.handCover, this.transform.position);
        animator.Play("CoverEye");

        yield return new WaitForSeconds(0.16f);
        AudioManager.instance.PlayOneShot(AudioManager.instance.roughBreath, this.transform.position);

        yield return new WaitForSeconds(2.68f);
        AudioManager.instance.PlayOneShot(AudioManager.instance.handCoverOff, this.transform.position);

        yield return new WaitForSeconds(0.3f);
        animator.Play("CoverOffEye");

        yield return new WaitForSeconds(0.15f);
        OnEnd();

        print("CoverGimcikTest End !");
    }
}