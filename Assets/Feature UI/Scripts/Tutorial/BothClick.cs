using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BothClick : Tutorial
{
    [SerializeField] private float runTimeFadeInDuration = 0.1f;  // 페이드 인
    [SerializeField] private float runTimeFadeOutDuration = 0.3f;  // 페이드 아웃

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvasGroup = parent.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        parent.GetComponent<CanvasGroup>().DOFade(1, fadeInDuration);

        if (sequenceOwn != null && sequenceShake != null)
        {
            sequenceOwn.Restart();
            sequenceShake.Restart();
        }
        else PlayEffect();
    }

    private void PlayEffect()
    {
        sequenceShake = DOTween.Sequence();
        sequenceShake.Append(Shake());

        sequenceOwn = DOTween.Sequence();

        //서서히 나타남
        sequenceOwn.Append(canvasGroup.DOFade(1, runTimeFadeInDuration));

        //서서히 사라짐
        sequenceOwn.Append(canvasGroup.DOFade(0, runTimeFadeOutDuration));

        //무한 반복 설정
        sequenceOwn.SetLoops(-1);
    }
}
