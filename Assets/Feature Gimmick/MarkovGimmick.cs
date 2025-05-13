
using System;
using AbstractGimmick;

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

public abstract class MarkovGimmick : Gimmick
{
    public MarkovState CurrState { get; set; } = null;
    public MarkovGimmickData.MarkovGimmickType CurrGimmickType { get; set; }
    
    protected abstract bool IsOn { get; set; }
    
    public abstract MarkovState Wait { get; set; }
    public abstract MarkovState Watch { get; set; }
    public abstract MarkovState Cautious { get; set; }
    public abstract MarkovState Danger { get; set; }
    public abstract MarkovState Near { get; set; }
    
    public abstract void ChangeRandomMarkovState();
    public abstract void ChangeMarkovState(MarkovState next);
    public abstract void ChangeMarkovState(MarkovGimmickData.MarkovGimmickType type);
}
