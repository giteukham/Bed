
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
