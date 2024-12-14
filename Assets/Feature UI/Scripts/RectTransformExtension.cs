
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class RectTransformExtension
{
    // �Ű������� this�� �پ� ������ Ȯ�� �޼���� ���ǵȴ�.
    // �� : outside.SetLeft(0); outside�� RectTransform�ε� ���� SetLeft�� ����.
    // ResizePreviewImage �Լ� ����
    
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
