
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class RectTransformExtension
{
    // 매개변수에 this가 붙어 있으면 확장 메서드로 정의된다.
    // 예 : outside.SetLeft(0); outside는 RectTransform인데 원래 SetLeft는 없다.
    // ResizePreviewImage 함수 참조
    
    public static void SetSizeWithCurrentAnchors(this RectTransform rectTransform, RectTransform.Axis axis, float size)
    {
        rectTransform.SetSizeWithCurrentAnchors(axis, size);
    }
    
    public static float GetLeft(this RectTransform rectTransform)
    {
        return rectTransform.offsetMin.x;
    }
    
    public static float GetRight(this RectTransform rectTransform)
    {
        return -rectTransform.offsetMax.x;
    }
    
    public static float GetTop(this RectTransform rectTransform)
    {
        return -rectTransform.offsetMax.y;
    }
    
    public static float GetBottom(this RectTransform rectTransform)
    {
        return rectTransform.offsetMin.y;
    }
    
    public static void SetLeft(this RectTransform rectTransform, float left)
    {
        rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
    }
    
    public static void SetRight(this RectTransform rectTransform, float right)
    {
        rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
    }
    
    public static void SetTop(this RectTransform rectTransform, float top)
    {
        rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
    }
    
    public static void SetBottom(this RectTransform rectTransform, float bottom)
    {
        rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
    }
}
