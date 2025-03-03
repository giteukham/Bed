
using UnityEngine;

public static class ResolutionUtility
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="width">해상도 Width</param>
    /// <param name="height">해상도 Height</param>
    /// <param name="sizeInDecreaseRate">UI 사이즈를 구하기 위해 나눠지거나 곱해지는 값. 예를 들어 1920 / 3 = 640이면 UI Width는 640</param>
    /// <returns></returns>
    public static Vector2[] ConvertResolutionToOffset(int width, int height, float sizeInDecreaseRate = 3)
    {
        var size = ConvertResolutionToSize(width, height, sizeInDecreaseRate);

        var offsetMin = new Vector2(-size.x / 2, -size.y / 2);
        var offsetMax = new Vector2(size.x / 2, size.y / 2);

        return new[] { offsetMin, offsetMax };
    }

    public static Vector2 ConvertResolutionToSize(int width, int height, float sizeInDecreaseRate = 3)
    {
        var sizeWidth = width / sizeInDecreaseRate;
        var sizeHeight = height / sizeInDecreaseRate;
        
        return new Vector2(sizeWidth, sizeHeight);
    }
    
    public static Vector2[] ConvertSizeToOffset(Vector2 size)
    {
        var offsetMin = new Vector2(-size.x / 2, -size.y / 2);
        var offsetMax = new Vector2(size.x / 2, size.y / 2);

        return new[] { offsetMin, offsetMax };
    }
}
