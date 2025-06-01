using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;

public class Scream : SoundOnlyGimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    protected override string eventKey { get; set; }
    protected override Guid eventGuid { get; set; }

    public override void UpdateProbability()
    {
        probability = 100f;
    }

    public override void Initialize() { }
    
    private void Start()
    {
        eventKey = AudioKeys.ScreamOutside;
    }
}
