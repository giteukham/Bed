using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BothClick : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float fadeInDuration = 0.1f;  // 페이드 인
    private float fadeOutDuration = 0.3f;  // 페이드 아웃

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        //처음에 안보이게
        canvasGroup.DOFade(0, 0.01f);

        PlayEffect();
    }

    private void PlayEffect()
    {
        Sequence sequence = DOTween.Sequence();

        //서서히 나타남
        sequence.Append(canvasGroup.DOFade(1, fadeInDuration));

        //서서히 사라짐
        sequence.Append(canvasGroup.DOFade(0, fadeOutDuration));

        //무한 반복 설정
        sequence.SetLoops(-1);
    }
}
