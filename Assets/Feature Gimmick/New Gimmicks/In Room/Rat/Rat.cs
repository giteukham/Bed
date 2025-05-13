using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using FMODUnity;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Rat : SoundOnlyGimmick
{
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    protected override EventReference soundEvent { get; set; }

    [SerializeField]
    private Transform floor;
    private Collider floorCollider;

    public override void UpdateProbability()
    {
        if (GameManager.Instance.isDemo) probability = 100f;
    }

    public override void Initialize() { }

    private void Start()
    {
        floorCollider = floor.GetComponent<Collider>();
        soundEvent = AudioManager.Instance.ratInRoom;
    }

    public override void Activate()
    {
        base.Activate();
        var pos = GetRandomPosition(floorCollider.bounds.min, floorCollider.bounds.max);
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        
        AudioManager.Instance.PlaySound(soundEvent, transform.position);
    }
    
    private Vector3 GetRandomPosition(Vector3 min, Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}
