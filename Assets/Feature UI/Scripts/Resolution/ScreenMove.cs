
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenMove : MonoBehaviour, IDragHandler
{
    private ResolutionManagement resolutionManager;
    
    private void OnEnable()
    {
        resolutionManager = ResolutionManagement.Instance;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.delta;
        Vector2 insideAnchoredPosition = resolutionManager.InsideAnchoredPosition;
        Vector2 insideSize = resolutionManager.InsideSize;
        Vector2 outsideSize = resolutionManager.OutsideSize;
        
        float distanceX = (outsideSize.x - insideSize.x) * 0.5f;
        float distanceY = (outsideSize.y - insideSize.y) * 0.5f;
        
        insideAnchoredPosition = new Vector2(
            Mathf.Clamp(insideAnchoredPosition.x + delta.x, -distanceX, distanceX),
            Mathf.Clamp(insideAnchoredPosition.y + delta.y, -distanceY, distanceY));
        
        resolutionManager.InsideAnchoredPosition = insideAnchoredPosition;
    }
}
