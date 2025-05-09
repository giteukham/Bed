using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using UnityEngine;

public class Breathing : Gimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    public override void UpdateProbability()
    {
        if (GameManager.Instance.isDemo) probability = 100f;
    }

    public override void Initialize()
    {
    }

    public override void Activate()
    {
        base.Activate();
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.breathingInRoom, transform.position);
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}
