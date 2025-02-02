
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private Door door;
    
    public void IsNotOpen(bool isNotOpen)
    {
        this.door.IsNotOpen = isNotOpen;
    }
    
    public void StartDoorKnock() => door.StartDoorKnock();
    public void StopDoorKnock() => door.StopDoorKnock();

    public void ReadyTutorial()
    {
        StartCoroutine(EyeOpenTutorialCoroutine());
        StartCoroutine(ReadyCheckCoroutine());
    }
    
    private IEnumerator EyeOpenTutorialCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        float startTime = Time.time;
        
        while(door.IsNotOpen)
        {
            if (Time.time - startTime >= 8f && PlayerConstant.isEyeOpen == false && door.IsNotOpen == true) UIManager.Instance.EyeOpenTutorial(true);
            if (BlinkEffect.Blink <= 0.93f) 
            {
                door.IsNotOpen = false;
                UIManager.Instance.EyeOpenTutorial(false);
                door.StartDoorKnock();
                StartCoroutine(LeftMoveTutorialCoroutine());
                yield break;
            }
            yield return null;
        }
    }
    
    private IEnumerator ReadyCheckCoroutine()
    {
        float checkTime = 0f;

        while (true)
        {
            if (PlayerConstant.isLeftState && PlayerConstant.isEyeOpen)
            {
                checkTime += Time.deltaTime;
                if (checkTime >= 2f)
                {
                    GameManager.Instance.SetState(GameState.GamePlay);
                    yield break;
                }
            }
            else
                checkTime = 0f;

            yield return null;
        }
    }

    private IEnumerator LeftMoveTutorialCoroutine()
    {
        float startTime = Time.time;
        while (true)
        {
            if (Time.time - startTime >= 10f && PlayerConstant.isLeftState == false)
                UIManager.Instance.LeftMoveTutorial(true);
            if (PlayerConstant.isLeftState == true)
            {
                UIManager.Instance.LeftMoveTutorial(false);
                yield break;
            }

            yield return null;
        }
    }
}
