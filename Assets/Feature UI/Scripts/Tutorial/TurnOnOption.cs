using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOnOption : MonoBehaviour
{

    private CanvasGroup canvasGroup;
    private float fadeDuration = 0.4f;  // ���̵� ��/�ƿ� �ð�
    private float moveDuration = 1f; // �̵� �ð�
    private float moveOffset = 10f;  // ���� �ö󰡴� �Ÿ�

    void Start()
    {
        // CanvasGroup ��������
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

        //��¦ ���� �̵� (RectTransform�� anchoredPosition ���)
        sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y + moveOffset, moveDuration));

        //������ �����
        sequence.Append(canvasGroup.DOFade(0, fadeDuration));

        //�� ��ġ ������ ����ġ
        //sequence.Append(GetComponent<RectTransform>().DOAnchorPosY(GetComponent<RectTransform>().anchoredPosition.y, 0.01f));
        sequence.OnComplete(() => GetComponent<RectTransform>().anchoredPosition = Vector2.zero);

        //���� �ݺ� ����
        sequence.SetLoops(-1);
    }
}
