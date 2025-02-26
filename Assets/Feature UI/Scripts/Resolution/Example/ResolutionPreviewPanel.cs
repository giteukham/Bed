using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPreviewPanel : MonoBehaviour
{
    // 해상도 영역
    [SerializeField] private RectTransform gameViewArea; 
    private ResolutionSettingsManegment settingsManagement;

    private void Awake()
    {
        settingsManagement = FindObjectOfType<ResolutionSettingsManegment>();
        if (settingsManagement != null) settingsManagement.OnResolutionChanged.AddListener(UpdatePreview);
    }
    
    private void UpdatePreview(int x, int y)
    {
        // 프리뷰 크기 변경 내용
    }

    public void OnDrag() // 직접 드래그해서 바꿀때 이거
    {
        //settingsManagement.OnPreviewResized(값, 값);
    }
}
