using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOpen : Tutorial
{
    [SerializeField] private float runTimeFadeDuration = 0.2f;  // 페이드 인/아웃 시간
    [SerializeField] private float moveDuration = 0.6f; // 이동 시간
    [SerializeField] private float moveOffset = 10f;  // 위로 올라가는 거리

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
        sequenceOwn.Append(canvasGroup.DOFade(1, runTimeFadeDuration));

        //살짝 위로 이동 (RectTransform의 anchoredPosition 사용)
        sequenceOwn.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + moveOffset, moveDuration));

        //서서히 사라짐
        sequenceOwn.Append(canvasGroup.DOFade(0, runTimeFadeDuration));

        //휠 위치 빠르게 원위치
        //sequenceOwn.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y, 0.01f));
        sequenceOwn.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //무한 반복 설정
        sequenceOwn.SetLoops(-1);
    }
}
