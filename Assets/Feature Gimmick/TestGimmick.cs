using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;
using System;

/// <summary>
/// 기믹 제작 예시 코드 및 유일하게 gimmickManager 싱글톤 변수 초기화하는 스크립트
/// </summary>
// public class TestGimmick : Gimmick
// {

//     public override GimmickType Type { get; protected set; } = GimmickType.Human;
//     public override float Probability { get; set; } = 100;

//     private void Awake()
//     {
//         //Gimmick 추상클래스의 gimmickManager 싱글톤 변수 초기화(이 스크립트에서만 1회 실행됨)
//         gimmickManager = FindObjectOfType<GimmickManager>();
//         gameObject.SetActive(false);
//     }

//     private void Update()
//     {
//         timeLimit += Time.deltaTime;
//     }

//     public override void Activate()
//     {
//         print("테스트기믹 실행");
//         SettingVariables();
//         StartCoroutine(MainCode());
//     }

//     public override void Deactivate()
//     {
//         gimmickManager.LowerProbability(this);
//         gimmickManager.humanGimmick = null;
//         gameObject.SetActive(false);
//     }

//     public override void UpdateProbability()
//     {
//         //Probability에 대한 계산식
//         Probability = 100;
//     }

//     private IEnumerator MainCode()
//     {
//         //기믹 파훼 제한시간 15초
//         while (timeLimit < 15)
//         {
//             yield return new WaitForSeconds(0.1f);
//             //기믹 파훼 성공시
//             if (false)
//             {
//                 //플레이어에게 데미지 주는거 없이 바로 Deactivate로 넘어감
//                 Deactivate();
//             }
//         }

//         //15초 지나서 기믹 파훼 실패 했을 때
//         //스트레스 지수 혹은 공포 지수를 플레이어에게 접근해서 올림
//         //이후 Deactivate 실행
//         Deactivate();
//     }
// }