
using UnityEngine;

public static class ResolutionUtility
{
    public static Vector2[] ConvertResolutionToOffset(RectTransform rect, ResolutionSettingsData data)
    {
         Vector2 size = ConvertResolutionToSize(data);
         
         Vector2 offsetMin = new Vector2(-size.x / 2, -size.y / 2);
         Vector2 offsetMax = new Vector2(size.x / 2, size.y / 2);
         
         return new[] { offsetMin, offsetMax };
    }
    
    public static Vector2 ConvertResolutionToSize(ResolutionSettingsData data)
    {
        float aspectRatio = data.ResolutionWidth / (float)data.ResolutionHeight;
        float height = data.ResolutionHeight;
        float width = height * aspectRatio;
        
        return new Vector2(width, height);
    }
}
