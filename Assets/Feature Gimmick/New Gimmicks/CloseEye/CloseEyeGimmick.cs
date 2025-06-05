using System;
using System.Collections;
using System.Collections.Generic;
using AbstractGimmick;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct FaceImageData
{
    public Image faceImage;
    public float startAlpha, endAlpha;
}

public class CloseEyeGimmick : Gimmick
{
    [SerializeField]
    private List<FaceImageData> faceList = new List<FaceImageData>();

    [SerializeField]
    private Camera playerCam;
    
    private Coroutine gimmickCoroutine;
    
    public override GimmickType type { get; protected set; }
    public override float probability { get; set; }
    public override List<Gimmick> ExclusionGimmickList { get; set; }
    public override void UpdateProbability()
    {
    }

    public override void Initialize()
    {
        
    }

    private void Awake()
    {
        faceList.ForEach((x) => x.faceImage.gameObject.SetActive(false));
    }

    public override void Activate()
    {
        base.Activate();
        if (gimmickCoroutine != null)
        {
            StopCoroutine(gimmickCoroutine);
            gimmickCoroutine = null;
        }
        
        faceList.ForEach((x) => x.faceImage.gameObject.SetActive(false));
        gimmickCoroutine = StartCoroutine(StartGimmick());
    }

    public override void Deactivate()
    {
        base.Deactivate();
        faceList.ForEach((x) => x.faceImage.gameObject.SetActive(false));
    }

    private IEnumerator StartGimmick()
    {
        yield return new WaitUntil(() => !PlayerConstant.isEyeOpen);
        
        faceList.ForEach((x) =>
        {
            x.faceImage.color = new Color(1f, 1f, 1f, x.startAlpha);
        });
        
        // 랜덤 이미지
        var randomData = faceList[(int) Random.Range(0f, faceList.Count)];
        var randomImage = randomData.faceImage;

        // 화면 안쪽에 랜덤 한 위치
        randomImage.transform.position =
            playerCam.ViewportToScreenPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.2f, 0.8f), 10f));
        randomImage.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-10f, 10f));
        randomImage.transform.DOShakePosition(9999f, 7f, 100, 5f);
        randomImage.gameObject.SetActive(true);
        
        while (true)
        {
            if (PlayerConstant.isEyeOpen)
            {
                Deactivate();
                yield break;
            }
            
            // 이미지 알파 값을 startAlpha 에서 endAlpha까지 증가
            randomImage.color = new Color(1f, 1f, 1f, 
                Mathf.Clamp(randomImage.color.a + Time.deltaTime, 
                    randomData.startAlpha, 
                    randomData.endAlpha));
            
            yield return null;
        }
    }
}
