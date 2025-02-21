
using UnityEngine;

public struct ResolutionData
{
    public int width, height;
    public float frameRate;
    public bool isWindowedScreen;
    
    public ResolutionData(int width, int height, float frameRate, bool isWindowedScreen)
    {
        this.width = width;
        this.height = height;
        this.frameRate = frameRate;
        this.isWindowedScreen = isWindowedScreen;
    }
    
    public bool Equals(ResolutionData other)
    {
        return width == other.width && height == other.height && Mathf.Approximately(frameRate, other.frameRate) && isWindowedScreen == other.isWindowedScreen;
    }
}
