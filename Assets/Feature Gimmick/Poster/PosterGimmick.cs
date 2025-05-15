
using System;
using System.Collections.Generic;
using System.Threading;
using AbstractGimmick;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PosterGimmick : Gimmick
{
    private enum PosterLevel
    {
        Level1,
        Level2,
        Level3
    }
    
    [FormerlySerializedAs("timeToActive")]
    [SerializeField, Tooltip("오른쪽 또는 왼쪽을 얼마나 쳐다봐야 기믹이 활성화 되는지")]
    private float timeToPlay = 3f;
    
    [SerializeField] 
    private Transform soundPos;
    
    [SerializeField] 
    private GameObject posters;
    
    private Collider gimmickCollider;
    private CancellationTokenSource cts = new CancellationTokenSource();
    
    private Queue<GameObject> activatedPosters = new();
    private PosterLevel posterLevel = PosterLevel.Level1;
    
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    public override void UpdateProbability()
    {
        probability = 100;
    }

    public override void Activate()
    {
        base.Activate();
    }

    private void Start()
    {
        Debug.Log("포스터 기믹 활성화");
        gimmickCollider = GetComponent<Collider>();
        PlayPosterGimmick();
    }

    private async UniTaskVoid PlayPosterGimmick()
    {
        for (int i = 0; i < posters.transform.childCount; i++)
        {
            posters.transform.GetChild(i).gameObject.SetActive(false);
            for (int j = 0; j < posters.transform.GetChild(i).childCount; j++)
            {
                posters.transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
            }
        }
        
        if (posters.activeSelf) posters.SetActive(false);
        if (gimmickCollider.enabled) gimmickCollider.enabled = false;
        
        await UniTask.WaitUntil(() => PlayerConstant.RightLookLAT > timeToPlay || PlayerConstant.LeftLookLAT > timeToPlay);

        gimmickCollider.enabled = true;
        posters.SetActive(true);

        // State 1
        if (await PlayPosterStage1(AudioManager.Instance.poster1, 1))              // Level 1. 포스터 1장 랜덤 소환 
        {
            posterLevel = PosterLevel.Level1;
            goto Stage2; // 성공 시
        }

        if (await PlayPosterStage1(AudioManager.Instance.poster2, 3))              // Level 2. 포스터 3장 랜덤 소환
        {
            posterLevel = PosterLevel.Level2;
            goto Stage2; // 성공 시
        }

        await PlayPosterStage1(AudioManager.Instance.poster3, posters.transform.childCount);        // Level 3. 포스터 전체 소환
        posterLevel = PosterLevel.Level3;
        
        Stage2:
        await UniTask.WaitForSeconds(3);

        // State 2
        // 소리 여러 개 작게 들리다가 3초 안에 눈 감으면 멈추면서 포스터 삭제
        // 눈 뜨고 있으면 계속 들림


        var count = activatedPosters.Count;
        // Blood 활성화
        for (var i = 0; i < count; i++)
        {
            if (activatedPosters.TryDequeue(out var poster))
            {
                for (int j = 0; j < poster.transform.childCount; j++)
                {
                    poster.transform.GetChild(j).gameObject.SetActive(true);
                }
            }
        }
        
        AudioManager.Instance.PlaySound(AudioManager.Instance.cry2, soundPos.position);

        if (posterLevel == PosterLevel.Level1) goto Finish;
        
        AudioManager.Instance.PlaySound(AudioManager.Instance.scream2, soundPos.position);
        
        if (posterLevel == PosterLevel.Level2) goto Finish;
        
        AudioManager.Instance.PlaySound(AudioManager.Instance.cry1, soundPos.position);

        Finish:
        if (await PlayPosterStage2())
        {
            // 성공 시 모든 포스터 삭제
            AudioManager.Instance.StopAllSounds(STOP_MODE.IMMEDIATE);
            posters.SetActive(false);
        }
    }

    private async UniTask<bool> PlayPosterStage1(EventReference eventRef, int imageToActivateCount)
    {
        AudioManager.Instance.PlaySound(eventRef, soundPos.position);
        
        List<int> inactiveIndices = new List<int>();
        for (int i = 0; i < posters.transform.childCount; i++)
        {
            if (!posters.transform.GetChild(i).gameObject.activeSelf)
            {
                inactiveIndices.Add(i);
            }
        }
    
        int actualCount = Mathf.Min(imageToActivateCount, inactiveIndices.Count);
    
        for (int i = 0; i < actualCount; i++)
        {
            var randomIndex = Random.Range(0, inactiveIndices.Count);
            var posterIndex = inactiveIndices[randomIndex];
            var poster = posters.transform.GetChild(posterIndex).gameObject;
            
            poster.SetActive(true);
            activatedPosters.Enqueue(poster);
            inactiveIndices.RemoveAt(randomIndex);
        }
        
        await UniTask.WaitWhile(() =>
        {
            if (isDetected)
            {
                AudioManager.Instance.StopSound(eventRef, STOP_MODE.IMMEDIATE);
                cts.Cancel();
                return false;
            }
            
            return AudioManager.Instance.GetPlaybackState(eventRef) == PLAYBACK_STATE.PLAYING ||
                   AudioManager.Instance.GetPlaybackState(eventRef) == PLAYBACK_STATE.STARTING;
        }, cancellationToken: cts.Token);

        return isDetected;
    }

    private async UniTask<bool> PlayPosterStage2()
    {
        int prev = PlayerConstant.EyeBlinkCAT;
        float timer = 0f;
        
        while (cts.IsCancellationRequested == false)
        {
            timer += Time.deltaTime;
            
            // 눈을 깜박이면 파회
            if (prev != PlayerConstant.EyeBlinkCAT)
            {
                return true;
            }
    
            // 3초 안에 눈을 깜박이지 않으면 실패
            if (timer >= 3f)
            {
                cts.Cancel();
                return false;
            }

            await UniTask.Yield();
        }
        
        return false;
    }
    
    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void Initialize()
    {
        throw new System.NotImplementedException();
    }
    
    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
