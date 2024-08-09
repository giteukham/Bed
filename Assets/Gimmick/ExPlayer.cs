using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExPlayer : MonoBehaviour
{
    /// <summary>
    /// ���� �÷��̾� ��ũ��Ʈ
    /// </summary>
     

    //���⿡ �̺�Ʈ ����� GimmickTest�ʿ��� �̺�Ʈ �����Ұ���
    //�ڷ�ƾ���� 3�ʸ��� �̺�Ʈ �ߵ�����
    //�̷��� �Ǹ� GimmickTest�� percent ���� ���� ����

    //���� ��ġ��(���콺 ������, �������� ���...)
    int mouseMove = 0;
    int eyeBlink = 0;

    //���� ����������, �� �����ŷȴ��� ����(�Ⱦ����� ����)
    bool manyMousemove = false;
    bool manyEyeBlink = false;

    //�̺�Ʈ ���� ����
    public event Action<int, int> percentEvent;

    private void Awake()
    {
        //���� ���۽� �ڷ�ƾ 1ȸ ���� ���� �ݺ�
        StartCoroutine(EventCouroutine());
    }

    //�̺�Ʈ ���� �޼ҵ�
    private void ExcuteEvent()
    {
        //�̺�Ʈ ������ ��� �޼ҵ忡�� ��ġ �ѱ�
        percentEvent?.Invoke(mouseMove, eyeBlink);
    }

    //3�ʸ��� 1ȸ ����Ǵ� �ڷ�ƾ
    IEnumerator EventCouroutine()
    {
        //���ѹݺ�
        while (true)
        {
            //3�ʸ��� ����
            yield return new WaitForSeconds(3);
            //����� ���� �м�
            UserTendencyAnalysis();
            //�̺�Ʈ ����
            ExcuteEvent();
        }

    }

    ///����� ���� �м� �޼ҵ�
    private void UserTendencyAnalysis()
    {
        //���� ���콺 ������, �������� ������ ���ϰ� ���� �޼ҵ�
        print("����� ���� �м�");
    }
}
