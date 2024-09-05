using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using Cinemachine;
using TMPro;
using FMODUnity;

public class ElevatorGimmick : Gimmick
{
    public override GimmickType Type { get; protected set; } = GimmickType.Object;
    public override float Probability { get; set; } = 100;

    private CinemachineImpulseSource impulseSource;
    private TextMeshPro tmo;
    private CamShaker camShaker;
    private int floor = 6;
    private int randomNum = 0;

    [SerializeField]
    private GameObject room;

    [SerializeField]
    private MeshRenderer ren;
    private Material[] tempMatList;
    [SerializeField]
    private Material whiteLightMat;
    [SerializeField]
    private Material BlinkLightMat;
    [SerializeField]
    private Material BlackLightMat;
    
    // add by kwon
    [SerializeField] private GameObject elevatorLight;
    [SerializeField] private GameObject elevatorBlinkLight;
    [SerializeField] private GameObject floorLight;
    private Coroutine blinkLightCoroutine;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        tmo = GetComponentInChildren<TextMeshPro>();
        camShaker = GetComponent<CamShaker>();
        tempMatList = ren.materials;
        tempMatList[0] = whiteLightMat;
        ren.materials = tempMatList;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        timeLimit += Time.deltaTime;
    }

    public override void Activate()
    {
        SettingVariables();
        room.SetActive(false);

        // add by kwon
        elevatorLight.SetActive(true);
        floorLight.SetActive(true);
        elevatorBlinkLight.SetActive(false);

        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.objectGimmick = null;
        camShaker.StopShakeCam();
        floor = 6;
        tmo.text = "6";
        //엘베 바꾸면서 0번째로 바뀜
        tempMatList[0] = whiteLightMat;
        ren.materials = tempMatList;
        //?????????? ????? ????
        AudioManager.instance.StopSound(AudioManager.instance.elevatorMove, FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.instance.StopSound(AudioManager.instance.elevatorFast, FMOD.Studio.STOP_MODE.IMMEDIATE);
        room.SetActive(true);
        gameObject.SetActive(false);
    }

    public override void UpdateProbability()
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        //????? ???? ????? ????????
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorMove, transform.position);

        //?????????? ????? ???? ??????
        while (floor > 2)
        {
            yield return new WaitForSeconds(2);
            floor--;
            tmo.text = floor.ToString();
        }
        //??! ????? ??? ???? ??? ?????????? ????? ????
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorCrash, transform.position);

        // add by kwon
        //yield return new WaitForSeconds(1.7f);
        yield return new WaitForSeconds(0.8f);
        elevatorLight.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        elevatorLight.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        elevatorLight.SetActive(false);
        yield return new WaitForSeconds(0.12f);
        elevatorLight.SetActive(true);
        yield return new WaitForSeconds(0.08f);
        elevatorLight.SetActive(false);
        yield return new WaitForSeconds(0.03f);
        elevatorLight.SetActive(true);
        yield return new WaitForSeconds(0.32f);
        impulseSource.GenerateImpulseWithForce(3);
        elevatorLight.SetActive(false);

        // add by kwon
        blinkLightCoroutine = StartCoroutine(BlinkLight());

        //엘베 바꾸면서 0번째로 바뀜
        tempMatList[0] = BlinkLightMat;
        ren.materials = tempMatList;
        AudioManager.instance.StopSound(AudioManager.instance.elevatorMove, FMOD.Studio.STOP_MODE.IMMEDIATE);
        tmo.text = "ERROR";
        yield return new WaitForSeconds(5);


        //??? ?????? ???? ???? ???? ?????
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorMove, transform.position);
        while (floor <= 6)
        {
            yield return new WaitForSeconds(0.7f);
            floor++;
            tmo.text = floor.ToString();
        }

        //??! ??????? ???? ???
        AudioManager.instance.PlaySound(AudioManager.instance.hit, transform.position);
        impulseSource.GenerateImpulseWithForce(4);
        //엘베 바꾸면서 0번째로 바뀜
        tempMatList[0] = BlackLightMat;
        ren.materials = tempMatList;

        // add by kwon
        StopCoroutine(blinkLightCoroutine);
        elevatorBlinkLight.SetActive(false);

        //????? ????? ???
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorFast, transform.position);
        while (floor < 66)
        {
            yield return new WaitForSeconds(0.05f);
            floor++;
            tmo.text = floor.ToString();
        }
        //???? ?????????? ??? ???
        camShaker.ShakeCam(66, 0.02f);

        while (floor < 666)
        {
            yield return null;
            floor++;
            tmo.text = floor.ToString();
        }
        //???? 666 ??? ??? ???
        yield return new WaitForSeconds(0.6f);

        //???? ??? ??????? ?? ?? ??? ????
        camShaker.StopShakeCam();
        camShaker.ShakeCam(66, 0.1f);

        //??? ?÷???? ?????? ????
        PlayerConstant.ResetLATStats();

        //????? ???? ????? ????(??? 9???)
        for (int i = 0; i < 1500; i++)
        {
            yield return null;
            //6?? ??? ?? ??????? ?????? ???? ???????
            if (PlayerConstant.EyeBlinkLAT >= 6)
            {
                Deactivate();
                yield break;
            }
            randomNum = Random.Range(100000, 1000000000);
            tmo.text = randomNum.ToString();
        }

        //???? ??? ????
        tmo.text = "666!66_66";
        camShaker.StopShakeCam();

        //?÷???? ?????? ??? ???????
        Deactivate();
    }


    // add by kwon
    private IEnumerator BlinkLight() 
    {
        while (true)
        {
            elevatorBlinkLight.SetActive(!elevatorBlinkLight.activeSelf);
            float randomNum = Random.Range(0.05f, 0.18f);
            yield return new WaitForSeconds(randomNum);
        }
    }
}
