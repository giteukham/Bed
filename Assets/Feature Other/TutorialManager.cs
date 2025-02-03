
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    private bool isEyeOpenTutorialEnabled = false;

    public void ReadyTutorial()
    {
        StartCoroutine(EyeOpenTutorialCoroutine());
    }
    
    private IEnumerator EyeOpenTutorialCoroutine()
    {
        if(!GameManager.Instance.isBlinkInit) yield return new WaitForSeconds(0.25f);
        float startTime = Time.time;
        
        while(isEyeOpenTutorialEnabled == false)
        {
            if (Time.time - startTime >= 8f && PlayerConstant.isEyeOpen == false && isEyeOpenTutorialEnabled == false) UIManager.Instance.EyeOpenTutorial(true);
            if (BlinkEffect.Blink <= 0.93f) 
            {
                isEyeOpenTutorialEnabled = true;
                UIManager.Instance.EyeOpenTutorial(false);
                //StartCoroutine(LeftMoveTutorialCoroutine());
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator LeftMoveTutorialCoroutine()
    {
        isEyeOpenTutorialEnabled = true;

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
