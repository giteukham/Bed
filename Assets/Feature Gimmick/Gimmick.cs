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
        public abstract GimmickType type { get; protected set; }

        public abstract float probability { get; set; }
        protected float timeLimit = 0;
        public bool isDetected = false;

        public abstract List<Gimmick> ExclusionGimmickList { get; set; }

        public virtual void Activate()
        {
            timeLimit = 0;
            isDetected = false;
            PlayerConstant.ResetLATStats();
        }

        public virtual void Deactivate()
        {
            GimmickManager.Instance.ResetDeactivateGimmick(gameObject.GetComponent<Gimmick>());
            Initialize();
        }

        public abstract void UpdateProbability();

        public abstract void Initialize();
    }
}

