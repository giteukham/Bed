using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class ClapGimcikTest : MonoBehaviour, IGimmick
{
    //�ڽ��� ��� ����Ʈ�� �����ִ��� �ʱⰪ �ٷ� ����
    public ListGroup myGroup { get; set; } = ListGroup.Unreal;

    public int percent { get; set; } = 100;

    public ExPlayer Player { get; set; }

    public Light houseLight;

    public Animator animator;

    private AnimatorStateInfo currentAnimatorStateInfo;

    private void Start() 
    {
        currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }

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
        print("ClapGimcikTest Start !!");

        yield return new WaitForSeconds(3.2f);
        houseLight.enabled = true;

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.6f);
        houseLight.enabled = false;
        OnEnd();

        print("ClapGimcikTest End !!");

        // if (currentAnimatorStateInfo.IsName("Clapping") && currentAnimatorStateInfo.normalizedTime <= 0.1f) 
        // {
            
        // }
    }
}