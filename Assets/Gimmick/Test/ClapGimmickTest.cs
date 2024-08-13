using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;
using System;

public class ClapGimcikTest : MonoBehaviour, IGimmick
{
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

    //기믹 끝났을 때 맨 마지막 마무리
    public void OnEnd()
    {
        print("OnEnd");
        if (gameObject.activeSelf == true)
            this.gameObject.SetActive(false);
    }

    //기믹이 처음 시작할 때
    public void OnStart()
    {
        
        //print("OnStart");
        OnUpdate();
    }

    //기믹이 시작하고 있는 도중에
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
        print("퍼센트 리디파인");

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
        
        AudioManager.instance.PlayOneShot(AudioManager.instance.switchOn, this.transform.position);
        // 스위치 키는 소리

        yield return new WaitForSeconds(0.4f);
        animator.Play("Clapping");

        yield return new WaitForSeconds(1.5f);
        animator.Play("ClapOff");

        yield return new WaitForSeconds(0.4f);
        houseLight.enabled = false;

        AudioManager.instance.PlayOneShot(AudioManager.instance.switchOff, this.transform.position);
        // 스위치 끄는 소리

        OnEnd();

        print("ClapGimcikTest End !!");
    }

    private void ClapSoundPlay()
    {
        // 박수 소리는 애니메이션 이벤트로 실행( 타이밍을 자연스럽게 하기 위해 ~ )
        AudioManager.instance.PlayOneShot(AudioManager.instance.handClap, this.transform.position);
        Debug.Log("clap");
    }
}