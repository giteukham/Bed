
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
    public static Vector2[] ConvertResolutionToOffset(int width, int height, float userAspectRatio)
    {
        var size = ConvertResolutionToSize(width, height, userAspectRatio);

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

    public static Vector2 Max(Vector2 a, Vector2 b)
    {
        return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }
 
}
