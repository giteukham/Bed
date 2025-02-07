using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbstractGimmick;

public class TestGimmick : Gimmick
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

    private void Awake()
    {
       // gameObject.SetActive(false);
    }

    public override void Activate()
    {
        base.Activate();
        //StartCoroutine(MainCode());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        //gameObject.SetActive(false);
    }


    public override void UpdateProbability()
    {

    }

    public override void Initialize(){}

    // private IEnumerator MainCode()
    // {
    // }
}
