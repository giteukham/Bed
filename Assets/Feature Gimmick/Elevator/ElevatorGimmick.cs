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

    [SerializeField] private GameObject elevatorLight, elevatorBlinkLight, floorLight, leftDoor, rightDoor;
    private Coroutine blinkLightCoroutine;
    [SerializeField] private Transform hitAndCrashSoundPos, moveAndFastSoundPos, laughSoundPos1, laughSoundPos2, laughSoundPos3, laughSoundPos4, screamSoundPos;
    [SerializeField] private GameObject soundPlayPrefab;
    // [SerializeField] private Transform moveAndFastSoundPos;

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
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorMove, moveAndFastSoundPos.position);
        camShaker.ShakeCam(66, 0.01f);

        //?????????? ????? ???? ??????
        while (floor > 2)
        {
            yield return new WaitForSeconds(2);
            floor--;
            tmo.text = floor.ToString();
        }
        //??! ????? ??? ???? ??? ?????????? ????? ????
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorCrash, hitAndCrashSoundPos.position);

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
        camShaker.StopShakeCam();
        AudioManager.instance.StopSound(AudioManager.instance.elevatorMove, FMOD.Studio.STOP_MODE.IMMEDIATE);
        tmo.text = "31212012";
        yield return new WaitForSeconds(5);

        //??? ?????? ???? ???? ???? ?????
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorMove, moveAndFastSoundPos.position);
        camShaker.ShakeCam(66, 0.01f);
        while (floor <= 6)
        {
            yield return new WaitForSeconds(0.7f);
            floor++;
            tmo.text = floor.ToString();
        }

        //??! ??????? ???? ???
        AudioManager.instance.PlaySound(AudioManager.instance.hit, hitAndCrashSoundPos.position);
        camShaker.StopShakeCam();
        impulseSource.GenerateImpulseWithForce(4);
        //엘베 바꾸면서 0번째로 바뀜
        tempMatList[0] = BlackLightMat;
        ren.materials = tempMatList;

        // add by kwon
        StopCoroutine(blinkLightCoroutine);
        elevatorBlinkLight.SetActive(false);

        //????? ????? ???
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorFast, moveAndFastSoundPos.position);
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

        //???? ??? ??????? ?? ?? ??? ????
        camShaker.StopShakeCam();
        camShaker.ShakeCam(66, 0.07f);

        //??? ?÷???? ?????? ????
        PlayerConstant.ResetLATStats();

        for (int i = 0; i < 1500; i++)
        {
            if (PlayerConstant.EyeBlinkLAT >= 6)
            {
                Deactivate();
                yield break;
            }
            randomNum = Random.Range(1000, 40000);
            tmo.text = randomNum.ToString();

            //if (i == 1) AudioManager.instance.PlaySound(AudioManager.instance.creepy1, laughSoundPos1.position);

            if (i == 1335) AudioManager.instance.PlaySound(AudioManager.instance.creepyScream, screamSoundPos.position);
            yield return null;
        }

        //???? ??? ????
        tmo.text = "31212012";
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorStop, moveAndFastSoundPos.position);
        yield return new WaitForSeconds(0.4f);
        AudioManager.instance.StopSound(AudioManager.instance.elevatorMove, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        AudioManager.instance.StopSound(AudioManager.instance.elevatorFast, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        camShaker.StopShakeCam();
        impulseSource.GenerateImpulseWithForce(3.5f);
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorDing, moveAndFastSoundPos.position);
        yield return new WaitForSeconds(3.5f);

        Vector3 leftDoorInitPosition = leftDoor.transform.localPosition;
        Vector3 leftDoorTargetPosition = new(leftDoorInitPosition.x, leftDoorInitPosition.y, -1.1f);

        Vector3 rightDoorInitPosition = rightDoor.transform.localPosition;
        Vector3 rightDoorTargetPosition = new(leftDoorInitPosition.x, leftDoorInitPosition.y, 1.1f);

        float elapsedTime = 0f;
        
        AudioManager.instance.PlaySound(AudioManager.instance.elevatorDoorOpen, moveAndFastSoundPos.position);
        camShaker.ShakeCam(1.6f, 0.07f);
        while (elapsedTime < 1.6f)
        {
            float t = elapsedTime / 1.6f;

            leftDoor.transform.localPosition = Vector3.Lerp(leftDoorInitPosition, leftDoorTargetPosition, t);
            rightDoor.transform.localPosition = Vector3.Lerp(rightDoorInitPosition, rightDoorTargetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //Deactivate();
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

    // add by kwon
    private void InstantiateSoundPrefab(Vector3 _pos, EventReference _sound)
    {
        GameObject prefab = Instantiate(soundPlayPrefab, _pos, Quaternion.identity);
        prefab.GetComponent<SoundPlayPrefab>().soundReference = _sound;
        prefab.SetActive(true);
        Destroy(prefab, 2.5f);
        
    }
}
