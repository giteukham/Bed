using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftMove : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;  // ���̵� ��/�ƿ� �ð�
    [SerializeField] private float moveDuration = 0.6f; // �̵� �ð�
    [SerializeField] private float moveOffset = -300f;  // �������� ���� �Ÿ�

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        //ó���� �Ⱥ��̰�
        canvasGroup.DOFade(0, 0.01f);

        PlayEffect();
    }

    private void PlayEffect()
    {
        Sequence sequence = DOTween.Sequence();

        //������ ��Ÿ��
        sequence.Append(canvasGroup.DOFade(1, fadeDuration));

        //��ǥ�� �̵� (RectTransform�� anchoredPosition ���)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPosX(GetComponent<RectTransform>().anchoredPosition.x + moveOffset, moveDuration));

        //������ �����
        sequence.Append(canvasGroup.DOFade(0, fadeDuration));

        //���콺 ��ġ ������ ����ġ
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //���� �ݺ� ����
        sequence.SetLoops(-1);
    }
}
