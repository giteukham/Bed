using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

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
    
    [SerializeField, Space] 
    private Player player;
    
    [Header("Siren Objects")]
    [SerializeField] private GameObject phone;
    [SerializeField] private GameObject monitor;
    
    private CustomAnimator _customAnimator;
    
    private bool _isPhoneActive = true;
    
    private void Awake()
    {
        _customAnimator = GetComponent<CustomAnimator>();
    }

    private void OnEnable()
    {
        player?.SubscribeConeEnter((col) =>
        {
            if (col.gameObject == phone)
            {
                _isPhoneActive = false;
            }
        });
        
        ChangeSirenObjectsTag("Gimmick");
        _customAnimator.StartAnimation();
    }

    private void OnDisable()
    {
        ChangeSirenObjectsTag("Untagged");
    }

    public async void OnPhoneAnimation()
    {
        await UniTask.WaitUntil(() => _isPhoneActive == false);
        _customAnimator.SetTrigger("Phone Off");
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
}
