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
        //�ڽ��� �׷� ǥ���ϱ� ���� ���
        public abstract GimmickType Type { get; protected set; }

        //��� ���� Ȯ��
        public abstract float Probability { get; set; }

        //��� ���� �ð�
        protected float timeLimit = 0;

        public bool isDetected = false;

        /// <summary>
        /// ��� Ȱ��ȭ �� �� (������Ʈ Ȱ��ȭ, �ð� �ʱ�ȭ, ���� ���� �ʱ�ȭ)
        /// </summary>
        public virtual void Activate()
        {
            gameObject.SetActive(true);
            timeLimit = 0;
            isDetected = false;
        }

        /// <summary>
        /// ��� ��Ȱ��ȭ �� �� (������Ʈ ��Ȱ��ȭ, ���� �ʱ�ȭ)
        /// </summary>
        public virtual void Deactivate()
        {
            GimmickManager.instance.ResetDeactivateGimmick(gameObject.GetComponent<Gimmick>());
            Initialize();
        }
        
        //�÷��̾��� �� ������, �� �����ӵ��� �����Ͽ� ��� ��ü Probability ���� ����
        public abstract void UpdateProbability();

        // ���� �ʱ�ȭ ����
        public abstract void Initialize();
    }
}

