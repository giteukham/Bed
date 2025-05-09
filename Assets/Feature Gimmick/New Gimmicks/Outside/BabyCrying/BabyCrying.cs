using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;

public class BabyCrying : SoundOnlyGimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    protected override EventReference soundEvent { get; set; }

    public override void UpdateProbability()
    {
        if (GameManager.Instance.isDemo) probability = 100f;
    }

    public override void Initialize() { }

    private void Start()
    {
        soundEvent = AudioManager.Instance.babyCryingOutside;
    }
}
