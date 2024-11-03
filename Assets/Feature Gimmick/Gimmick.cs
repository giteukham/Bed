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
        // 자신의 그룹 표시하기 위해 사용
        public abstract GimmickType type { get; protected set; }

        // 기믹 등장 확률
        public abstract float probability { get; set; }

        // 기믹 진행 시간
        protected float timeLimit = 0;

        // 발견 됐는지 여부
        public bool isDetected = false;

        public abstract List<Gimmick> ExclusionGimmickList { get; set; }

        /// <summary>
        /// 기믹 활성화 될 때 (오브젝트 활성화, 시간 초기화, 감지 여부 초기화)
        /// </summary>
        public virtual void Activate()
        {
            gameObject.SetActive(true);
            timeLimit = 0;
            isDetected = false;
        }

        /// <summary>
        /// 기믹 비활성화 될 때 (오브젝트 비활성화, 변수 초기화)
        /// </summary>
        public virtual void Deactivate()
        {
            GimmickManager.instance.ResetDeactivateGimmick(gameObject.GetComponent<Gimmick>());
            Initialize();
        }
        
        // 플레이어의 몸 움직임, 눈 깜빡임등을 참조하여 기믹 자체 Probability 계산식 실행
        public abstract void UpdateProbability();

        // 변수 초기화 내용
        public abstract void Initialize();
    }
}

