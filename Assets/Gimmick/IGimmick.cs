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

        void OnStart();

        void OnUpdate();

        void OnEnd();
    }
}

