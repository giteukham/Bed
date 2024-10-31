using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbstractGimmick
{
    public enum GimmickType
    {
        Unreal,
        Human,
        Object
    }

    public abstract class Gimmick : MonoBehaviour
    {
        //��� �Ŵ��� ������ �̱��� ����
        protected static GimmickManager gimmickManager;

        //�ڽ��� �׷� ǥ���ϱ� ���� ���
        public abstract GimmickType Type { get; protected set; }
        //��� ���� Ȯ��
        public abstract float Probability { get; set; }
        //��� ���� �ð�
        protected float timeLimit = 0;

        public bool isDetected = false;

        //��� ����� ��
        public abstract void Activate();
        //��� ���� ��
        public abstract void Deactivate();
        //�÷��̾��� �� ������, �� �����ӵ��� �����Ͽ� ��� ��ü Probability ���� ����
        public abstract void UpdateProbability();

        //Activate �����Ҷ� �⺻������ �ʱ�ȭ�� �����
        protected virtual void SettingVariables()
        {
            gameObject.SetActive(true);
            timeLimit = 0;
            isDetected = false;
        }
    }
}

