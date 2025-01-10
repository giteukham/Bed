using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using AbstractGimmick;
using Bed.Collider;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SirenGimmick : Gimmick
{
    #region Override Variables
    [field: SerializeField] public override GimmickType type { get; protected set; }
    [SerializeField] private float _probability;
    public override float probability 
    { 
        get => _probability; 
        set => _probability = Mathf.Clamp(value, 0, 100); 
    }
    [field: SerializeField] public override List<Gimmick> ExclusionGimmickList { get; set; }
    #endregion
    
    [Header("Siren Objects")]
    [SerializeField] private GameObject phone;
    [SerializeField] private GameObject monitor;
    
    private GimmickSequence gimmickSequence;
    
    private CancellationTokenSource gimmickCts = new();
    private CancellationTokenSource triggerCts = new();
    
    private void Awake()
    {
        gimmickSequence = GetComponent<GimmickSequence>();
    }

    private void OnEnable()
    {
        ChangeSirenObjectsTag("Gimmick");
        gimmickSequence.StartGimmick();
    }

    private void OnDisable()
    {
        ChangeSirenObjectsTag("Untagged");
    }

    public void OnPhoneTriggerEvent()
    {
        phone.GetAsyncTriggerEnterTrigger()
            .Subscribe(DetectPhone)
            .AddTo(triggerCts.Token);
        
        phone.GetAsyncTriggerExitTrigger()
            .Subscribe(coll => ResetToken(ref gimmickCts))
            .AddTo(triggerCts.Token);
    }

    private async UniTaskVoid DetectPhone(Collider col)
    {
        if (!col.gameObject.CompareTag("SightRange")) return;

        var isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: gimmickCts.Token)
            .SuppressCancellationThrow();
        
        if (!isCanceled)
        {
            gimmickSequence.SetTrigger("Phone Off");
            ResetToken(ref triggerCts);
        }
    }

    private void ResetToken(ref CancellationTokenSource token)
    {
        token.Cancel();
        token = new CancellationTokenSource();
    }

    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Initialize() { }

    public override void Activate()
    {
        base.Activate();

    }

    private void ChangeSirenObjectsTag(string tagName)
    {
        if (phone != null) phone.tag = tagName;
    }

    private void OnDestroy()
    {
        gimmickCts?.Dispose();
    }
}
