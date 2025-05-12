
using System;

[Serializable]
public class MarkovGimmickData
{
    public enum MarkovGimmickType
    {
        Wait,
        Watch,
        Cautious,
        Danger,
        Near
    }

    public MarkovGimmickType type;
    public int activeSecTime;
}

[Serializable]
struct StateProbabilityData
{
    public MarkovGimmickData.MarkovGimmickType type;
        
    /// <summary>
    /// 잡음 임계값
    /// </summary>
    public int noiseThreshold;
        
    /// <summary>
    /// 확률 증감치
    /// </summary>
    public int probabilityChangeValue;
}

public interface IMarkovGimmick
{
    public MarkovState CurrState { get; set; }
    public MarkovGimmickData.MarkovGimmickType CurrGimmickType { get; set; }
    public bool IsOn { get; set; }
    
    public MarkovState Wait { get; set; }
    public MarkovState Watch { get; set; }
    public MarkovState Cautious { get; set; }
    public MarkovState Danger { get; set; }
    public MarkovState Near { get; set; }
    
    void ChangeRandomMarkovState();
    void ChangeMarkovState(MarkovState next);
    void ChangeMarkovState(MarkovGimmickData.MarkovGimmickType type);
}
