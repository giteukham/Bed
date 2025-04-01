
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void StateActionHandler(MarkovState state);

public class MarkovState : IEquatable<MarkovState>, IComparable<MarkovState>
{
    public string Name;

    public event StateActionHandler OnStateAction;
    
    public bool Equals(MarkovState other)
    {
        return Name == other.Name;
    }

    public int CompareTo(MarkovState other)
    {
        return string.Compare(Name, other.Name, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
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
            Debug.LogWarning($"���� {curr.Name}�� ���� ���̰� ���ǵ��� �ʾҽ��ϴ�.");
            return curr;
        }

        var next = curr;
        
        foreach (var transition in transitions[curr])
        {
            if (probability >= transition.ThresholdRange.x && probability <= transition.ThresholdRange.y)
            {
                next = transition.Target;
                Debug.Log("ThresHold Range : " + transition.ThresholdRange.x + " ~ " + transition.ThresholdRange.y + "\nProbability : " + probability);
            }
        }
        
        next.Active();
        return next;
    }

    public MarkovState TransitionCurrentState(MarkovState curr)
    {
        if (!transitions.ContainsKey(curr))
        {
            Debug.LogWarning($"���� {curr.Name}�� ���� ���̰� ���ǵ��� �ʾҽ��ϴ�.");
            return curr;
        }
        
        curr.Active();
        return curr;
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
}