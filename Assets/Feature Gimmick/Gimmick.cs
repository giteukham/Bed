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
        //기믹 매니저 참조용 싱글톤 변수
        protected static GimmickManager gimmickManager;

        //자신의 그룹 표시하기 위해 사용
        public abstract GimmickType Type { get; protected set; }
        //기믹 등장 확률
        public abstract float Probability { get; set; }
        //기믹 진행 시간
        protected float timeLimit = 0;

        public bool isDetected = false;

        //기믹 실행될 때
        public abstract void Activate();
        //기믹 끝날 때
        public abstract void Deactivate();
        //플레이어의 몸 움직임, 눈 깜빡임등을 참조하여 기믹 자체 Probability 계산식 실행
        public abstract void UpdateProbability();

        //Activate 실행할때 기본적으로 초기화할 내용들
        protected virtual void SettingVariables()
        {
            gameObject.SetActive(true);
            timeLimit = 0;
            isDetected = false;
        }
    }
}

