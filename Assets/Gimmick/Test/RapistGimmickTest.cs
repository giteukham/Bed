using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;

public class RapistGimmickTest : MonoBehaviour, IGimmick
{
    public ListGroup myGroup { get; set; } = ListGroup.Human;

    public int percent { get; set; } = 100;

    public ExPlayer Player { get; set; }

    public GameObject hand, houseLight;

    private bool onePhase, twoPhase, threePhase, fourPhase = false;

    private Animator animator;

    private void Start() 
    {
        animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        if (onePhase)
        {
            animator.Play("Phase 1");
        }

        if (twoPhase)
        {
            animator.Play("Phase 2");
        }

        if (threePhase)
        {
            animator.Play("Phase 3");
        }

        if (fourPhase)
        {
            animator.Play("Phase 4");
        }
    }

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
        print("RapistGimmickTest Start !!");
        //1������
        if (onePhase == false)
            onePhase = true;
        houseLight.SetActive(true);

        //2������
        yield return new WaitForSeconds(3f);
        onePhase = false;
        if (twoPhase == false)
            twoPhase = true;
        houseLight.SetActive(false);
        AudioManager.instance.PlayOneShot(AudioManager.instance.hornyBreath, this.transform.position);
        AudioManager.instance.PlayOneShot(AudioManager.instance.pantRustle, this.transform.position);

        //3������
        yield return new WaitForSeconds(3f);
        twoPhase = false;
        if (threePhase == false)
            threePhase = true;
        AudioManager.instance.PlayOneShot(AudioManager.instance.windowOpenClose, this.transform.position);


        // 4������ / �̶� ���� ����
        yield return new WaitForSeconds(3f);
        threePhase = false;
        if (fourPhase == false)
            fourPhase = true;
        hand.SetActive(true);
        AudioManager.instance.PlayOneShot(AudioManager.instance.rapist4Phase, this.transform.position);
        
        yield return new WaitForSeconds(3f);
        hand.SetActive(false);
        fourPhase = false;
        OnEnd();

        print("RapistGimmickTest End !!");
    }

    private void RustleSoundPlay()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.pantRustle, this.transform.position);
    }
}
