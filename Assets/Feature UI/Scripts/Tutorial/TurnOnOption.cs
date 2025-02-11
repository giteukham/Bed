using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOption : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private float fadeDuration = 0.4f;  // 페이드 인/아웃 시간
    private float moveDuration = 1f; // 이동 시간
    private float moveOffset = 10f;  // 위로 올라가는 거리

    void Start()
    {
        // CanvasGroup 가져오기
        canvasGroup = GetComponent<CanvasGroup>();

        //처음에 안보이게
        canvasGroup.DOFade(0, 0.01f);

        PlayEffect();
    }

    private void PlayEffect()
    {
        Sequence sequence = DOTween.Sequence();

        //서서히 나타남
        sequence.Append(canvasGroup.DOFade(1, fadeDuration));

        //살짝 위로 이동 (RectTransform의 anchoredPosition 사용)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + moveOffset, moveDuration));

        //서서히 사라짐
        sequence.Append(canvasGroup.DOFade(0, fadeDuration));

        //휠 위치 빠르게 원위치
        //sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y, 0.01f));
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //무한 반복 설정
        sequence.SetLoops(-1);
    }
}
