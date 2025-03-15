using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMove : Tutorial
{
    [SerializeField] private float runTimeFadeDuration = 0.2f;  // ���̵� ��/�ƿ� �ð�
    [SerializeField] private float moveDuration = 0.7f; // �̵� �ð�
    [SerializeField] private float moveOffset = -300f;  // �������� ���� �Ÿ�

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

        //������ ��Ÿ��
        sequenceOwn.Append(canvasGroup.DOFade(1, runTimeFadeDuration));

        //��ǥ�� �̵� (RectTransform�� anchoredPosition ���)
        sequenceOwn.Append(GetComponent<RectTransform>().DOAnchorPosX(GetComponent<RectTransform>().anchoredPosition.x + moveOffset, moveDuration));

        //������ �����
        sequenceOwn.Append(canvasGroup.DOFade(0, runTimeFadeDuration));

        //���콺 ��ġ ������ ����ġ
        sequenceOwn.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //���� �ݺ� ����
        sequenceOwn.SetLoops(-1);

    }
}
