using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;

public class FemaleLaughing : SoundOnlyGimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    protected override string eventKey { get; set; }
    protected override Guid eventGuid { get; set; }

    public override void UpdateProbability()
    {
    }

    private void Start()
    {
        eventKey = AudioKeys.FemaleLaughingInHouse;
    }

    public override void Initialize() { }
}
