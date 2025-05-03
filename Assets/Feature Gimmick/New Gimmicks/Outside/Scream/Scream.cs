using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using UnityEngine;

public class Scream : Gimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    public override void UpdateProbability()
    {
    }

    public override void Initialize()
    {
    }
    
    private void Start()
    {
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.screamOutside, transform.position);
    }
    
    public override void Activate()
    {
        base.Activate();
        // AudioManager.Instance.PlayOneShot(AudioManager.Instance.screamOutside, transform.position);
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
    }
}
