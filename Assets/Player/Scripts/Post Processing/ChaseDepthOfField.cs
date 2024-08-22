using Cinemachine.PostFX;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


public class ChaseDepthOfField : MonoBehaviour
{
    [SerializeField] private GameObject postProcessCamera;
    private PostProcessVolume postProcessVolume;
    private DepthOfField depthOfField;

    private float focusValue = 0.1f;
    private float focusSpeed = 0.2f;
    private float minForcusDistance = 0.4f;

    private CancellationTokenSource focusCancelToken = new CancellationTokenSource();
    
    private GameObject currentFocusedObj;

    private void Awake()
    {
       postProcessCamera.gameObject.SetActive(false);
    }

    private void Start()
    {
        postProcessCamera.TryGetComponent(out PostProcessVolume postProcessVolume);
        postProcessVolume.profile.TryGetSettings(out depthOfField);
    }

    private void Update()
    {
        Debug.Log(currentFocusedObj);
    }

    public async void FocusTarget(GameObject focusingObj, Bed.Collider.ConeCollider.TriggerType triggerType)
    {
        StopUniTask();
        
        switch (triggerType)
        {
            case Bed.Collider.ConeCollider.TriggerType.Enter:
                if (currentFocusedObj != null)
                {
                    await FocusOut(currentFocusedObj);
                    currentFocusedObj.layer = LayerMask.NameToLayer("Default");
                    currentFocusedObj = focusingObj;
                    currentFocusedObj.layer = LayerMask.NameToLayer("Focused");
                    await FocusIn(currentFocusedObj);
                }
                else
                {
                    currentFocusedObj = focusingObj;
                    currentFocusedObj.layer = LayerMask.NameToLayer("Focused");
                    postProcessCamera.gameObject.SetActive(true);
                    await FocusIn(currentFocusedObj);
                }
            break;
            case Bed.Collider.ConeCollider.TriggerType.Exit:
                if (currentFocusedObj != null)
                {
                    await FocusOut(currentFocusedObj);
                    currentFocusedObj.layer = LayerMask.NameToLayer("Default");
                    currentFocusedObj = null;
                    postProcessCamera.gameObject.SetActive(false);
                }
            break;
        }
               
    }
    
    private async UniTask FocusIn(GameObject enteredObj)
    {
        float distance = Vector3.Distance(enteredObj.transform.position, this.transform.position);
        depthOfField.focusDistance.value = distance;
    
        while (depthOfField.focusDistance.value > minForcusDistance)
        {
            if (focusCancelToken.Token.IsCancellationRequested)
            {
                return;
            }
            depthOfField.focusDistance.value -= focusValue;
            await UniTask.Delay(TimeSpan.FromMilliseconds(focusSpeed));
        }
        await UniTask.WaitUntil(() => depthOfField.focusDistance.value <= minForcusDistance);
    
    }
    
    private async UniTask FocusOut(GameObject exitedObj)
    {
        float distance = Vector3.Distance(exitedObj.transform.position, this.transform.position);
        
        while (depthOfField.focusDistance.value < distance)
        {
            if (focusCancelToken.Token.IsCancellationRequested)
            {
                return;
            }
            depthOfField.focusDistance.value += focusValue;
            await UniTask.Delay(TimeSpan.FromMilliseconds(focusSpeed));
        }
        await UniTask.WaitUntil(() => depthOfField.focusDistance.value >= distance);
    }
    
    private void StopUniTask()
    {
        focusCancelToken?.Cancel();
        focusCancelToken = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        focusCancelToken?.Dispose();    // 메모리 해제
    }

    private void OnApplicationQuit()
    {
        focusCancelToken?.Dispose();
        postProcessCamera.gameObject.SetActive(false);
    }
}