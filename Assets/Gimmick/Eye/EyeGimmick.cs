using AbstractGimmick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeGimmick : Gimmick
{
    [SerializeField]
    private NewGimmickManager gimmickManager;

    [SerializeField]
    private new Light light;

    public override GimmickType Type { get; protected set; } = GimmickType.Unreal;
    public override float Probability { get; set; } = 100;

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
        light.intensity = 0;
        light.color = Color.white;
        SettingVariables();
        StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        gimmickManager.LowerProbability(this);
        gimmickManager.unrealGimmick = null;
        gameObject.SetActive(false);
    }

    public override void UpdateProbability(ExPlayer player)
    {
        Probability = 100;
    }

    private IEnumerator MainCode()
    {
        while (timeLimit < 15)
        {
            yield return new WaitForSeconds(0.05f);
            if (isDetected == true)
            {
                light.intensity += 0.1f;
            }
            else
            {
                light.intensity -= 0.1f;
            }

            //안광 20 넘으면 기믹 파훼 실패
            if (light.intensity >= 20)
            {
                light.intensity = 20;
                light.color = Color.red;
                for (int i = 0; i < 20; i++)
                {
                    yield return new WaitForSeconds(0.05f);
                    if (light.intensity == 20)
                    {
                        light.intensity = 0;
                    }
                    else
                    {
                        light.intensity = 20;
                    }
                }
                //스트레스 지수, 공포 지수 올리는 코드
                //그다음 종료
                Deactivate();
            }
        }

        Deactivate();

    }
}
