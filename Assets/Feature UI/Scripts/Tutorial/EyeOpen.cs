using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeOpen : Tutorial
{
    [SerializeField] private float runTimeFadeDuration = 0.2f;  // ���̵� ��/�ƿ� �ð�
    [SerializeField] private float moveDuration = 0.6f; // �̵� �ð�
    [SerializeField] private float moveOffset = 10f;  // ���� �ö󰡴� �Ÿ�

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

        //��¦ ���� �̵� (RectTransform�� anchoredPosition ���)
        sequenceOwn.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + moveOffset, moveDuration));

        //������ �����
        sequenceOwn.Append(canvasGroup.DOFade(0, runTimeFadeDuration));

        //�� ��ġ ������ ����ġ
        //sequenceOwn.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y, 0.01f));
        sequenceOwn.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //���� �ݺ� ����
        sequenceOwn.SetLoops(-1);
    }
}
