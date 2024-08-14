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

        //인터페이스에서 일반 변수 선언 불가하므로 프로퍼티로 대체
        int percent { get; set; }

        //플레이어 스크립트 접근을 위한 변수(ExPlayer는 다른걸로 교체될거임)
        ExPlayer Player { get; set; }

        //기믹의 gameObject에 접근하기 위한 변수
        GameObject gimmickObject {  get; set; }

        void OnStart();

        void OnUpdate();

        void OnEnd();

        //이벤트 구독용 메서드
        void PercentRedefine(bool mouseMove, bool eyeBlink);
    }
}

