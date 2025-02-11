using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMove : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private float fadeDuration = 0.4f;  // 페이드 인/아웃 시간
    private float moveDuration = 0.6f; // 이동 시간
    [SerializeField] private RectTransform target;

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
        sequence.Append(canvasGroup.DOFade(1, fadeDuration));

        //목표로 이동 (RectTransform의 anchoredPosition 사용)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPos(target.anchoredPosition, moveDuration));

        //서서히 사라짐
        sequence.Append(canvasGroup.DOFade(0, fadeDuration));

        //마우스 위치 빠르게 원위치
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //무한 반복 설정
        sequence.SetLoops(-1);
    }
}
