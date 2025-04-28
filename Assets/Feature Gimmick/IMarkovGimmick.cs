
public interface IMarkovGimmick
{
    public MarkovState CurrState { get; set; }
    
    public MarkovState Wait { get; set; }
    public MarkovState Watch { get; set; }
    public MarkovState Cautious { get; set; }
    public MarkovState Danger { get; set; }
    public MarkovState Near { get; set; }
    
    void ChangeRandomMarkovState();
    void ChangeMarkovState(MarkovState next);
}
