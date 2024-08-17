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
        //자신의 그룹 표시하기 위해 사용
        public abstract GimmickType Type { get; protected set; }
        public abstract float Probability { get; set; }

        protected float timeLimit = 0;

        public abstract void Activate();
        public abstract void Deactivate();
        //플레이어의 몸 움직임, 눈 깜빡임등을 참조하여 기믹 자체 Probability 계산식 실행
        public abstract void UpdateProbability(ExPlayer player);
    }
}

