using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using UnityEngine;
using UnityEngine.Serialization;

public class MirrorGimmick : Gimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    
    [SerializeField]
    private GameObject monsterObj;
    private Coroutine mirrorCoroutine;

    private void Awake()
    {
        if (monsterObj.activeSelf == true) monsterObj.SetActive(false);
        
        mirrorCoroutine = StartCoroutine(PlayMirrorGimmick());

    }

    public override void UpdateProbability()
    {
    }

    public override void Initialize()
    {
    }

    public override void Activate()
    {
        base.Activate();
        mirrorCoroutine = StartCoroutine(PlayMirrorGimmick());
    }

    private IEnumerator PlayMirrorGimmick()
    {
        yield return new WaitUntil(() => isDetected);
        monsterObj.SetActive(true);
        
        yield return new WaitUntil(() => !isDetected);
        monsterObj.SetActive(false);
        Deactivate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
        
        if (mirrorCoroutine != null)
        {
            StopCoroutine(mirrorCoroutine);
            mirrorCoroutine = null;
        }
    }
}
