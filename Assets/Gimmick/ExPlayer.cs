using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//�����ٶ󸶹ٻ�� �� �� �� ��
//�� �� �� �� �� �� ��
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
    //���� bool������ ��, ���� 2���� �������� ��� ���� Ȯ���� �����ִµ�
    //��Ȳ�� ���� �߰����� �ʿ��غ���
    //enum ���� �ϳ� ���� ���°͵� ������ �ʾƺ���
    bool manyMousemove = false;
    bool manyEyeBlink = false;

    //�̺�Ʈ ���� ����
    public event Action<bool, bool> TendencyDataEvent;

    private void Awake()
    {
        //���� ���۽� �ڷ�ƾ 1ȸ ���� ���� �ݺ�
        StartCoroutine(EventCouroutine());
    }

    private void Update()
    {
        //���� ���콺 ��Ű Ŭ��, ���콺 ������ �޾Ƽ� ������ �ִ� �ڵ�
        //��Ű ���������� eyeBlink ������ �ְ�(Ȥ�� �� ��ũ�� ��������)

        //���콺 ��ǥ�� ���� ��ġ�� ���Ͽ� �󸶳� ���������� Ȥ�� x��ǥ�� y��ǥ�� ���� ��ǥ�� �ݴ� ������ �Ǿ��ִ���
        //���� �Ǵ��Ͽ� mouseMove�� ����(�ƴϸ� �ٸ� ����� �ᵵ ��)
    }

    //�̺�Ʈ ���� �޼ҵ�
    private void ThrowTendencyData()
    {
        //�̺�Ʈ ������ ��� �޼ҵ忡�� ��ġ �ѱ�
        TendencyDataEvent?.Invoke(manyMousemove, manyEyeBlink);
    }

    //3�ʸ��� 1ȸ ����Ǵ� �ڷ�ƾ
    IEnumerator EventCouroutine()
    {
        //���ѹݺ�
        while (true)
        {
            //3�ʸ��� ����
            yield return new WaitForSeconds(3);
            //����� ���� �м�(���콺 ���� ������ ���� ����)
            UserTendencyAnalysis(mouseMove, eyeBlink);
            //���� ���� ���� �ѱ�(��͵��� ��� ������ ����Ȯ���� ������ �ϰԵ�)
            ThrowTendencyData();
        }

    }

    ///����� ���� �м� �޼ҵ�
    private void UserTendencyAnalysis(int mouseMove, int eyeBlink)
    {
        //���� ���콺 ������, �������� ������ ���ϰ� ���� �޼ҵ�
        print("����� ���� �м�");

        //�ϴ� ���ؼ�ġ 50���� ����, 3�ʸ��� üũ�ϴ°Ŷ� �����δ� ���� �� ��������
        //�ƴϸ� 30�ʸ��� üũ�ϰų�
        //����� ������, Ŭ�� ���� ���ġ ���� ���ؼ�ġ ���ϴ� �͵� �����ƺ���
        if (mouseMove > 50)
        {
            manyMousemove = true;
        }
        else
        {
            manyMousemove= false;
        }

        if (eyeBlink > 50)
        {
            manyEyeBlink = true;
        }
        else
        {
            manyEyeBlink= false;
        }

    }
}
