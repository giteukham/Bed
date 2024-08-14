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
        ListGroup myGroup { get; set; }

        //�������̽����� �Ϲ� ���� ���� �Ұ��ϹǷ� ������Ƽ�� ��ü
        int percent { get; set; }

        //�÷��̾� ��ũ��Ʈ ������ ���� ����(ExPlayer�� �ٸ��ɷ� ��ü�ɰ���)
        ExPlayer Player { get; set; }

        //����� gameObject�� �����ϱ� ���� ����
        GameObject gimmickObject {  get; set; }

        void OnStart();

        void OnUpdate();

        void OnEnd();

        //�̺�Ʈ ������ �޼���
        void PercentRedefine(bool mouseMove, bool eyeBlink);
    }
}

