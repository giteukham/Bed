
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
