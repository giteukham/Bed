
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public delegate void StateActionHandler(MarkovState state);

public class MarkovState : IEquatable<MarkovState>
{
    public string name;

    public event StateActionHandler OnStateAction;
    
    public bool Equals(MarkovState other)
    {
        return name == other.name;
    }

    public override bool Equals(object obj)
    {
        return obj is MarkovState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return name != null ? name.GetHashCode() : 0;
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

public class MarkovChain
{
    private Dictionary<MarkovState, Dictionary<MarkovState, int>> transitions = new();

    /// <summary>
    /// Ư�� ���¿� ��ϵǾ� �ִ� ��� ���� Ȯ���� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="state"></param>
    public int[] this[MarkovState state]
    {
        get { return transitions.TryGetValue(state, out var t) ? t.Values.ToArray() : null; }
    }

    /// <summary>
    /// ���� ���� ���� ��� �Լ�
    /// to �Ű����� ������ probabilities�� ������ �����ϰ� ����� �մϴ�.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="probabilities">���� Ȯ��</param>
    public void InsertTransition(MarkovState from, MarkovState[] to, int[] probabilities)
    {
        if (to.Length != probabilities.Length)
        {
            Debug.LogWarning("���¿� Ȯ���� ������ ��ġ���� �ʽ��ϴ�.");
            return;
        }
        
        if (!transitions.ContainsKey(from))
        {
            transitions[from] = new Dictionary<MarkovState, int>();
        }
            
        for (int i = 0; i < to.Length; i++)
        {
            transitions[from][to[i]] = probabilities[i];
        }
    }

    public void RemoveTransition(MarkovState from, MarkovState to)
    {
        if (transitions.TryGetValue(from, out var toDict))
        {
            toDict.Remove(to);
        }
        else
        {
            Debug.LogWarning("�����Ϸ��� �ϴ� ���°� �������� �ʽ��ϴ�.");
        }
    }

    /// <summary>
    /// ��ϵ� ���� Ȯ���� probability���� �۰ų� ������ ���� ���·� �����մϴ�.
    /// </summary>
    /// <param name="curr"></param>
    /// <param name="probability"></param>
    /// <returns>���� ����</returns>
    public MarkovState GetNextState(MarkovState curr, int probability)
    {
        if (!transitions.ContainsKey(curr))
        {
            Debug.LogWarning($"���� {curr.name}�� ���� ���̰� ���ǵ��� �ʾҽ��ϴ�.");
            return curr;
        }
        
        var next = curr;
        foreach (var (state, prob) in transitions[curr])
        {
            if (probability >= prob)
            {
                next = state;
            }
        }
        
        next.Active();
        return next;
    }

    public MarkovState GetCurrentState(MarkovState curr)
    {
        if (!transitions.ContainsKey(curr))
        {
            Debug.LogWarning($"���� {curr.name}�� ���� ���̰� ���ǵ��� �ʾҽ��ϴ�.");
            return curr;
        }
        
        curr.Active();
        return curr;
    }
}