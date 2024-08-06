using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GimmickInterface;

public class GimmickTest : MonoBehaviour, IGimmick
{
    //자신이 어느 리스트에 속해있는지 초기값 바로 지정
    public ListGroup myGroup { get; set; } = ListGroup.Human;

    private void Awake()
    {
        //위에 적은 초기값 바로 지정하는 방식 오류 없으면 이거 그냥 지워도 무관
        //myGroup = ListGroup.Human;

    }

    //기믹 끝났을 때 맨 마지막 마무리
    public void OnEnd()
    {
        //GimmickManager 쪽에 끝났다는 신호주고 제외 되었던 본인소속 리스트를
        //다시 GimmickList에 넣어야 함
    }

    //기믹이 처음 시작할 때
    public void OnStart()
    {
     //이건 외부 접근해서 OnStart가 실행되게 하거나
     //아니면 OnEnable같은거로 실행시켜도 될 듯
    }

    //기믹이 시작하고 있는 도중에
    public void OnUpdate()
    {
        //모든 코드 다 실행되면 OnEnd 실행
        OnEnd();
    }

}
