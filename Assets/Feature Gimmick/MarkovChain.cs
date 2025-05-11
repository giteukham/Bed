
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void StateActionHandler(MarkovState state);

public class MarkovState
{
    public int ActiveCount { get; set; } = 0;
    
    private readonly string name;
    public string Name => name;
    public MarkovGimmickData.MarkovGimmickType Type;

    public event StateActionHandler OnStateAction;
    
    public MarkovState(string name)
    {
        this.name = name;
        this.Type = (MarkovGimmickData.MarkovGimmickType) Enum.Parse(typeof(MarkovGimmickData.MarkovGimmickType), name);
    }

    public void Active()
    {
        if (OnStateAction == null)
        {
            Debug.LogWarning("OnStateAction�� null�Դϴ�.");
            return;
        }
        
        OnStateAction(this);
    }

    public override bool Equals(object obj)
    {
        var markovState = obj as MarkovState;
        return name == markovState.name;
    }

    public override int GetHashCode()
    {
        return name != null ? name.GetHashCode() : 0;
    }
}

public class MarkovTransition
{
    public MarkovState Target;
    public Vector2 ThresholdRange;
}

public class MarkovChain
{
    private readonly Dictionary<MarkovState, List<MarkovTransition>> transitions = new();

    public List<MarkovTransition> this[MarkovState state]
    {
        get
        {
            SortASC(state);
            return transitions[state];
        }
    }

    public void InsertTransition(MarkovState from, List<MarkovTransition> to)
    {
        transitions.Add(from, to);
    }

    public void RemoveTransition(MarkovState from, MarkovTransition transition)
    {
        if (transitions.TryGetValue(from, out var to))
        {
            to.Remove(transition);
        }
        else
        {
            Debug.LogWarning("�����Ϸ��� �ϴ� ���� ������ �������� �ʽ��ϴ�.");
        }
    }
    
    public MarkovState TransitionNextState(MarkovState curr, int probability)
    {
        if (!transitions.ContainsKey(curr))
        {
            Debug.LogWarning($"���� {curr}�� ���� ���̰� ���ǵ��� �ʾҽ��ϴ�.");
            return curr;
        }

        var next = curr;
        
        foreach (var transition in transitions[curr])
        {
            if (probability >= transition.ThresholdRange.x && probability <= transition.ThresholdRange.y)
            {
                next = transition.Target;
            }
        }
        
        next.Active();
        next.ActiveCount++;
        
        return next;
    }

    public MarkovState TransitionNextState(MarkovState next)
    {
        if (!transitions.ContainsKey(next))
        {
            Debug.LogWarning($"���� {next.Name}�� ���� ���̰� ���ǵ��� �ʾҽ��ϴ�.");
            return next;
        }
        
        next.Active();
        next.ActiveCount++;
        
        return next;
    }

    public void SortASC(MarkovState sortingState)
    {
        if (!transitions.ContainsKey(sortingState))
        {
            Debug.LogWarning($"���°� �����ϴ�.");
            return;
        }
        
        transitions[sortingState] = transitions[sortingState].OrderBy(x => x.ThresholdRange.y).ToList();
    }

    public void InitStateCount()
    {
        foreach (var state in transitions.Keys)
        {
            state.ActiveCount = 0;
        }
    }
}