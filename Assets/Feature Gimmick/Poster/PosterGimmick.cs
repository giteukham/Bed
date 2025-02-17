
using System;
using System.Collections.Generic;
using System.Threading;
using AbstractGimmick;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Random = UnityEngine.Random;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PosterGimmick : Gimmick
{
    [SerializeField, Tooltip("오른쪽 또는 왼쪽을 얼마나 쳐다봐야 기믹이 활성화 되는지")]
    private float timeToActive = 3f;
    
    [SerializeField] 
    private Transform soundPos;
    
    [SerializeField] 
    private GameObject posters;
    
    private Collider gimmickCollider;
    
    private CancellationTokenSource cts = new CancellationTokenSource();

    private AsyncReactiveProperty<int> blinkCount = new AsyncReactiveProperty<int>(0);
    
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
        }
        
        if (posters.activeSelf) posters.SetActive(false);
        if (gimmickCollider.enabled) gimmickCollider.enabled = false;
        
        await UniTask.WaitUntil(() => PlayerConstant.RightLookLAT > timeToActive || PlayerConstant.LeftLookLAT > timeToActive);

        gimmickCollider.enabled = true;
        posters.SetActive(true);

        if (await PlayPosterStage1(AudioManager.Instance.poster1, 1))              // 1단계. 포스터 1장 랜덤 소환 
        {
            return; // 성공 시?
        }

        if (await PlayPosterStage1(AudioManager.Instance.poster2, 3))              // 2단계. 포스터 3장 랜덤 소환
        {
            return; // 성공 시?
        }          
        
        if (await PlayPosterStage1(AudioManager.Instance.poster3, posters.transform.childCount))    // 3단계. 포스터 전체 소환
        {
            return; // 성공 시?
        }
        
        await UniTask.WaitForSeconds(3);
        Debug.Log("Stage 2 Start");

        if (await PlayPosterStage2() == false)
        {
            Debug.Log("실패");
            //AudioManager.Instance.PlaySound(AudioManager.Instance.scream, soundPos.position);
        }
        else
        {
            Debug.Log("성공");
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
            int randomIndex = Random.Range(0, inactiveIndices.Count);
            int posterIndex = inactiveIndices[randomIndex];
            posters.transform.GetChild(posterIndex).gameObject.SetActive(true);
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
