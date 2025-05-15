using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;

public class Radio : SoundOnlyGimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    protected override EventReference soundEvent { get; set; }

    public override void UpdateProbability()
    {
    }
    
    private void Start()
    {
        soundEvent = AudioManager.Instance.radioInHouse;
    }

    public override void Initialize() { }

}
