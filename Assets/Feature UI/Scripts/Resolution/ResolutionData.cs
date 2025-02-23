
using UnityEngine;

public struct ResolutionData
{
    public int width, height;
    public int frameRate;
    public bool isWindowedScreen;
    
    public ResolutionData(int width, int height, int frameRate, bool isWindowedScreen)
    {
        this.width = width;
        this.height = height;
        this.frameRate = frameRate;
        this.isWindowedScreen = isWindowedScreen;
    }
    
    public bool Equals(ResolutionData other)
    {
        //Debug.Log($"실제 값: {width}, {height}, {frameRate}, {isWindowedScreen}, 준비 값: {other.width}, {other.height}, {other.frameRate}, {other.isWindowedScreen}");
        return width == other.width && height == other.height && frameRate == other.frameRate && isWindowedScreen == other.isWindowedScreen;
    }
}
