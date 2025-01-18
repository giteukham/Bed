using System;
using System.Collections.Generic;
using System.Threading;
using AbstractGimmick;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MeteorGimmick : Gimmick
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

    [Space]
    [SerializeField] private CinemachineVirtualCamera playerVCam;
    
    [Header("Gimmick Items")]
    [SerializeField] private GameObject phone;
    [SerializeField] private GameObject screen;
    [SerializeField] private GameObject meteor;
    [SerializeField] private NoiseSettings shakeSettings;
    
    [Space]
    [SerializeField] private Transform meteorSpawnPoint, meteorEndPoint;

    [Space] 
    [Header("Fractured Objects")] 
    [SerializeField] private GameObject originHouse;
    [SerializeField] private GameObject fracturedHouse;
    
    [Space]
    [SerializeField] private GameObject[] fracturedObjects;

    private GameObject chunksParent;
    
    private SequenceController sequenceController;
    
    private CancellationTokenSource gimmickCts = new();
    private CancellationTokenSource triggerCts = new();

    public bool isSpawnMeteor = false;
    
    private void Awake()
    {
        sequenceController = GetComponent<SequenceController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isSpawnMeteor)
        {
            SpawnMeteor();
        }
    }

    private void OnEnable()
    {
        originHouse.SetActive(false);
        fracturedHouse.SetActive(true);
        
        chunksParent = new GameObject("Chunks Parent");
        
        foreach (var obj in fracturedObjects)
        {
            Rigidbody rigid = obj.AddComponent<Rigidbody>();
            rigid.mass = 10f; 
            
            obj.AddComponent<FixedJoint>();                         // TOOD: 왜 FixedJoint 추가 후 값 수정하면 없어짐?
            
            MeshCollider col = obj.AddComponent<MeshCollider>();
            col.convex = true;
            
            AddFractureComponent(obj);
        }
        
        ChangeSirenObjectsTag("Gimmick");
        sequenceController.StartGimmick();
    }

    private void OnDisable()
    {
        ChangeSirenObjectsTag("Untagged");
    }

    #region Phone
    public void OnPhoneVibrateSequence()
    {
        AudioManager.Instance.PlaySound(AudioManager.Instance.phoneVibrate, phone.transform.position);
        
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
            Debug.Log("Phone Detected");
            AudioManager.Instance.StopSound(AudioManager.Instance.phoneVibrate, STOP_MODE.IMMEDIATE);
            sequenceController.SetTrigger("Phone");
            ResetToken(ref triggerCts);
        }
    }
    #endregion

    #region Phone Siren

    public async void OnPhoneSirenSequence()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        screen.SetActive(true);
        //AudioManager.Instance.PlaySound(AudioManager.Instance.siren, phone.transform.position);
        sequenceController.SetTrigger("Screen");
        Debug.Log("Screen");
    }
    
    #endregion
    
    #region Meteor
    
    public async void OnMeteorSequence()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        DoCameraNoise();
        SpawnMeteor();
    }

    private async void DoCameraNoise()
    {
        var noise = playerVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float time = 0f;
        noise.m_NoiseProfile = shakeSettings;

        noise.m_FrequencyGain = 3f;
        await UniTask.WaitUntil(() =>
        {
            time += Time.deltaTime;
            noise.m_AmplitudeGain = Mathf.Lerp(0, 1, time / 10);
            return time >= 10;
        });
    }

    private void SpawnMeteor()
    {
        GameObject obj = Instantiate(meteor, meteorSpawnPoint.position, Quaternion.identity);
        obj.transform.DOMove(meteorEndPoint.position, 10f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            Destroy(obj);
            sequenceController.SetTrigger("Meteor");
        });
    }
    
    #endregion
    
    private void AddFractureComponent(GameObject obj)
    {
        var fracture = obj.AddComponent<Fracture>();
        fracture.InitSettings(null, 3f, 1f, 10);
        fracture.GenerateChunk(chunksParent);
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
        if (screen != null) screen.tag = tagName;
        if (meteor != null) meteor.tag = tagName;
    }

    private void OnDestroy()
    {
        triggerCts?.Cancel();
        triggerCts?.Dispose();
        gimmickCts?.Cancel();
        gimmickCts?.Dispose();
    }
}
