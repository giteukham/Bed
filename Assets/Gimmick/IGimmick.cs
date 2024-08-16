using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GimmickInterface
{
    public enum ListGroup
    {
        Unreal,
        Human,
        Object
    }

    public interface IGimmick
    {
        //�ڽ��� �׷� ǥ���ϱ� ���� ���
        ListGroup myGroup { get; set; }

        //�������̽����� �Ϲ� ���� ���� �Ұ��ϹǷ� ������Ƽ�� ��ü
        int percent { get; set; }

        //�÷��̾� ��ũ��Ʈ ������ ���� ����
        ExPlayer Player { get; set; }

        //����� gameObject�� �����ϱ� ���� ����
        GameObject gimmickObject {  get; set; }

        void OnStart();

        void OnUpdate();

        void OnEnd();

        //��� ���� Ȯ�� ������ �޼���
        void PercentRedefine(bool mouseMove, bool eyeBlink);
        //���� �ۼ�Ʈ�� ���缭 Ÿ�� ����Ʈ�� ��� �ִ� �޼���
        void InsertIntoListUsingPercent(int randomNum);
    }
}

