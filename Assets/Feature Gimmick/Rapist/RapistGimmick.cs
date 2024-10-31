using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;

public class RapistGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Human;
    public override float Probability { get; set; } = 100;

    public GameObject hand, houseLight;

    [SerializeField]
    private bool zeroPahse, onePhase, twoPhase, threePhase, fourPhase = false;

    private Animator animator;

    private void Awake()
    {
        gameObject.SetActive(false);
        animator = GetComponent<Animator>();
    }

    private void Update() 
    {
        timeLimit += Time.deltaTime;

        if (zeroPahse)
        {
            animator.Play("Phase 0");
        }
        if (onePhase)
        {
            animator.Play("Phase 1");
            if (houseLight.activeSelf == false)
                houseLight.SetActive(true);
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
        if (zeroPahse == false) 
            zeroPahse = true;

        yield return new WaitForSeconds(3f);
        //1페이즈
        zeroPahse = false;
        if (onePhase == false) 
            onePhase = true;

        //2페이즈
        yield return new WaitForSeconds(3f);
        onePhase = false;
        if (twoPhase == false)
            twoPhase = true;
        if (houseLight.activeSelf == true)
            houseLight.SetActive(false);
        AudioManager.instance.PlaySound(AudioManager.instance.hornyBreath, this.transform.position);
        AudioManager.instance.PlaySound(AudioManager.instance.pantRustle, this.transform.position);

        //3페이즈
        yield return new WaitForSeconds(3f);
        twoPhase = false;
        if (threePhase == false)
            threePhase = true;
        AudioManager.instance.PlaySound(AudioManager.instance.windowOpenClose, this.transform.position);


        // 4페이즈 / 이땐 정면 고정
        yield return new WaitForSeconds(3f);
        threePhase = false;
        if (fourPhase == false)
            fourPhase = true;
        hand.SetActive(true);
        AudioManager.instance.PlaySound(AudioManager.instance.rapist4Phase, this.transform.position);
        
        yield return new WaitForSeconds(3f);
        hand.SetActive(false);
        fourPhase = false;

        Deactivate(); 
    }

    private void RustleSoundPlay()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.pantRustle, this.transform.position);
    }
}
