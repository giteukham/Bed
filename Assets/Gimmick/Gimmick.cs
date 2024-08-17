using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GimmickInterface
{
    public enum GimmickType
    {
        Unreal,
        Human,
        Object
    }

    public abstract class Gimmick : MonoBehaviour
    {
        //�ڽ��� �׷� ǥ���ϱ� ���� ���
        public abstract GimmickType Type { get; protected set; }
        public abstract float Probability { get; set; }

        protected float timeLimit = 0;

        public abstract void Activate();
        public abstract void Deactivate();
        //�÷��̾��� �� ������, �� �����ӵ��� �����Ͽ� ��� ��ü Probability ���� ����
        public abstract void UpdateProbability(ExPlayer player);
    }
}

