using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using FMOD.Studio;
using Unity.VisualScripting;
using UnityEngine;

public class PosterGimmick : Gimmick
{
    [SerializeField] 
    private Transform soundPos;
    
    [SerializeField] 
    private GameObject posters;
    
    private Collider gimmickCollider;
    
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Activate()
    {
        base.Activate();
    }

    private void Start()
    {
        Debug.Log("포스터 기믹 활성화");
        gimmickCollider = GetComponent<Collider>();
        ActivePosterGimmick();
    }

    private async UniTaskVoid ActivePosterGimmick()
    {
        if (posters.activeSelf) posters.SetActive(false);
        if (gimmickCollider.enabled) gimmickCollider.enabled = false;
        
        await UniTask.WaitUntil(() => PlayerConstant.RightLookLAT > 2f || PlayerConstant.LeftLookLAT > 2f);
        AudioManager.Instance.PlaySound(AudioManager.Instance.poster, soundPos.position);
        
        gimmickCollider.enabled = true;
        posters.SetActive(true);

        await UniTask.WaitUntil(() => isDetected);
        AudioManager.Instance.StopSound(AudioManager.Instance.poster, STOP_MODE.IMMEDIATE);
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }
}
