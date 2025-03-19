
using UnityEngine;

public static class ResolutionUtility
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="width">�ػ� Width</param>
    /// <param name="height">�ػ� Height</param>
    /// <param name="sizeInDecreaseRate">UI ����� ���ϱ� ���� �������ų� �������� ��. ���� ��� 1920 / 3 = 640�̸� UI Width�� 640</param>
    /// <returns></returns>
    public static Vector2[] ConvertResolutionToOffset(int width, int height, float sizeInDecreaseRate = 3)
    {
        var size = ConvertResolutionToSize(width, height, sizeInDecreaseRate);

        var offsetMin = new Vector2(-size.x / 2, -size.y / 2);
        var offsetMax = new Vector2(size.x / 2, size.y / 2);

        return new[] { offsetMin, offsetMax };
    }
    
    public static Vector2 ConvertResolutionToSize(float x, float y, float userAspectRatio)
    {
        if (userAspectRatio >= StaticUIData.BaseAspectRatio)
        {
            y /= (Display.main.systemHeight / (StaticUIData.BaseHeight / 3f));
            x = y * StaticUIData.BaseAspectRatio;
        }
        else
        {
            x /= (Display.main.systemWidth / (StaticUIData.BaseWidth / 3f));
            y = x * StaticUIData.BaseReverseAspectRatio;
        }
        
        return new Vector2(x, y);
    }
    
    public static Vector2[] ConvertSizeToOffset(Vector2 size)
    {
        var offsetMin = new Vector2(-size.x / 2f, -size.y / 2f);
        var offsetMax = new Vector2(size.x / 2f, size.y / 2f);

        return new[] { offsetMin, offsetMax };
    }
    
    public static Vector2Int ConvertSizeToResolution(Vector2 size, float sizeInDecreaseRate = 3)
    {
        var width = size.x * sizeInDecreaseRate;
        var height = size.y * sizeInDecreaseRate;
        
        return new Vector2Int(Mathf.CeilToInt(width), Mathf.CeilToInt(height));
    }
    
    public static Vector2 ConvertOffsetsToSize(Vector2[] offset)
    {
        var sizeWidth = offset[1].x - offset[0].x;
        var sizeHeight = offset[1].y - offset[0].y;
        
        return new Vector2(sizeWidth, sizeHeight);
    }
    
    public static Vector2 ConvertOffsetsToResolution(Vector2[] offset, float sizeInDecreaseRate = 3)
    {
        var size = ConvertOffsetsToSize(offset);
        var resolution = ConvertSizeToResolution(size, sizeInDecreaseRate);
        
        return resolution;
    }

    public static Vector2 Max(Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }
 
}
